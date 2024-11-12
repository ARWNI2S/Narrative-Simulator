using ARWNI2S.Infrastructure;

namespace ARWNI2S.Portal.Framework.Infrastructure.Extensions
{
    public static class WebEngineExtensions
    {
        /// <summary>
        /// Configure HTTP request pipeline
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void ConfigureRequestPipeline(this INodeEngine engine, IApplicationBuilder application)
        {
            if (engine is WebNodeEngine webNodeEngine)
                webNodeEngine.ConfigureRequestPipeline(application);
        }
    }
}
