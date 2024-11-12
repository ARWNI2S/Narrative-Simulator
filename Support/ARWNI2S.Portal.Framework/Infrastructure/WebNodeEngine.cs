using ARWNI2S.Infrastructure;
using ARWNI2S.Node.Core.Infrastructure;

namespace ARWNI2S.Portal.Framework.Infrastructure
{
    /// <summary>
    /// Represents DragonCorp™ metalink backend engine
    /// </summary>
    public partial class WebNodeEngine : NodeEngine
    {
        #region Utilities

        /// <summary>
        /// Add and configure services
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //register engine
            services.AddSingleton<INodeEngine>(this);

            //find startup configurations provided by other assemblies
            var typeFinder = Singleton<ITypeFinder>.Instance;
            var startupConfigurations = typeFinder.FindClassesOfType<IWebStartup>();

            //create and sort instances of startup configurations
            var instances = startupConfigurations
                .Select(startup => (IWebStartup)Activator.CreateInstance(startup))
                .OrderBy(startup => startup.Order);

            //configure services
            foreach (var instance in instances)
                instance.ConfigureServices(services, configuration);

            services.AddSingleton(services);

            //register mapper configurations
            AddAutoMapper();

            //run startup tasks
            RunStartupTasks();

            //resolve assemblies here. otherwise, modules can throw an exception when rendering views
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        /// <summary>
        /// Configure HTTP request pipeline
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void ConfigureRequestPipeline(IApplicationBuilder application)
        {
            ServiceProvider = application.ApplicationServices;

            //find startup configurations provided by other assemblies
            var typeFinder = Singleton<ITypeFinder>.Instance;
            var startupConfigurations = typeFinder.FindClassesOfType<IWebStartup>();

            //create and sort instances of startup configurations
            var instances = startupConfigurations
                .Select(startup => (IWebStartup)Activator.CreateInstance(startup))
                .OrderBy(startup => startup.Order);

            //configure request pipeline
            foreach (var instance in instances)
                instance.Configure(application);

        }

        #endregion
    }
}
