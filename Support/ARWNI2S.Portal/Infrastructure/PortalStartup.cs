using ARWNI2S.Portal.Framework.Infrastructure;
using ARWNI2S.Portal.Infrastructure.Installation;

namespace ARWNI2S.Portal.Infrastructure
{
    public class PortalStartup : IWebStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>
        public virtual void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //installation localization service
            services.AddScoped<IInstallationLocalizationService, InstallationLocalizationService>();

            //common factories
            //services.AddScoped<IAclSupportedModelFactory, AclSupportedModelFactory>();
            //services.AddScoped<ILocalizedModelFactory, LocalizedModelFactory>();
            //services.AddScoped<INodeMappingSupportedModelFactory, NodeMappingSupportedModelFactory>();

            //admin factories
            //services.AddScoped<IBaseAdminModelFactory, BaseAdminModelFactory>();
            //services.AddScoped<IActivityLogModelFactory, ActivityLogModelFactory>();
            //services.AddScoped<IAddressModelFactory, AddressModelFactory>();
            //services.AddScoped<IAddressAttributeModelFactory, AddressAttributeModelFactory>();
            //services.AddScoped<IAffiliateModelFactory, AffiliateModelFactory>();
            //services.AddScoped<IBlogModelFactory, BlogModelFactory>();
            //services.AddScoped<ICampaignModelFactory, CampaignModelFactory>();
            //services.AddScoped<ICommonModelFactory, CommonModelFactory>();
            //services.AddScoped<ICountryModelFactory, CountryModelFactory>();
            //services.AddScoped<ICurrencyModelFactory, CurrencyModelFactory>();
            //services.AddScoped<IUserAttributeModelFactory, UserAttributeModelFactory>();
            //services.AddScoped<IUserModelFactory, UserModelFactory>();
            //services.AddScoped<IUserRoleModelFactory, UserRoleModelFactory>();
            //services.AddScoped<IEmailAccountModelFactory, EmailAccountModelFactory>();
            //services.AddScoped<IExternalAuthenticationMethodModelFactory, ExternalAuthenticationMethodModelFactory>();
            //services.AddScoped<IGamesModelFactory, GamesModelFactory>();
            //services.AddScoped<IHomeModelFactory, HomeModelFactory>();
            //services.AddScoped<ILanguageModelFactory, LanguageModelFactory>();
            //services.AddScoped<ILogModelFactory, LogModelFactory>();
            //services.AddScoped<IMeasureModelFactory, MeasureModelFactory>();
            //services.AddScoped<IMessageTemplateModelFactory, MessageTemplateModelFactory>();
            //services.AddScoped<IMultiFactorAuthenticationMethodModelFactory, MultiFactorAuthenticationMethodModelFactory>();
            //services.AddScoped<INewsletterSubscriptionModelFactory, NewsletterSubscriptionModelFactory>();
            //services.AddScoped<INewsModelFactory, NewsModelFactory>();
            //services.AddScoped<IModuleModelFactory, ModuleModelFactory>();
            //services.AddScoped<IPollModelFactory, PollModelFactory>();
            //services.AddScoped<IReportModelFactory, ReportModelFactory>();
            //services.AddScoped<IQueuedEmailModelFactory, QueuedEmailModelFactory>();
            //services.AddScoped<IScheduleTaskModelFactory, ScheduleTaskModelFactory>();
            //services.AddScoped<ISecurityModelFactory, SecurityModelFactory>();
            //services.AddScoped<ISettingModelFactory, SettingModelFactory>();
            //services.AddScoped<INodeModelFactory, NodeModelFactory>();
            //services.AddScoped<ITaxModelFactory, TaxModelFactory>();
            //services.AddScoped<ITemplateModelFactory, TemplateModelFactory>();
            //services.AddScoped<ITopicModelFactory, TopicModelFactory>();
            //services.AddScoped<IPartnerAttributeModelFactory, PartnerAttributeModelFactory>();
            //services.AddScoped<IPartnerModelFactory, PartnerModelFactory>();
            //services.AddScoped<IWidgetModelFactory, WidgetModelFactory>();

