using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ARWNI2S.Engine.Hosting
{
    /// <summary>
    /// Builder for configuring a GDESK.
    /// </summary>
    public interface IGDESKBuilder
    {
        /// <summary>
        /// The services shared by the kernel and host.
        /// </summary>
        IServiceCollection Services { get; }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        IConfiguration Configuration { get; }
    }
}