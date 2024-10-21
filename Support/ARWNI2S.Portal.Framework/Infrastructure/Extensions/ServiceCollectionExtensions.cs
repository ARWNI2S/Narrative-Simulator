using ARWNI2S.Infrastructure;
using ARWNI2S.Infrastructure.Configuration;
using ARWNI2S.Infrastructure.Engine;
using ARWNI2S.Node.Core.Caching;
using ARWNI2S.Node.Core.Configuration;
using ARWNI2S.Node.Core.Infrastructure;
using ARWNI2S.Node.Core.Network;
using ARWNI2S.Node.Core.Security;
using ARWNI2S.Node.Data;
using ARWNI2S.Node.Services.Network;
using ARWNI2S.Node.Services.Security;
using ARWNI2S.Portal.Framework.Profiling;
using ARWNI2S.Portal.Framework.Security.Captcha;
using ARWNI2S.Portal.Framework.WebOptimizer;
using ARWNI2S.Portal.Services.Authentication;
using ARWNI2S.Portal.Services.Authentication.External;
using ARWNI2S.Portal.Services.Common;
using ARWNI2S.Portal.Services.Configuration;
using ARWNI2S.Portal.Services.Entities.Common;
using ARWNI2S.Portal.Services.Http;
using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using StackExchange.Profiling;
using StackExchange.Profiling.Internal;
using StackExchange.Profiling.Storage;
using System.Net;
using WebMarkupMin.AspNetCore8;
using WebMarkupMin.Core;
using WebMarkupMin.NUglify;

namespace ARWNI2S.Portal.Framework.Infrastructure.Extensions
{
    /// <summary>
    /// Represents extensions of IServiceCollection
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configure base application settings
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="builder">A builder for web applications and services</param>
        public static void ConfigureApplicationSettings(this IServiceCollection services,
            WebApplicationBuilder builder)
        {
            //let the operating system decide what TLS protocol version to use
            //see https://docs.microsoft.com/dotnet/framework/network-programming/tls
            ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault;

            //create default file provider
            CommonHelper.DefaultFileProvider = new EngineFileProvider(builder.Environment);

            //register type finder
            var typeFinder = new WebAppTypeFinder();
            Singleton<ITypeFinder>.Instance = typeFinder;
            services.AddSingleton<ITypeFinder>(typeFinder);

            //add configuration parameters
            var configurations = typeFinder
                .FindClassesOfType<IConfig>()
                .Select(configType => (IConfig)Activator.CreateInstance(configType))
                .ToList();

            foreach (var config in configurations)
                builder.Configuration.GetSection(config.Name).Bind(config, options => options.BindNonPublicProperties = true);

            //TODO: READ SECRETS FROM KEYVAULT use SecretAttribute decorated properties

            var ni2sSettings = NI2SSettingsHelper.SaveNI2SSettings(configurations, CommonHelper.DefaultFileProvider, false);
            services.AddSingleton(ni2sSettings);
            //services.AddSingleton(ni2sSettings as );
        }

        /// <summary>
        /// Add services to the application and configure service provider
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="builder">A builder for web applications and services</param>
        public static void ConfigureApplicationServices(this IServiceCollection services,
            WebApplicationBuilder builder)
        {
            //add accessor to HttpContext
            services.AddContextAccessor();

            //initialize modules
            var mvcCoreBuilder = services.AddMvcCore();
            var moduleConfig = new ModuleConfig();
            builder.Configuration.GetSection(nameof(ModuleConfig)).Bind(moduleConfig, options => options.BindNonPublicProperties = true);
            mvcCoreBuilder.PartManager.InitializeModules(moduleConfig);

            //create engine and configure service provider
            Singleton<IEngine>.Instance = new WebNodeEngine();
            var engine = NodeEngineContext.Create();

            engine.ConfigureServices(services, builder.Configuration);
        }

        /// <summary>
        /// Register HttpContextAccessor
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddContextAccessor(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IEngineContextAccessor, PortalContextAccessor>();
        }

        /// <summary>
        /// Adds services required for anti-forgery support
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddAntiForgery(this IServiceCollection services)
        {
            //override cookie name
            services.AddAntiforgery(options =>
            {
                options.Cookie.Name = $"{CookieDefaults.Prefix}{CookieDefaults.AntiforgeryCookie}";
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });
        }

