using ARWNI2S.Engine.Hosting.Extensions;
using ARWNI2S.Engine.Services.Clustering;
using ARWNI2S.Infrastructure;
using ARWNI2S.Infrastructure.Configuration;
using ARWNI2S.Node.Builder;
using ARWNI2S.Node.Configuration.Options.Extensions;
using ARWNI2S.Node.Core;
using ARWNI2S.Node.Core.Caching;
using ARWNI2S.Node.Core.Configuration;
using ARWNI2S.Node.Core.Infrastructure;
using ARWNI2S.Node.Core.Network;
using ARWNI2S.Node.Hosting.Extensions;
using ARWNI2S.Node.Services.Clustering;
using ARWNI2S.Node.Services.Network;
using Azure.Data.Tables;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;
using StackExchange.Redis;
using System.Net;

namespace ARWNI2S.Narrator.Framework.Hosting.Extensions
{
    /// <summary>
    /// Represents extensions of IServiceCollection
    /// </summary>
    public static class EngineBuilderServiceCollectionExtensions
    {
        ///// <summary>
        ///// Configure base enginelication settings
        ///// </summary>
        ///// <param name="services">Collection of service descriptors</param>
        ///// <param name="builder">A builder for enginelications and services</param>
        //public static void ConfigureEngineSettings(this IServiceCollection services,
        //    NodeEngineBuilder builder)
        //{
        //    //let the operating system decide what TLS protocol version to use
        //    //see https://docs.microsoft.com/dotnet/framework/network-programming/tls
        //    ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault;

        //    //create default file provider
        //    CommonHelper.DefaultFileProvider = new EngineFileProvider(builder.Environment);

        //    //register type finder
        //    var typeFinder = new NodeEngineTypeFinder();
        //    Singleton<ITypeFinder>.Instance = typeFinder;
        //    services.AddSingleton<ITypeFinder>(typeFinder);

        //    //add configuration parameters
        //    var configurations = typeFinder
        //        .FindClassesOfType<IConfig>()
        //        .Select(configType => (IConfig)Activator.CreateInstance(configType))
        //        .ToList();

        //    foreach (var config in configurations)
        //        builder.Configuration.GetSection(config.Name).Bind(config, options => options.BindNonPublicProperties = true);

        //    //TODO: READ SECRETS FROM KEYVAULT use SecretAttribute decorated properties

        //    var ni2sSettings = NI2SSettingsHelper.SaveNI2SSettings(configurations, CommonHelper.DefaultFileProvider, false);
        //    services.AddSingleton(ni2sSettings);
        //}

        ///// <summary>
        ///// Add services to the enginelication and configure service provider
        ///// </summary>
        ///// <param name="services">Collection of service descriptors</param>
        ///// <param name="builder">A builder for enginelications and services</param>
        //public static void ConfigureEngineServices(this IServiceCollection services,
        //    NodeEngineBuilder builder)
        //{
        //    //add accessor to Context
        //    services.AddContextAccessor();

        //    //add core services
        //    var niisCoreBuilder = services.AddNI2SCore();

        //    //initialize modules
        //    var moduleConfig = new ModuleConfig();
        //    builder.Configuration.GetSection(nameof(ModuleConfig)).Bind(moduleConfig, options => options.BindNonPublicProperties = true);
        //    niisCoreBuilder.PartManager.InitializeModules(moduleConfig);

        //    //create engine and configure service provider
        //    var engine = NodeEngineContext.Create();

        //    engine.ConfigureServices(services, builder.Configuration);
        //}

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

        ///// <summary>
        ///// Adds frontline (orleans client) services
        ///// </summary>
        ///// <param name="services">Collection of service descriptors</param>
        //public static void AddNI2SRuntimeServices(this IServiceCollection services)
        //{
        //    var ni2sSettings = Singleton<NI2SSettings>.Instance;
        //    var nodeConfig = ni2sSettings.Get<NodeConfig>();

        //    // notice: Orleans Silo Nodes will throw exception if UseFrontline enabled.
        //    // frontline services enables orleans client access, disable if no realtime simulation data is needed.
        //    if (nodeConfig.NodeType == NodeType.Frontline)
        //    {
        //        var clusterConfig = ni2sSettings.Get<ClusterConfig>();

        //        services.AddOrleansClient(client =>
        //        {
        //            client = client.Configure<ClusterOptions>(options =>
        //            {
        //                options.ClusterId = clusterConfig.ClusterId;
        //                options.ServiceId = clusterConfig.ServiceId;
        //            }).Configure<ClientMessagingOptions>(options =>
        //            {
        //                // Configurar el tiempo de espera para respuestas
        //                options.ResponseTimeout = TimeSpan.FromSeconds(60); // Timeout para mensajes
        //                options.ResponseTimeoutWithDebugger = TimeSpan.FromSeconds(180); // Timeout extendido si se está depurando
        //            });


