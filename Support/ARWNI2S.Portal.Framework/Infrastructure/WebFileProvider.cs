using ARWNI2S.Node.Core.Infrastructure;
using ARWNI2S.Portal.Services.Infrastructure;

namespace ARWNI2S.Portal.Framework.Infrastructure
{
    /// <summary>
    /// IO functions using the on-disk file system
    /// </summary>
    public partial class WebFileProvider : EngineFileProvider, IWebFileProvider
    {
        public WebFileProvider(IWebHostEnvironment webHostEnvironment)
            : base(File.Exists(webHostEnvironment.ContentRootPath) ? Path.GetDirectoryName(webHostEnvironment.ContentRootPath) : webHostEnvironment.ContentRootPath)
        {
            WebRootPath = File.Exists(webHostEnvironment.WebRootPath)
                ? Path.GetDirectoryName(webHostEnvironment.WebRootPath)
                : webHostEnvironment.WebRootPath;
        }

        public string WebRootPath { get; }

        /// <summary>
        /// Returns the absolute path to the directory
        /// </summary>
        /// <param name="paths">An array of parts of the path</param>
        /// <returns>The absolute path to the directory</returns>
        public override string GetAbsolutePath(params string[] paths)
        {
            var allPaths = new List<string>();

            if (paths.Length != 0 && !paths[0].Contains(WebRootPath, StringComparison.InvariantCulture))
                allPaths.Add(WebRootPath);

            allPaths.AddRange(paths);

            return Combine([.. allPaths]);
        }


    }
}
