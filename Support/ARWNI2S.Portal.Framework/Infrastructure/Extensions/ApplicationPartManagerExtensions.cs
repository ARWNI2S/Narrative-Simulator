using ARWNI2S.Infrastructure;
using ARWNI2S.Node.Core.ComponentModel;
using ARWNI2S.Node.Core.Configuration;
using ARWNI2S.Node.Core.Infrastructure;
using ARWNI2S.Node.Data.Mapping;
using ARWNI2S.Node.Services.Plugins;
using System.Reflection;

namespace ARWNI2S.Portal.Framework.Infrastructure.Extensions
{
    /// <summary>
    /// Represents application part manager extensions
    /// </summary>
    public static partial class ApplicationPartManagerExtensions
    {
        #region Fields

        private static readonly IEngineFileProvider _fileProvider;
        private static readonly List<KeyValuePair<string, Version>> _baseAppLibraries;
        private static readonly Dictionary<string, Version> _moduleLibraries;
        private static readonly Dictionary<string, ModuleLoadedAssemblyInfo> _loadedAssemblies = [];
        private static readonly ReaderWriterLockSlim _locker = new();

        #endregion

        #region Ctor

        static ApplicationPartManagerExtensions()
        {
            //we use the default file provider, since the DI isn't initialized yet
            _fileProvider = CommonHelper.DefaultFileProvider;

            _baseAppLibraries = [];
            _moduleLibraries = [];

            //get all libraries from /bin/{version}/ directory
            foreach (var file in _fileProvider.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll"))
                _baseAppLibraries.Add(new KeyValuePair<string, Version>(_fileProvider.GetFileName(file), GetAssemblyVersion(file)));

            //get all libraries from base site directory
            if (!AppDomain.CurrentDomain.BaseDirectory.Equals(Environment.CurrentDirectory, StringComparison.InvariantCultureIgnoreCase))
                foreach (var file in _fileProvider.GetFiles(Environment.CurrentDirectory, "*.dll"))
                    _baseAppLibraries.Add(new KeyValuePair<string, Version>(_fileProvider.GetFileName(file), GetAssemblyVersion(file)));

            //get all libraries from refs directory
            var refsPathName = _fileProvider.Combine(Environment.CurrentDirectory, ModuleServicesDefaults.RefsPathName);
            if (_fileProvider.DirectoryExists(refsPathName))
                foreach (var file in _fileProvider.GetFiles(refsPathName, "*.dll"))
                    _baseAppLibraries.Add(new KeyValuePair<string, Version>(_fileProvider.GetFileName(file), GetAssemblyVersion(file)));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets access to information about modules
        /// </summary>
        private static IModulesInfo ModulesInfo
        {
            get => Singleton<IModulesInfo>.Instance;
            set => Singleton<IModulesInfo>.Instance = value;
        }

        #endregion

        #region Utilities

        private static Version GetAssemblyVersion(string filePath)
        {
            try
            {
                return AssemblyName.GetAssemblyName(filePath).Version;
            }
            catch (BadImageFormatException)
            {
                //ignore
            }

            return null;
        }

        private static void CheckCompatible(ModuleDescriptor moduleDescriptor, Dictionary<string, Version> assemblies)
        {
            var refFiles = moduleDescriptor.ModuleFiles.Where(file =>
                !_fileProvider.GetFileName(file).Equals(_fileProvider.GetFileName(moduleDescriptor.OriginalAssemblyFile))).ToList();

            foreach (var refFile in refFiles.Where(file =>
                         assemblies.ContainsKey(_fileProvider.GetFileName(file).ToLower())))
                IsAlreadyLoaded(refFile, moduleDescriptor.SystemName);

            var hasCollisions = _loadedAssemblies.Where(p =>
                    p.Value.References.Any(r => r.ModuleName.Equals(moduleDescriptor.SystemName)))
                .Any(p => p.Value.Collisions.Any());

            if (hasCollisions)
            {
                ModulesInfo.IncompatibleModules.Add(moduleDescriptor.SystemName, ModuleIncompatibleType.HasCollisions);
                ModulesInfo.ModuleDescriptors.Remove((moduleDescriptor, false));
            }
        }

        /// <summary>
        /// Load and register the assembly
        /// </summary>
        /// <param name="applicationPartManager">Application part manager</param>
        /// <param name="assemblyFile">Path to the assembly file</param>
        /// <param name="useUnsafeLoadAssembly">Indicating whether to load an assembly into the load-from context, bypassing some security checks</param>
        /// <returns>Assembly</returns>
        private static Assembly AddApplicationParts(ApplicationPartManager applicationPartManager, string assemblyFile, bool useUnsafeLoadAssembly)
        {
            //try to load a assembly
            Assembly assembly;

            try
            {
                assembly = Assembly.LoadFrom(assemblyFile);
            }
            catch (FileLoadException)
            {
                if (useUnsafeLoadAssembly)
                {
                    //if an application has been copied from the web, it is flagged by Windows as being a web application,
                    //even if it resides on the local computer.You can change that designation by changing the file properties,
                    //or you can use the<loadFromRemoteSources> element to grant the assembly full trust.As an alternative,
                    //you can use the UnsafeLoadFrom method to load a local assembly that the operating system has flagged as
                    //having been loaded from the web.
                    //see http://go.microsoft.com/fwlink/?LinkId=155569 for more information.
                    assembly = Assembly.UnsafeLoadFrom(assemblyFile);
                }
                else
                    throw;
            }

            //register the module definition
            applicationPartManager.ApplicationParts.Add(new AssemblyPart(assembly));

            return assembly;
        }

        /// <summary>
        /// Perform file deploy and return loaded assembly
        /// </summary>
        /// <param name="applicationPartManager">Application part manager</param>
        /// <param name="assemblyFile">Path to the module assembly file</param>
        /// <param name="moduleConfig">Module config</param>
        /// <param name="fileProvider">DraCo file provider</param>
        /// <returns>Assembly</returns>
        private static Assembly PerformFileDeploy(this ApplicationPartManager applicationPartManager,
            string assemblyFile, ModuleConfig moduleConfig, IEngineFileProvider fileProvider)
        {
            //ensure for proper directory structure
            if (string.IsNullOrEmpty(assemblyFile) ||
                string.IsNullOrEmpty(fileProvider.GetParentDirectory(assemblyFile)))
                throw new InvalidOperationException(
                    $"The module directory for the {fileProvider.GetFileName(assemblyFile)} file exists in a directory outside of the allowed dragonCorp directory hierarchy");

            var assembly =
                AddApplicationParts(applicationPartManager, assemblyFile, moduleConfig.UseUnsafeLoadAssembly);

            // delete the .deps file
            if (assemblyFile.EndsWith(".dll"))
                _fileProvider.DeleteFile(assemblyFile[0..^4] + ".deps.json");

            if (!_moduleLibraries.ContainsKey(fileProvider.GetFileName(assemblyFile)))
                _moduleLibraries.Add(fileProvider.GetFileName(assemblyFile), assembly.GetName().Version);

            return assembly;
        }

        /// <summary>
        /// Check whether the assembly is already loaded
        /// </summary>
        /// <param name="filePath">Assembly file path</param>
        /// <param name="moduleName">Module system name</param>
        /// <returns>Result of check</returns>
        private static bool IsAlreadyLoaded(string filePath, string moduleName)
        {
            //ignore already loaded libraries
            //(we do it because not all libraries are loaded immediately after application start)
            var fileName = _fileProvider.GetFileName(filePath);
            if (_baseAppLibraries.Any(library => library.Key.Equals(fileName, StringComparison.InvariantCultureIgnoreCase)))
                return true;

            try
            {
                //get filename without extension
                var fileNameWithoutExtension = _fileProvider.GetFileNameWithoutExtension(filePath);
                if (string.IsNullOrEmpty(fileNameWithoutExtension))
                    throw new PortalException($"Cannot get file extension for {fileName}");

                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    //compare assemblies by file names
                    var assemblyName = (assembly.FullName ?? string.Empty).Split(',').FirstOrDefault();
                    if (!fileNameWithoutExtension.Equals(assemblyName, StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    //loaded assembly not found
                    if (!_loadedAssemblies.TryGetValue(assemblyName, out ModuleLoadedAssemblyInfo value))
                    {
                        value = new ModuleLoadedAssemblyInfo(assemblyName, GetAssemblyVersion(assembly.Location));
                        //add it to the list to find collisions later
                        _loadedAssemblies.Add(assemblyName, value);
                    }

                    value.References.Add((moduleName, GetAssemblyVersion(filePath)));

                    return true;
                }
            }
            catch
            {
                // ignored
            }

            //nothing found
            return false;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize modules system
        /// </summary>
        /// <param name="applicationPartManager">Application part manager</param>
        /// <param name="moduleConfig">Module config</param>
        public static void InitializeModules(this ApplicationPartManager applicationPartManager, ModuleConfig moduleConfig)
        {
            ArgumentNullException.ThrowIfNull(applicationPartManager);

            ArgumentNullException.ThrowIfNull(moduleConfig);

            //perform with locked access to resources
            using (new ReaderWriteLockDisposable(_locker))
            {
                try
                {
                    //ensure modules directory is created
                    var modulesDirectory = _fileProvider.MapPath(ModuleServicesDefaults.Path);
                    _fileProvider.CreateDirectory(modulesDirectory);

                    //ensure uploaded directory is created
                    var uploadedPath = _fileProvider.MapPath(ModuleServicesDefaults.UploadedPath);
                    _fileProvider.CreateDirectory(uploadedPath);

                    foreach (var directory in _fileProvider.GetDirectories(uploadedPath))
                    {
                        var moveTo = _fileProvider.Combine(modulesDirectory, _fileProvider.GetDirectoryNameOnly(directory));

                        if (_fileProvider.DirectoryExists(moveTo))
                            _fileProvider.DeleteDirectory(moveTo);

                        _fileProvider.DirectoryMove(directory, moveTo);
                    }

                    ModulesInfo = new ModulesInfo(_fileProvider);
                    ModulesInfo.LoadModuleInfo();

                    foreach (var moduleDescriptor in ModulesInfo.ModuleDescriptors.Where(p => p.needToDeploy)
                                 .Select(p => p.moduleDescriptor))
                    {
                        var mainModuleFile = moduleDescriptor.OriginalAssemblyFile;

                        //try to deploy main module assembly 
                        moduleDescriptor.ReferencedAssembly =
                            applicationPartManager.PerformFileDeploy(mainModuleFile, moduleConfig, _fileProvider);

                        //and then deploy all other referenced assemblies
                        var filesToDeploy = moduleDescriptor.ModuleFiles.Where(file =>
                            !_fileProvider.GetFileName(file).Equals(_fileProvider.GetFileName(mainModuleFile)) &&
                            !IsAlreadyLoaded(file, moduleDescriptor.SystemName)).ToList();

                        foreach (var file in filesToDeploy)
                            applicationPartManager.PerformFileDeploy(file, moduleConfig, _fileProvider);

                        //determine a module type (only one module per assembly is allowed)
                        var moduleType = moduleDescriptor.ReferencedAssembly.GetTypes().FirstOrDefault(type =>
                            typeof(IModule).IsAssignableFrom(type) && !type.IsInterface && type.IsClass &&
                            !type.IsAbstract);

                        if (moduleType != default)
                            moduleDescriptor.ModuleType = moduleType;
                    }


                    var assemblies = _baseAppLibraries.ToList();
                    foreach (var moduleLoadedAssemblyInfo in _loadedAssemblies)
                        assemblies.Add(new KeyValuePair<string, Version>(moduleLoadedAssemblyInfo.Key, moduleLoadedAssemblyInfo.Value.AssemblyInMemory));

                    foreach (var moduleLibrary in _moduleLibraries.Where(item => !assemblies.Any(p => p.Key.Equals(item.Key, StringComparison.InvariantCultureIgnoreCase))).ToList())
                        assemblies.Add(new KeyValuePair<string, Version>(moduleLibrary.Key, moduleLibrary.Value));

                    var inMemoryAssemblies = assemblies.GroupBy(p => p.Key).Select(p => p.First())
                        .ToDictionary(p => p.Key.ToLower(), p => p.Value);

                    foreach (var moduleDescriptor in ModulesInfo.ModuleDescriptors.Where(p => !p.needToDeploy)
                                 .Select(p => p.moduleDescriptor).ToList())
                        CheckCompatible(moduleDescriptor, inMemoryAssemblies);
                }
                catch (Exception exception)
                {
                    //throw full exception
                    var message = string.Empty;
                    for (var inner = exception; inner != null; inner = inner.InnerException)
                        message = $"{message}{inner.Message}{Environment.NewLine}";

                    throw new PortalException(message, exception);
                }

                ModulesInfo.AssemblyLoadedCollision = _loadedAssemblies.Select(item => item.Value)
                    .Where(loadedAssemblyInfo => loadedAssemblyInfo.Collisions.Any()).ToList();

                //add name compatibility types from modules
                var nameCompatibilityList = ModulesInfo.ModuleDescriptors.Where(pd => pd.moduleDescriptor.Installed).SelectMany(pd => pd
                    .moduleDescriptor.ReferencedAssembly.GetTypes().Where(type =>
                        typeof(INameCompatibility).IsAssignableFrom(type) && !type.IsInterface && type.IsClass &&
                        !type.IsAbstract));
                NameCompatibilityManager.AdditionalNameCompatibilities.AddRange(nameCompatibilityList);
            }
        }

        #endregion
    }
}
