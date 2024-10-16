using ARWNI2S.Infrastructure;
using ARWNI2S.Infrastructure.Configuration;
using ARWNI2S.Node.Core;
using ARWNI2S.Node.Core.Caching;
using ARWNI2S.Node.Core.Configuration;
using ARWNI2S.Node.Core.Events;
using ARWNI2S.Node.Core.Infrastructure;
using ARWNI2S.Node.Core.Services.Helpers;
using ARWNI2S.Node.Data;
using ARWNI2S.Node.Data.Entities.Clustering;
using ARWNI2S.Node.Services.Caching;
using ARWNI2S.Node.Services.Clustering;
using ARWNI2S.Node.Services.Common;
using ARWNI2S.Node.Services.Configuration;
using ARWNI2S.Node.Services.Directory;
using ARWNI2S.Node.Services.Events;
using ARWNI2S.Node.Services.Localization;
using ARWNI2S.Node.Services.Logging;
using ARWNI2S.Node.Services.Plugins;
using ARWNI2S.Node.Services.ScheduleTasks;
using ARWNI2S.Node.Services.Users;
using ARWNI2S.Portal.Framework.Menu;
using ARWNI2S.Portal.Framework.Routing;
using ARWNI2S.Portal.Services;
using ARWNI2S.Portal.Services.Authentication;
using ARWNI2S.Portal.Services.Authentication.External;
using ARWNI2S.Portal.Services.Authentication.MultiFactor;
using ARWNI2S.Portal.Services.Blogs;
using ARWNI2S.Portal.Services.Cms;
using ARWNI2S.Portal.Services.Common;
using ARWNI2S.Portal.Services.ExportImport;
using ARWNI2S.Portal.Services.Gdpr;
using ARWNI2S.Portal.Services.Globalization;
using ARWNI2S.Portal.Services.Helpers;
using ARWNI2S.Portal.Services.Html;
using ARWNI2S.Portal.Services.Installation;
using ARWNI2S.Portal.Services.Mailing;
using ARWNI2S.Portal.Services.Media;
using ARWNI2S.Portal.Services.Media.RoxyFileman;
using ARWNI2S.Portal.Services.Messages;
using ARWNI2S.Portal.Services.News;
using ARWNI2S.Portal.Services.Security;
using ARWNI2S.Portal.Services.Seo;
using ARWNI2S.Portal.Services.Topics;
using ARWNI2S.Portal.Services.Users;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using TaskScheduler = ARWNI2S.Node.Services.ScheduleTasks.TaskScheduler;
using UserService = ARWNI2S.Portal.Services.Users.UserService;

namespace ARWNI2S.Portal.Framework.Infrastructure
{
    /// <summary>
    /// Represents the registering services on application startup
    /// </summary>
    public partial class PortalStartup : IWebStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>
        public virtual void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //file provider
            services.AddScoped<IEngineFileProvider, EngineFileProvider>();

            //web helper
            services.AddScoped<IWebHelper, NodeWebHelper>();

            //user agent helper
            services.AddScoped<IUserAgentHelper, UserAgentHelper>();

            //modules
            services.AddScoped<IModuleService, ModuleService>();
            //services.AddScoped<OfficialFeedManager>();

            //static cache manager
            var ni2sSettings = Singleton<NI2SSettings>.Instance;
            var distributedCacheConfig = ni2sSettings.Get<DistributedCacheConfig>();

            services.AddSingleton<ICacheKeyManager, CacheKeyManager>();

            if (distributedCacheConfig.Enabled)
            {
                switch (distributedCacheConfig.DistributedCacheType)
                {
                    case DistributedCacheType.Memory:
                        services.AddScoped<IStaticCacheManager, MemoryDistributedCacheManager>();
                        break;
                    case DistributedCacheType.SqlServer:
                        services.AddScoped<IStaticCacheManager, MsSqlServerCacheManager>();
                        break;
                    case DistributedCacheType.Redis:
                        services.AddSingleton<IRedisConnectionWrapper, RedisConnectionWrapper>();
                        services.AddScoped<IStaticCacheManager, RedisCacheManager>();
                        break;
                    case DistributedCacheType.RedisSynchronizedMemory:
                        services.AddSingleton<IRedisConnectionWrapper, RedisConnectionWrapper>();
                        services.AddSingleton<ISynchronizedMemoryCache, RedisSynchronizedMemoryCache>();
                        services.AddSingleton<IStaticCacheManager, SynchronizedMemoryCacheManager>();
                        break;
                }

                services.AddSingleton<ILocker, DistributedCacheLocker>();
            }
            else
            {
                services.AddSingleton<ILocker, MemoryCacheLocker>();
                services.AddSingleton<IStaticCacheManager, MemoryCacheManager>();
            }

            //work context
            services.AddScoped<IWorkContext, PortalWorkContext>();

            //node context
            services.AddScoped<INodeContext, NI2SPortalContext>();