        /// <summary>
        /// Adds services required for application session state
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddHttpSession(this IServiceCollection services)
        {
            services.AddSession(options =>
            {
                options.Cookie.Name = $"{CookieDefaults.Prefix}{CookieDefaults.SessionCookie}";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });
        }

        /// <summary>
        /// Adds services required for distributed cache
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddDistributedCache(this IServiceCollection services)
        {
            var ni2sSettings = Singleton<NI2SSettings>.Instance;
            var distributedCacheConfig = ni2sSettings.Get<DistributedCacheConfig>();

            if (!distributedCacheConfig.Enabled)
                return;

            switch (distributedCacheConfig.DistributedCacheType)
            {
                case DistributedCacheType.Memory:
                    services.AddDistributedMemoryCache();
                    break;

                case DistributedCacheType.SqlServer:
                    services.AddDistributedSqlServerCache(options =>
                    {
                        options.ConnectionString = distributedCacheConfig.ConnectionString;
                        options.SchemaName = distributedCacheConfig.SchemaName;
                        options.TableName = distributedCacheConfig.TableName;
                    });
                    break;

                case DistributedCacheType.Redis:
                    services.AddStackExchangeRedisCache(options =>
                    {
                        options.Configuration = distributedCacheConfig.ConnectionString;
                        options.InstanceName = distributedCacheConfig.InstanceName ?? string.Empty;
                    });
                    break;

                case DistributedCacheType.RedisSynchronizedMemory:
                    services.AddStackExchangeRedisCache(options =>
                    {
                        options.Configuration = distributedCacheConfig.ConnectionString;
                        options.InstanceName = distributedCacheConfig.InstanceName ?? string.Empty;
                    });
                    break;
            }
        }

        /// <summary>
        /// Adds data protection services
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddNI2SDataProtection(this IServiceCollection services)
        {
            var ni2sSettings = Singleton<NI2SSettings>.Instance;
            if (ni2sSettings.Get<AzureBlobConfig>().Enabled && ni2sSettings.Get<AzureBlobConfig>().StoreDataProtectionKeys)
            {
                var blobServiceClient = new BlobServiceClient(ni2sSettings.Get<AzureBlobConfig>().ConnectionString);
                var blobContainerClient = blobServiceClient.GetBlobContainerClient(ni2sSettings.Get<AzureBlobConfig>().DataProtectionKeysContainerName);
                var blobClient = blobContainerClient.GetBlobClient(DataProtectionDefaults.AzureDataProtectionKeyFile);

                var dataProtectionBuilder = services.AddDataProtection().PersistKeysToAzureBlobStorage(blobClient);

                if (!ni2sSettings.Get<AzureBlobConfig>().DataProtectionKeysEncryptWithVault)
                    return;

                var keyIdentifier = ni2sSettings.Get<AzureBlobConfig>().DataProtectionKeysVaultId;
                var credentialOptions = new DefaultAzureCredentialOptions();
                var tokenCredential = new DefaultAzureCredential(credentialOptions);

                dataProtectionBuilder.ProtectKeysWithAzureKeyVault(new Uri(keyIdentifier), tokenCredential);
            }
            else
            {
                var dataProtectionKeysPath = CommonHelper.DefaultFileProvider.MapPath(DataProtectionDefaults.DataProtectionKeysPath);
                var dataProtectionKeysFolder = new DirectoryInfo(dataProtectionKeysPath);

                //configure the data protection system to persist keys to the specified directory
                services.AddDataProtection().PersistKeysToFileSystem(dataProtectionKeysFolder);
            }
        }

