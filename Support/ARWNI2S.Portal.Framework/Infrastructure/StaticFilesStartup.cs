using ARWNI2S.Portal.Framework.Infrastructure.Extensions;

namespace ARWNI2S.Portal.Framework.Infrastructure
{
    /// <summary>
    /// Represents class for the configuring routing on application startup
    /// </summary>
    public partial class StaticFilesStartup : IWebStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //compression
            services.AddResponseCompression();

            //middleware for bundling and minification of CSS and JavaScript files.
            services.AddDraCoWebOptimizer();
        }

        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
            //use response compression before UseDraCoStaticFiles to support compress for it
            application.UseDraCoResponseCompression();

            //WebOptimizer should be placed before configuring static files
            application.UseDraCoWebOptimizer();

            //use static files feature
            application.UseDraCoStaticFiles();
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => 99; //Static files should be registered before routing & custom middlewares
    }
}