            //services
            services.AddScoped<ITopicTemplateService, TopicTemplateService>();
            services.AddScoped<IAddressAttributeFormatter, AddressAttributeFormatter>();
            services.AddScoped<IAddressAttributeParser, AddressAttributeParser>();
            services.AddScoped<IAddressAttributeService, AddressAttributeService>();
            services.AddScoped<IAddressService, AddressService>();
            //services.AddScoped<IAffiliateService, AffiliateService>();
            //services.AddScoped<IPartnerService, PartnerService>();
            //services.AddScoped<IGamesService, GamesService>();
            //services.AddScoped<IGamePublisherService, GamePublisherService>();
            //services.AddScoped<IPartnerAttributeFormatter, PartnerAttributeFormatter>();
            //services.AddScoped<IPartnerAttributeParser, PartnerAttributeParser>();
            //services.AddScoped<IPartnerAttributeService, PartnerAttributeService>();
            services.AddScoped<ISearchTermService, SearchTermService>();
            services.AddScoped<IGenericAttributeService, GenericAttributeService>();
            services.AddScoped<IMaintenanceService, MaintenanceService>();
            services.AddScoped<IUserAttributeFormatter, UserAttributeFormatter>();
            services.AddScoped<IUserAttributeParser, UserAttributeParser>();
            services.AddScoped<IUserAttributeService, UserAttributeService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRegistrationService, UserRegistrationService>();
            services.AddScoped<IUserReportService, UserReportService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IAclService, AclService>();
            //services.AddScoped<IGeoLookupService, GeoLookupService>();
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<ICurrencyService, CurrencyService>();
            services.AddScoped<IMeasureService, MeasureService>();
            services.AddScoped<IStateProvinceService, StateProvinceService>();
            services.AddScoped<IClusteringService, ClusteringService>();
            services.AddScoped<INodeMappingService, NodeMappingService>();
            services.AddScoped<ILocalizationService, LocalizationService>();
            services.AddScoped<ILocalizedEntityService, LocalizedEntityService>();
            services.AddScoped<ILanguageService, LanguageService>();
            services.AddScoped<IDownloadService, DownloadService>();
            services.AddScoped<IMessageTemplateService, MessageTemplateService>();
            services.AddScoped<IQueuedEmailService, QueuedEmailService>();
            services.AddScoped<INewsLetterSubscriptionService, NewsLetterSubscriptionService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ICampaignService, CampaignService>();
            services.AddScoped<IEmailAccountService, EmailAccountService>();
            services.AddScoped<IWorkflowMessageService, WorkflowMessageService>();
            services.AddScoped<IMessageTokenProvider, MessageTokenProvider>();
            services.AddScoped<ITokenizer, Tokenizer>();
            services.AddScoped<ISmtpBuilder, SmtpBuilder>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IEncryptionService, EncryptionService>();
            services.AddScoped<IAuthenticationService, CookieAuthenticationService>();
            services.AddScoped<IUrlRecordService, UrlRecordService>();
            //services.AddScoped<ITaxCategoryService, TaxCategoryService>();
            //services.AddScoped<ITaxService, TaxService>();
            services.AddScoped<ILogService, DefaultLogger>();
            services.AddScoped<IUserActivityService, UserActivityService>();
            services.AddScoped<IGdprService, GdprService>();
            //services.AddScoped<IPollService, PollService>();
            services.AddScoped<IBlogService, BlogService>();
            services.AddScoped<ITopicService, TopicService>();
            services.AddScoped<INewsService, NewsService>();
            //services.AddScoped<ISystemMessageService, SystemMessageService>();
            services.AddScoped<IDateTimeHelper, DateTimeHelper>();
            //services.AddScoped<IDraCoHtmlHelper, DraCoHtmlHelper>();
            services.AddScoped<IScheduleTaskService, ScheduleTaskService>();
            services.AddScoped<IExportManager, ExportManager>();
            services.AddScoped<IImportManager, ImportManager>();
            services.AddScoped<IUploadService, UploadService>();
            services.AddScoped<IExternalAuthenticationService, ExternalAuthenticationService>();
            //services.AddScoped<IWalletAuthenticationService, WalletAuthenticationService>();
            services.AddSingleton<IRoutePublisher, RoutePublisher>();
            services.AddSingleton<INodeEventPublisher, EventPublisher>();
            services.AddScoped<ISettingService, SettingService>();
            services.AddScoped<IBBCodeHelper, BBCodeHelper>();
            services.AddScoped<IHtmlFormatter, HtmlFormatter>();
            services.AddScoped<IVideoService, VideoService>();
            //services.AddScoped<IDraCoUrlHelper, DraCoUrlHelper>();

            //services.AddScoped<IMetaverseService, MetaverseService>();
            //services.AddScoped<IBlockchainService, BlockchainService>();
            //services.AddScoped<ICryptoAddressService, CryptoAddressService>();
            //services.AddScoped<ITokenService, TokenService>();
            //services.AddScoped<INonFungibleTokenService, NonFungibleTokenService>();
            //services.AddScoped<ISmartContractService, SmartContractService>();
            //services.AddScoped<IBlockchainApiService, BlockchainApiService>();