        /// <summary>
        /// Adds authentication service
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddNI2SAuthentication(this IServiceCollection services)
        {
            //set default authentication schemes
            var authenticationBuilder = services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = AuthenticationServicesDefaults.AuthenticationScheme;
                options.DefaultScheme = AuthenticationServicesDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = AuthenticationServicesDefaults.ExternalAuthenticationScheme;
            });

            //add main cookie authentication
            authenticationBuilder.AddCookie(AuthenticationServicesDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.Name = $"{CookieDefaults.Prefix}{CookieDefaults.AuthenticationCookie}";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.LoginPath = AuthenticationServicesDefaults.LoginPath;
                options.AccessDeniedPath = AuthenticationServicesDefaults.AccessDeniedPath;
            });

            //add external authentication
            authenticationBuilder.AddCookie(AuthenticationServicesDefaults.ExternalAuthenticationScheme, options =>
            {
                options.Cookie.Name = $"{CookieDefaults.Prefix}{CookieDefaults.ExternalAuthenticationCookie}";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.LoginPath = AuthenticationServicesDefaults.LoginPath;
                options.AccessDeniedPath = AuthenticationServicesDefaults.AccessDeniedPath;
            });

            //add wallet authentication
            //authenticationBuilder.AddCookie(AuthenticationServicesDefaults.WalletAuthenticationScheme, options =>
            //{
            //    options.Cookie.Name = $"{CookieDefaults.Prefix}{CookieDefaults.WalletAuthenticationCookie}";
            //    options.Cookie.HttpOnly = true;
            //    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            //    options.LoginPath = AuthenticationServicesDefaults.LoginPath;
            //    options.AccessDeniedPath = AuthenticationServicesDefaults.AccessDeniedPath;
            //});

            //register and configure external authentication modules now
            var typeFinder = Singleton<ITypeFinder>.Instance;
            var externalAuthConfigurations = typeFinder.FindClassesOfType<IExternalAuthenticationRegistrar>();
            var externalAuthInstances = externalAuthConfigurations
                .Select(x => (IExternalAuthenticationRegistrar)Activator.CreateInstance(x));

            foreach (var instance in externalAuthInstances)
                instance.Configure(authenticationBuilder);

            //var walletAuthConfigurations = typeFinder.FindClassesOfType<IWalletAuthenticationRegistrar>();
            //var walletAuthInstances = walletAuthConfigurations
            //    .Select(x => (IWalletAuthenticationRegistrar)Activator.CreateInstance(x));

            //foreach (var instance in walletAuthInstances)
            //    instance.Configure(authenticationBuilder);

        }

        /// <summary>
        /// Add and configure Blazor for the application
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <returns>A builder for configuring Blazor services</returns>
        public static IServiceCollection AddNI2SBlazor(this IServiceCollection services)
        {
            services.AddRazorComponents();  // Configuración básica para Blazor Server

            services.AddServerSideBlazor();  // Configuración adicional para Blazor Server


            var ni2sSettings = Singleton<NI2SSettings>.Instance;
            if (ni2sSettings.Get<PortalConfig>().UseSessionStateTempDataProvider)
            {
                //use session-based temp data provider
                services.AddSession();
            }
            else
            {
                //use cookie-based temp data provider
                //mvcBuilder.AddCookieTempDataProvider(options =>
                //{
                //    options.Cookie.Name = $"{CookieDefaults.Prefix}{CookieDefaults.TempDataCookie}";
                //    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                //});
            }

            services.AddRazorPages();

            //// Serialización JSON si necesitas manipular datos
            //services.AddControllers().AddNewtonsoftJson(options =>
            //    options.SerializerSettings.ContractResolver = new DefaultContractResolver()
            //);

            ////MVC now serializes JSON with camel case names by default, use this code to avoid it
            //mvcBuilder.AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            ////set some options
            //mvcBuilder.AddMvcOptions(options =>
            //{
            //    //we'll use this until https://github.com/dotnet/aspnetcore/issues/6566 is solved 
            //    options.ModelBinderProviders.Insert(0, new InvariantNumberModelBinderProvider());
            //    options.ModelBinderProviders.Insert(1, new CustomPropertiesModelBinderProvider());
            //    //add custom display metadata provider 
            //    options.ModelMetadataDetailsProviders.Add(new NI2SMetadataProvider());

            //    //in .NET model binding for a non-nullable property may fail with an error message "The value '' is invalid"
            //    //here we set the locale name as the message, we'll replace it with the actual one later when not-null validation failed
            //    options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(_ => NI2SValidationDefaults.NotNullValidationLocaleName);
            //});

            ////add fluent validation
            //services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();

            ////register all available validators from DragonCorp assemblies
            //var assemblies = mvcBuilder.PartManager.ApplicationParts
            //    .OfType<AssemblyPart>()
            //    .Where(part => part.Name.StartsWith("DragonCorp", StringComparison.InvariantCultureIgnoreCase))
            //    .Select(part => part.Assembly);
            //services.AddValidatorsFromAssemblies(assemblies);

            ////register controllers as services, it'll allow to override them
            //mvcBuilder.AddControllersAsServices();

            return services;
        }

        ///// <summary>
        ///// Add and configure MVC for the application
        ///// </summary>
        ///// <param name="services">Collection of service descriptors</param>
        ///// <returns>A builder for configuring MVC services</returns>
        //public static IMvcBuilder AddNI2SMvc(this IServiceCollection services)
        //{
        //    //add basic MVC feature
        //    var mvcBuilder = services.AddControllersWithViews();

        //    mvcBuilder.AddRazorRuntimeCompilation();

        //    var ni2sSettings = Singleton<NI2SSettings>.Instance;
        //    if (ni2sSettings.Get<PortalConfig>().UseSessionStateTempDataProvider)
        //    {
        //        //use session-based temp data provider
        //        mvcBuilder.AddSessionStateTempDataProvider();
        //    }
        //    else
        //    {
        //        //use cookie-based temp data provider
        //        mvcBuilder.AddCookieTempDataProvider(options =>
        //        {
        //            options.Cookie.Name = $"{CookieDefaults.Prefix}{CookieDefaults.TempDataCookie}";
        //            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        //        });
        //    }

        //    services.AddRazorPages();

        //    //MVC now serializes JSON with camel case names by default, use this code to avoid it
        //    mvcBuilder.AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

        //    //set some options
        //    mvcBuilder.AddMvcOptions(options =>
        //    {
        //        //we'll use this until https://github.com/dotnet/aspnetcore/issues/6566 is solved 
        //        options.ModelBinderProviders.Insert(0, new InvariantNumberModelBinderProvider());
        //        options.ModelBinderProviders.Insert(1, new CustomPropertiesModelBinderProvider());
        //        //add custom display metadata provider 
        //        options.ModelMetadataDetailsProviders.Add(new NI2SMetadataProvider());

        //        //in .NET model binding for a non-nullable property may fail with an error message "The value '' is invalid"
        //        //here we set the locale name as the message, we'll replace it with the actual one later when not-null validation failed
        //        options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(_ => NI2SValidationDefaults.NotNullValidationLocaleName);
        //    });

        //    //add fluent validation
        //    services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();

        //    //register all available validators from DragonCorp assemblies
        //    var assemblies = mvcBuilder.PartManager.ApplicationParts
        //        .OfType<AssemblyPart>()
        //        .Where(part => part.Name.StartsWith("DragonCorp", StringComparison.InvariantCultureIgnoreCase))
        //        .Select(part => part.Assembly);
        //    services.AddValidatorsFromAssemblies(assemblies);

        //    //register controllers as services, it'll allow to override them
        //    mvcBuilder.AddControllersAsServices();

        //    return mvcBuilder;
        //}

        ///// <summary>
        ///// Register custom RedirectResultExecutor
        ///// </summary>
        ///// <param name="services">Collection of service descriptors</param>
        //public static void AddNI2SRedirectResultExecutor(this IServiceCollection services)
        //{
        //    //we use custom redirect executor as a workaround to allow using non-ASCII characters in redirect URLs
        //    services.AddScoped<IActionResultExecutor<RedirectResult>, MetalinkRedirectResultExecutor>();
        //}

        /// <summary>
        /// Add and configure MiniProfiler service
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddNI2SMiniProfiler(this IServiceCollection services)
        {
            //whether database is already installed
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            var ni2sSettings = Singleton<NI2SSettings>.Instance;
            if (ni2sSettings.Get<CommonConfig>().MiniProfilerEnabled)
            {
                services.AddMiniProfiler(miniProfilerOptions =>
                {
                    //use memory cache provider for storing each result
                    ((MemoryCacheStorage)miniProfilerOptions.Storage).CacheDuration = TimeSpan.FromMinutes(ni2sSettings.Get<CacheConfig>().DefaultCacheTime);

                    //determine who can access the MiniProfiler results
                    miniProfilerOptions.ResultsAuthorize = request => NodeEngineContext.Current.Resolve<IPermissionService>().AuthorizeAsync(StandardPermissionProvider.AccessProfiling).Result;
                });
            }
        }


        /// <summary>
        /// Adds MiniProfiler timings for actions and views.
        /// </summary>
        /// <param name="services">The services collection to configure.</param>
        /// <param name="configureOptions">An <see cref="Action{MiniProfilerOptions}"/> to configure options for MiniProfiler.</param>
        private static IMiniProfilerBuilder AddMiniProfiler(this IServiceCollection services, Action<MiniProfilerOptions> configureOptions = null)
        {
            services.AddMemoryCache(); // Unconditionally register an IMemoryCache since it's the most common and default case
            services.AddSingleton<IConfigureOptions<MiniProfilerOptions>, MiniProfilerOptionsDefaults>();
            if (configureOptions != null)
            {
                services.Configure(configureOptions);
            }
            // Set background statics
            services.Configure<MiniProfilerOptions>(o => MiniProfiler.Configure(o));
            services.AddSingleton<DiagnosticInitializer>(); // For any IMiniProfilerDiagnosticListener registration

            services.AddSingleton<IMiniProfilerDiagnosticListener, MiniProfilerDiagnosticListener>(); // For view and action profiling

            return new MiniProfilerBuilder(services);
        }

        /// <summary>
        /// Add and configure WebMarkupMin service
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddNI2SWebMarkupMin(this IServiceCollection services)
        {
            //check whether database is installed
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            services
                .AddWebMarkupMin(options =>
                {
                    options.AllowMinificationInDevelopmentEnvironment = true;
                    options.AllowCompressionInDevelopmentEnvironment = true;
                    options.DisableMinification = !NodeEngineContext.Current.Resolve<CommonSettings>().EnableHtmlMinification;
                    options.DisableCompression = true;
                    options.DisablePoweredByHttpHeaders = true;
                })
                .AddHtmlMinification(options =>
                {
                    options.MinificationSettings.AttributeQuotesRemovalMode = HtmlAttributeQuotesRemovalMode.KeepQuotes;

                    options.CssMinifierFactory = new NUglifyCssMinifierFactory();
                    options.JsMinifierFactory = new NUglifyJsMinifierFactory();
                })
                .AddXmlMinification(options =>
                {
                    var settings = options.MinificationSettings;
                    settings.RenderEmptyTagsWithSpace = true;
                    settings.CollapseTagsWithoutContent = true;
                });
        }

        /// <summary>
        /// Adds WebOptimizer to the specified <see cref="IServiceCollection"/> and enables CSS and JavaScript minification.
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddNI2SWebOptimizer(this IServiceCollection services)
        {
            var ni2sSettings = Singleton<NI2SSettings>.Instance;
            var cssBundling = ni2sSettings.Get<WebOptimizerConfig>().EnableCssBundling;
            var jsBundling = ni2sSettings.Get<WebOptimizerConfig>().EnableJavaScriptBundling;

            //add minification & bundling
            var cssSettings = new CssBundlingSettings
            {
                FingerprintUrls = false,
                Minify = cssBundling
            };

            var codeSettings = new CodeBundlingSettings
            {
                Minify = jsBundling,
                AdjustRelativePaths = false //disable this feature because it breaks function names that have "Url(" at the end
            };

            services.AddWebOptimizer(null, cssSettings, codeSettings);
        }

        /// <summary>
        /// Add and configure default HTTP clients
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddNI2SHttpClients(this IServiceCollection services)
        {
            //default client
            services.AddHttpClient(HttpDefaults.DefaultHttpClient).WithProxy();

            //client to request current server
            services.AddHttpClient<NodeHttpClient>();

            //client to request dragonCorp official site
            services.AddHttpClient<DraCoHttpClient>().WithProxy();

            //client to request reCAPTCHA service
            services.AddHttpClient<CaptchaHttpClient>().WithProxy();
        }
    }
}