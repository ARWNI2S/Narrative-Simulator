using ARWNI2S.Engine.Hosting.Extensions;
using ARWNI2S.Infrastructure;
using ARWNI2S.Infrastructure.Engine.Builder;
using ARWNI2S.Node.Hosting.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ARWNI2S.Node.Infrastructure
{
    public class ClusterStartup : INI2SStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //services.AddNI2SNodeServerServices();

            services.AddClusteringServices();

            services.AddNI2SRuntimeServices();
        }

        public void Configure(IEngineBuilder engine)
        {
            engine.UseClustering();
        }

        public int Order => 100;     // clustering services should be loaded after error handlers
    }
}