            //services.AddScoped<IAchievementService, AchievementService>();
            //services.AddScoped<IGameplayService, GameplayService>();
            //services.AddScoped<IPlayerCreationService, PlayerCreationService>();
            //services.AddScoped<IGovernanceService, GovernanceService>();
            //services.AddScoped<IRankService, RankService>();
            //services.AddScoped<IPlayerService, PlayerService>();
            //services.AddScoped<IPlayerAttributeFormatter, PlayerAttributeFormatter>();
            //services.AddScoped<IPlayerAttributeParser, PlayerAttributeParser>();
            //services.AddScoped<IPlayerAttributeService, PlayerAttributeService>();
            //services.AddScoped<IPlayerExperienceService, PlayerExperienceService>();
            //services.AddScoped<IInventoryService, InventoryService>();
            //services.AddScoped<IQuestService, QuestService>();
            //services.AddScoped<IQuestEnrollmentService, QuestEnrollmentService>();
            //services.AddScoped<IGoalService, GoalService>();
            //services.AddScoped<IGoalAttributeService, GoalAttributeService>();
            //services.AddScoped<IGoalAttributeParser, GoalAttributeParser>();
            //services.AddScoped<IGoalAttributeFormatter, GoalAttributeFormatter>();
            //services.AddScoped<IRewardService, RewardService>();
            //services.AddScoped<ITournamentService, TournamentService>();
            //services.AddScoped<ITournamentEnrollmentService, TournamentEnrollmentService>();

            //module managers
            services.AddScoped(typeof(IModuleManager<>), typeof(ModuleManager<>));
            services.AddScoped<IAuthenticationModuleManager, AuthenticationModuleManager>();
            //services.AddScoped<IBlockchainRpcApiModuleManager, BlockchainRpcApiModuleManager>();
            //services.AddScoped<IMetaverseModuleManager, MetaverseModuleManager>();
            services.AddScoped<IMultiFactorAuthenticationModuleManager, MultiFactorAuthenticationModuleManager>();
            //services.AddScoped<IWalletExtensionModuleManager, WalletExtensionModuleManager>();
            services.AddScoped<IWidgetModuleManager, WidgetModuleManager>();
            services.AddScoped<IExchangeRateModuleManager, ExchangeRateModuleManager>();
            //services.AddScoped<ITaxModuleManager, TaxModuleManager>();
            //services.AddScoped<ITournamentModuleManager, TournamentModuleManager>();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            //register all settings
            var typeFinder = Singleton<ITypeFinder>.Instance;

            var settings = typeFinder.FindClassesOfType(typeof(ISettings), false).ToList();
            foreach (var setting in settings)
            {
                services.AddScoped(setting, serviceProvider =>
                {
                    var nodeId = DataSettingsManager.IsDatabaseInstalled()
                        ? ((NI2SNode)serviceProvider.GetRequiredService<INodeContext>().GetCurrentNode())?.Id ?? 0
                        : 0;

                    return serviceProvider.GetRequiredService<ISettingService>().LoadSettingAsync(setting, nodeId).Result;
                });
            }

            //picture service
            if (ni2sSettings.Get<AzureBlobConfig>().Enabled)
                services.AddScoped<IPictureService, AzurePictureService>();
            else
                services.AddScoped<IPictureService, PictureService>();

            //roxy file manager
            services.AddScoped<IRoxyFilemanService, RoxyFilemanService>();
            services.AddScoped<IRoxyFilemanFileProvider, RoxyFilemanFileProvider>();

            //installation service
            services.AddScoped<IInstallationService, InstallationService>();

            ////slug route transformer
            //if (DataSettingsManager.IsDatabaseInstalled())
            //    services.AddScoped<SlugRouteTransformer>();

            //schedule tasks
            services.AddSingleton<ITaskScheduler, TaskScheduler>();
            services.AddTransient<IScheduleTaskRunner, ScheduleTaskRunner>();

            //event consumers
            var consumers = typeFinder.FindClassesOfType(typeof(IConsumer<>)).ToList();
            foreach (var consumer in consumers)
                foreach (var findInterface in consumer.FindInterfaces((type, criteria) =>
                {
                    var isMatch = type.IsGenericType && ((Type)criteria).IsAssignableFrom(type.GetGenericTypeDefinition());
                    return isMatch;
                }, typeof(IConsumer<>)))
                    services.AddScoped(findInterface, consumer);

            //XML sitemap
            services.AddScoped<IXmlSiteMap, XmlSiteMap>();

            //register the Lazy resolver for .Net IoC
            var useAutofac = ni2sSettings.Get<CommonConfig>().UseAutofac;
            if (!useAutofac)
                services.AddScoped(typeof(Lazy<>), typeof(LazyInstance<>));
        }

        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => 2000;
    }
}
