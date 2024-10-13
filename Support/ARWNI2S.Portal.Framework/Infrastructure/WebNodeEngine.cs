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
