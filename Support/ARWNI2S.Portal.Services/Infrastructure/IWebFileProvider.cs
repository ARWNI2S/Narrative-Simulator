using ARWNI2S.Infrastructure.Engine;

namespace ARWNI2S.Portal.Services.Infrastructure
{
    public interface IWebFileProvider : IEngineFileProvider
    {
        /// <summary>
        /// Gets or sets the absolute path to the directory that contains the web-servable application content files.
        /// </summary>
        string WebRootPath { get; }

    }
}