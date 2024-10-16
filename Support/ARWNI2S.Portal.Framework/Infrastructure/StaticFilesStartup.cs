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
            services.AddNI2SWebOptimizer();
        }

        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
            //use response compression before UseNI2SStaticFiles to support compress for it
            application.UseNI2SResponseCompression();

            //WebOptimizer should be placed before configuring static files
            application.UseNI2SWebOptimizer();

            //use static files feature
            application.UseNI2SStaticFiles();
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => 99; //Static files should be registered before routing & custom middlewares
    }
}