            //services.AddScoped<IBlockchainModelFactory, BlockchainModelFactory>();
            //services.AddScoped<ITokenModelFactory, TokenModelFactory>();
            //services.AddScoped<ISmartContractModelFactory, SmartContractModelFactory>();
            //services.AddScoped<INonFungibleTokenModelFactory, NonFungibleTokenModelFactory>();
            //services.AddScoped<IWalletExtensionModelFactory, WalletExtensionModelFactory>();

            //services.AddScoped<IGameplayModelFactory, GameplayModelFactory>();
            //services.AddScoped<IPlayerModelFactory, PlayerModelFactory>();
            //services.AddScoped<IPlayerAttributeModelFactory, PlayerAttributeModelFactory>();
            //services.AddScoped<IPlayerExperienceModelFactory, PlayerExperienceModelFactory>();
            //services.AddScoped<IInventoryItemModelFactory, InventoryItemModelFactory>();
            //services.AddScoped<IGovernanceModelFactory, GovernanceModelFactory>();
            //services.AddScoped<IQuestModelFactory, QuestModelFactory>();
            //services.AddScoped<IGoalModelFactory, GoalModelFactory>();
            //services.AddScoped<IGoalAttributeModelFactory, GoalAttributeModelFactory>();
            //services.AddScoped<IRewardModelFactory, RewardModelFactory>();
            //services.AddScoped<ITournamentModelFactory, TournamentModelFactory>();

            //factories
            //services.AddScoped<Factories.IAddressModelFactory, Factories.AddressModelFactory>();
            //services.AddScoped<Factories.IBlogModelFactory, Factories.BlogModelFactory>();
            //services.AddScoped<Factories.ICommonModelFactory, Factories.CommonModelFactory>();
            //services.AddScoped<Factories.ICountryModelFactory, Factories.CountryModelFactory>();
            //services.AddScoped<Factories.ICryptoAddressModelFactory, Factories.CryptoAddressModelFactory>();
            //services.AddScoped<Factories.IExternalAuthenticationModelFactory, Factories.ExternalAuthenticationModelFactory>();
            //services.AddScoped<Factories.IGamesModelFactory, Factories.GamesModelFactory>();
            //services.AddScoped<Factories.IGameplayModelFactory, Factories.GameplayModelFactory>();
            //services.AddScoped<Factories.INewsModelFactory, Factories.NewsModelFactory>();
            //services.AddScoped<Factories.INewsletterModelFactory, Factories.NewsletterModelFactory>();
            //services.AddScoped<Factories.ISystemMessagesModelFactory, Factories.SystemMessagesModelFactory>();
            //services.AddScoped<Factories.IPartnerModelFactory, Factories.PartnerModelFactory>();
            //services.AddScoped<Factories.IPollModelFactory, Factories.PollModelFactory>();
            //services.AddScoped<Factories.IProfileModelFactory, Factories.ProfileModelFactory>();
            //services.AddScoped<Factories.IQuestsModelFactory, Factories.QuestsModelFactory>();
            //services.AddScoped<Factories.ISitemapModelFactory, Factories.SitemapModelFactory>();
            //services.AddScoped<Factories.ITopicModelFactory, Factories.TopicModelFactory>();
            //services.AddScoped<Factories.ITournamentModelFactory, Factories.TournamentModelFactory>();
            //services.AddScoped<Factories.IUserModelFactory, Factories.UserModelFactory>();
            //services.AddScoped<Factories.IBlockchainModelFactory, Factories.BlockchainModelFactory>();
            //services.AddScoped<Factories.IWidgetModelFactory, Factories.WidgetModelFactory>();

            //helpers classes
            //services.AddScoped<ITinyMceHelper, TinyMceHelper>();
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
        public int Order => 2002;
    }
}
