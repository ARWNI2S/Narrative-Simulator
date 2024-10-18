using ARWNI2S.Infrastructure;
using ARWNI2S.Node.Core.Configuration;
using ARWNI2S.Node.Core.Infrastructure;
using Azure.Data.Tables;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;

namespace ARWNI2S.Narrator.Framework.Infrastructure
{
    public class OrleansSiloStartup : INI2SStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //narrator node
            var ni2sSettings = Singleton<NI2SSettings>.Instance;
            var clusterConfig = ni2sSettings.Get<ClusterConfig>();
            //var siloConfig = ni2sSettings.Get<OrleansSiloConfig>();

            services.AddOrleans(siloBuilder =>
            {
                siloBuilder = siloBuilder.Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = clusterConfig.ClusterId;
                    options.ServiceId = clusterConfig.ServiceId;
                });

                if (!clusterConfig.IsDevelopment)
                {
                    switch (clusterConfig.SiloStorageClustering)
                    {
                        case SimulationClusteringType.AzureStorage:
                            {
                                // Configurar el silo Orleans
                                siloBuilder = siloBuilder.UseAzureStorageClustering(options =>
                                                options.TableServiceClient = new TableServiceClient(configuration["ORLEANS_AZURE_STORAGE_CONNECTION_STRING"],
                                                options.ClientOptions));
                                break;
                            }
                        case SimulationClusteringType.Localhost:
                        default:
                            {
                                // Configurar el silo Orleans
                                siloBuilder = siloBuilder.UseLocalhostClustering();// narratorNodeConfig.SiloPort, narratorNodeConfig.GatewayPort, narratorNodeConfig.PrimarySiloEndpoint, serviceId, clusterId);
                                break;
                            }
                    }
                }
                else
                {
                    // Configurar el silo Orleans
                    siloBuilder = siloBuilder.UseLocalhostClustering();// narratorNodeConfig.SiloPort, narratorNodeConfig.GatewayPort, narratorNodeConfig.PrimarySiloEndpoint, serviceId, clusterId);
                }

                siloBuilder.AddMemoryGrainStorage("Default"); // Almacenamiento en memoria para grains
            });
        }

        public void Configure(IHost application)
        {
            // Orleans normalmente no necesita configuraciones adicionales aquí
        }

        public int Order => 149;    // Puedes ajustar el orden según sea necesario
    }
}
