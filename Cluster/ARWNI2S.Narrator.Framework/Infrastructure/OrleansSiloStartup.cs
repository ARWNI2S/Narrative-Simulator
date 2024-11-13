using ARWNI2S.Infrastructure;
using ARWNI2S.Infrastructure.Engine.Builder;
using ARWNI2S.Node.Configuration.Options.Extensions;
using ARWNI2S.Node.Core;
using ARWNI2S.Node.Core.Configuration;
using ARWNI2S.Node.Core.Infrastructure;
using Azure.Data.Tables;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;
using StackExchange.Redis;

namespace ARWNI2S.Narrator.Framework.Infrastructure
{
    public class OrleansSiloStartup : INI2SStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //narrator node
            var ni2sSettings = Singleton<NI2SSettings>.Instance;
            var clusterConfig = ni2sSettings.Get<ClusterConfig>();
            var nodeConfig = ni2sSettings.Get<NodeConfig>();

            services.AddOrleans(siloBuilder =>
            {
                siloBuilder = siloBuilder.Configure<SiloOptions>(options =>
                {
                    options.SiloName = nodeConfig.NodeName;
                }).Configure<ClusterOptions>(options =>
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
                                if (string.IsNullOrEmpty(clusterConfig.ConnectionString))
                                    throw new NodeException("Unable to configure Azure storage clustering: missing connection string.");

                                // Configurar el cluster Orleans
                                siloBuilder = siloBuilder.UseAzureStorageClustering(options => options.TableServiceClient = new TableServiceClient(clusterConfig.ConnectionString, options.ClientOptions));
                                break;
                            }
                        case SimulationClusteringType.Redis:
                            {
                                if (string.IsNullOrEmpty(clusterConfig.ConnectionString))
                                    throw new NodeException("Unable to configure Redis storage clustering: missing connection string.");

                                siloBuilder = siloBuilder.UseRedisClustering(options =>
                                {
                                    // Configura los detalles de conexión a Redis
                                    options.ConfigurationOptions = new ConfigurationOptions().ParseConnectionString(clusterConfig.ConnectionString);
                                });
                                break;
                            }
                        case SimulationClusteringType.SqlServer:
                            {
                                if (string.IsNullOrEmpty(clusterConfig.ConnectionString))
                                    throw new NodeException("Unable to configure SqlServer storage clustering: missing connection string.");
                                siloBuilder = siloBuilder.UseAdoNetClustering(options => {
                                    options.Invariant = Constants.INVARIANT_NAME_SQL_SERVER;
                                    options.ConnectionString = clusterConfig.ConnectionString;
                                });
                                break;
                            }
                        case SimulationClusteringType.Localhost:
                        default:
                            {
                                siloBuilder.UseLocalhostClustering();
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

            //services.AddSingleton<ILifecycleParticipant<ISiloLifecycle>, SiloStartupNotifier>();


        }

        public void Configure(IEngineBuilder engine)
        {
        }

        public int Order => 149;    // Puedes ajustar el orden según sea necesario
    }
}
