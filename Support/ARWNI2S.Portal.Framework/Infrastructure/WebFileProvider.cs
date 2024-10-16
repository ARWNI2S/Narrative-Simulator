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
    }
}
