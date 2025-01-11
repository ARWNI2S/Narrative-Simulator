using ARWNI2S.Cluster.Configuration;
using ARWNI2S.Configuration;
using ARWNI2S.Engine.Builder;
using ARWNI2S.Extensibility;
using ARWNI2S.Lifecycle;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans.Hosting;

namespace ARWNI2S.Runtime.Narrator
{
    internal class ConfigureOrleansSilo : IConfigureEngine
    {
        public int Order => NI2SLifecycleStage.CoreInitialize;

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            NodeConfig nodeConfig = Singleton<NI2SSettings>.Instance.Get<NodeConfig>();
            ClusterConfig clusterConfig = Singleton<NI2SSettings>.Instance.Get<ClusterConfig>();

            services.AddOrleans(siloBuilder =>
            {
                siloBuilder.UseLocalhostClustering();
            });
        }

        public void ConfigureEngine(IEngineBuilder engineBuilder)
        {
        }
    }
}
