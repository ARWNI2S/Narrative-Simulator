using ARWNI2S.Engine.Services.Clustering;
using ARWNI2S.Node.Services.Clustering;
using Microsoft.Extensions.DependencyInjection;

namespace ARWNI2S.Narrator.Framework.Hosting.Extensions
{
    /// <summary>
    /// Represents extensions of IServiceCollection
    /// </summary>
    public static class EngineBuilderServiceCollectionExtensions
    {
        public static void AddClusteringServices(this IServiceCollection services)
        {
            //var ni2sSettings = Singleton<NI2SSettings>.Instance;
            //var nodeConfig = ni2sSettings.Get<NodeConfig>();
            //var clusterConfig = ni2sSettings.Get<ClusterConfig>();

            //services.AddSingleton<INodeClientFactory, NodeClientFactory>();

            services.AddScoped<IClusteringService, ClusteringService>();
            services.AddScoped<INodeMappingService, NodeMappingService>();

            //services.AddSingleton<ClusterManager>();

            //services.AddHostedService<NodeHealthMonitorService>();

        }
    }
}