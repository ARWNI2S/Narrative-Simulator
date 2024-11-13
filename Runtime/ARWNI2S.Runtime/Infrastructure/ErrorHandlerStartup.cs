using ARWNI2S.Infrastructure;
using ARWNI2S.Runtime.Hosting.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// INIT STEP 2.1
namespace ARWNI2S.Runtime.Infrastructure
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
        /// <param name="configuration">Configuration of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
        }

        /// <summary>
        /// Configure the using of added services
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IHost application)
        {
            //exception handling
            application.UseNI2SExceptionHandler();

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