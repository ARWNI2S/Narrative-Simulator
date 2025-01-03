using Microsoft.Build.Framework;

namespace FrameworkAssembliesTasks
{
    public class ClearModuleAssembliesTask : Microsoft.Build.Utilities.Task
    {
        [Required]
        public string OutputPath { get; set; }

        public string ModulePath { get; set; }

        public bool SaveLocalesFolders { get; set; } = true;

        protected const string FILES_TO_DELETE = "dotnet-bundle.exe;" +
    "ARWNI2S.Abstractions.pdb;ARWNI2S.Abstractions.dll;ARWNI2S.Abstractions.xml;" +
    "ARWNI2S.Engine.Core.pdb;ARWNI2S.Engine.Core.dll;ARWNI2S.Engine.Core.xml;" +
    "ARWNI2S.Engine.Data.pdb;ARWNI2S.Engine.Data.dll;ARWNI2S.Engine.Data.xml;" +
    "ARWNI2S.Engine.Cluster.pdb;ARWNI2S.Engine.Cluster.dll;ARWNI2S.Engine.Cluster.xml;" +
    "ARWNI2S.Node.Runtime.pdb;ARWNI2S.Node.Runtime.dll;ARWNI2S.Node.Runtime.xml;" +
    "ARWNI2S.Node.pdb;ARWNI2S.Node.dll;ARWNI2S.Node.xml";

        protected void Clear(string paths, IList<string> fileNames, bool saveLocalesFolders)
        {
            foreach (var modulePath in paths.Split(';'))
            {
                Log.LogMessage("modulePath: {0}", modulePath);

                try
                {
                    var moduleDirectoryInfo = new DirectoryInfo(modulePath);
                    var allDirectoryInfo = new List<DirectoryInfo> { moduleDirectoryInfo };

                    if (!saveLocalesFolders)
                        allDirectoryInfo.AddRange(moduleDirectoryInfo.GetDirectories());

                    foreach (var directoryInfo in allDirectoryInfo)
                    {
                        foreach (var fileName in fileNames)
                        {
                            //delete dll file if it exists in current path
                            var dllfilePath = Path.Combine(directoryInfo.FullName, fileName + ".dll");
                            if (File.Exists(dllfilePath))
                                File.Delete(dllfilePath);
                            //delete pdb file if it exists in current path
                            var pdbfilePath = Path.Combine(directoryInfo.FullName, fileName + ".pdb");
                            if (File.Exists(pdbfilePath))
                                File.Delete(pdbfilePath);
                            //delete xml file if it exists in current path
                            var xmlfilePath = Path.Combine(directoryInfo.FullName, fileName + ".xml");
                            if (File.Exists(xmlfilePath))
                                File.Delete(xmlfilePath);
                        }

                        foreach (var fileName in FILES_TO_DELETE.Split(';'))
                        {
                            //delete file if it exists in current path
                            var pdbfilePath = Path.Combine(directoryInfo.FullName, fileName);
                            if (File.Exists(pdbfilePath))
                                File.Delete(pdbfilePath);
                        }

                        if (directoryInfo.GetFiles().Length == 0 && directoryInfo.GetDirectories().Length == 0 && !saveLocalesFolders)
                            directoryInfo.Delete(true);
                    }
                }
                catch
                {
                    //do nothing
                }
            }
        }

        public override bool Execute()
        {
            try
            {
                Log.LogMessage("OutputPath: {0}", OutputPath);
                Log.LogMessage("ModulePath: {0}", ModulePath);

                if (!Directory.Exists(OutputPath))
                    return true;

                var di = new DirectoryInfo(OutputPath);
                var separator = Path.DirectorySeparatorChar;
                var folderToIgnore = string.Concat(separator, "Plugins", separator);
                var fileNames = di.GetFiles("*.dll", SearchOption.AllDirectories)
                    .Where(fi => !fi.FullName.Contains(folderToIgnore))
                    .Select(fi => fi.Name.Replace(fi.Extension, "")).ToList();

                if (string.IsNullOrEmpty(ModulePath) || fileNames.Count == 0)
                {
                    return true;
                }

                Clear(ModulePath, fileNames, SaveLocalesFolders);

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }

    }
}
