using ARWNI2S.Engine.Hosting.Extensions;
using ARWNI2S.Infrastructure;
using ARWNI2S.Infrastructure.Engine.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ARWNI2S.Engine.Infrastructure
{
    /// <summary>
    /// Represents object for the configuring services on application startup
    /// </summary>
    public partial class ProxyStartup : INI2SStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration) { }

        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="builder">Builder for configuring an application's request pipeline</param>
        public void Configure(IEngineBuilder builder)
        {
            builder.UseNodeProxy();
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => -1; // Forwarded headers by HTTP proxy should be applied before calling other middleware.
    }
}