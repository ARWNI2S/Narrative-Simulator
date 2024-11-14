using ARWNI2S.Engine.Hosting.Extensions;
using ARWNI2S.Infrastructure;
using ARWNI2S.Infrastructure.Engine.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// INIT STEP 2.1
namespace ARWNI2S.Node.Infrastructure
{
    /// <summary>
    /// Represents object for the configuring exceptions and errors handling on application startup
    /// </summary>
    public partial class ErrorHandlerStartup : INI2SStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the engine</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
        }

        /// <summary>
        /// Configure the using of added services
        /// </summary>
        /// <param name="engine">Builder for configuring an application's request pipeline</param>
        public void Configure(IEngineBuilder engine)
        {
            //exception handling
            engine.UseNI2SExceptionHandler();

            ////handle 400 errors (bad request)
            //application.UseBadRequestResult();

            ////handle 404 errors (not found)
            //application.UseNotFoundResult();
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => 0; //error handlers should be loaded first
    }
}