        //            if (!clusterConfig.IsDevelopment)
        //            {
        //                switch (clusterConfig.SiloStorageClustering)
        //                {
        //                    case SimulationClusteringType.AzureStorage:
        //                        {
        //                            if (string.IsNullOrEmpty(clusterConfig.ConnectionString))
        //                                throw new NodeException("Unable to configure Azure storage clustering: missing connection string.");

        //                            // Configurar el cluster Orleans
        //                            client = client.UseAzureStorageClustering(options => options.TableServiceClient = new TableServiceClient(clusterConfig.ConnectionString, options.ClientOptions));
        //                            break;
        //                        }
        //                    case SimulationClusteringType.Redis:
        //                        {
        //                            if (string.IsNullOrEmpty(clusterConfig.ConnectionString))
        //                                throw new NodeException("Unable to configure Redis storage clustering: missing connection string.");

        //                            client = client.UseRedisClustering(options =>
        //                            {
        //                                // Configura los detalles de conexión a Redis
        //                                options.ConfigurationOptions = new ConfigurationOptions().ParseConnectionString(clusterConfig.ConnectionString);
        //                            });
        //                            break;
        //                        }
        //                    case SimulationClusteringType.SqlServer:
        //                        {
        //                            if (string.IsNullOrEmpty(clusterConfig.ConnectionString))
        //                                throw new NodeException("Unable to configure SqlServer storage clustering: missing connection string.");
        //                            client = client.UseAdoNetClustering(options =>
        //                            {
        //                                options.Invariant = Constants.INVARIANT_NAME_SQL_SERVER;
        //                                options.ConnectionString = clusterConfig.ConnectionString;
        //                            });
        //                            break;
        //                        }
        //                    case SimulationClusteringType.Localhost:
        //                    default:
        //                        {
        //                            client.UseLocalhostClustering();
        //                            break;
        //                        }
        //                }

        //            }
        //            else
        //            {
        //                client.UseLocalhostClustering();
        //            }
        //        });
        //    }



        //}


        ///// <summary>
        ///// Register RuntimeContextAccessor
        ///// </summary>
        ///// <param name="_">Collection of service descriptors</param>
        //public static void AddContextAccessor(this IServiceCollection _)
        //{
        //    //services.AddSingleton<IFrameContextAccessor, RuntimeContextAccessor>();
        //    //services.AddSingleton<IPackageHandlingContextAccessor<NI2SProtoPacket>, RuntimeContextAccessor>();
        //}

        ///// <summary>
        ///// Adds services required for enginelication session state
        ///// </summary>
        ///// <param name="services">Collection of service descriptors</param>
        //public static void AddNI2SSession(this IServiceCollection services)
        //{
        //    //services.AddSingleton<ISessionFactory, GenericSessionFactory<>>();

        //    //services.AddSession(options =>
        //    //{
        //    //    options.Cookie.Name = $"{CookieDefaults.Prefix}{CookieDefaults.SessionCookie}";
        //    //    options.Cookie.HttpOnly = true;
        //    //    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        //    //});
        //}




        ///// <summary>
        ///// Adds services required for distributed cache
        ///// </summary>
        ///// <param name="services">Collection of service descriptors</param>
        //public static void AddDistributedCache(this IServiceCollection services)
        //{
        //    var ni2sSettings = Singleton<NI2SSettings>.Instance;
        //    var distributedCacheConfig = ni2sSettings.Get<DistributedCacheConfig>();

        //    if (!distributedCacheConfig.Enabled)
        //        return;

        //    switch (distributedCacheConfig.DistributedCacheType)
        //    {
        //        case DistributedCacheType.Memory:
        //            services.AddDistributedMemoryCache();
        //            break;

        //        case DistributedCacheType.SqlServer:
        //            services.AddDistributedSqlServerCache(options =>
        //            {
        //                options.ConnectionString = distributedCacheConfig.ConnectionString;
        //                options.SchemaName = distributedCacheConfig.SchemaName;
        //                options.TableName = distributedCacheConfig.TableName;
        //            });
        //            break;

        //        case DistributedCacheType.Redis:
        //        case DistributedCacheType.RedisSynchronizedMemory:
        //            services.AddStackExchangeRedisCache(options =>
        //            {
        //                options.Configuration = distributedCacheConfig.ConnectionString;
        //                options.InstanceName = distributedCacheConfig.InstanceName ?? string.Empty;
        //            });
        //            break;
        //    }
        //}

        ///// <summary>
        ///// Add and configure default HTTP clients
        ///// </summary>
        ///// <param name="services">Collection of service descriptors</param>
        //public static void AddNI2SHttpClients(this IServiceCollection services)
        //{
        //    //default client
        //    services.AddHttpClient(HttpDefaults.DefaultHttpClient).WithProxy();

        //    //client to request current node
        //    services.AddHttpClient<NodeHttpClient>();

        //    ////client to request dragonCorp official site
        //    //services.AddHttpClient<DraCoHttpClient>().WithProxy();

        //    ////client to request reCAPTCHA service
        //    //services.AddHttpClient<CaptchaHttpClient>().WithProxy();
        //}
    }
}