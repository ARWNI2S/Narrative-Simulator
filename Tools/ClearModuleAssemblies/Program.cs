using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ClearModuleAssemblies
{
    public class Program
    {
        protected const string FILES_TO_DELETE = "dotnet-bundle.exe;ARWNI2S.Node.pdb;ARWNI2S.Node.exe;ARWNI2S.Node.exe.config;";
            //"ARWNI2S.Node.Core.pdb;ARWNI2S.Node.Core.dll;ARWNI2S.Node.Data.pdb;ARWNI2S.Node.Data.dll;ARWNI2S.Node.Services.pdb;ARWNI2S.Node.Services.dll";

        protected static void Clear(string paths, IList<string> fileNames, bool saveLocalesFolders)
        {
            foreach (var modulePath in paths.Split(';'))
            {
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

        private static void Main(string[] args)
        {
            var outputPath = string.Empty;
            var modulePaths = string.Empty;
            var saveLocalesFolders = true;

            var settings = args.FirstOrDefault(a => a.Contains('|')) ?? string.Empty;
            if (string.IsNullOrEmpty(settings))
                return;

            foreach (var arg in settings.Split('|'))
            {
                var data = arg.Split("=").Select(p => p.Trim()).ToList();

                var name = data[0];
                var value = data.Count > 1 ? data[1] : string.Empty;

                switch (name)
                {
                    case "OutputPath":
                        outputPath = value;
                        break;
                    case "ModulePath":
                        modulePaths = value;
                        break;
                    case "SaveLocalesFolders":
                        _ = bool.TryParse(value, out saveLocalesFolders);
                        break;
                }
            }

            if (!Directory.Exists(outputPath))
                return;

            var di = new DirectoryInfo(outputPath);
            var separator = Path.DirectorySeparatorChar;
            var folderToIgnore = string.Concat(separator, "Modules", separator);
            var fileNames = di.GetFiles("*.dll", SearchOption.AllDirectories)
                .Where(fi => !fi.FullName.Contains(folderToIgnore))
                .Select(fi => fi.Name.Replace(fi.Extension, "")).ToList();

            if (string.IsNullOrEmpty(modulePaths) || fileNames.Count == 0)
            {
                return;
            }

            Clear(modulePaths, fileNames, saveLocalesFolders);
        }
    }
}