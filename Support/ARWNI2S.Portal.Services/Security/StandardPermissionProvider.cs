using ARWNI2S.Node.Core.Entities.Users;
using ARWNI2S.Portal.Services.Entities.Security;

namespace ARWNI2S.Portal.Services.Security
{
    /// <summary>
    /// Standard permission provider
    /// </summary>
    public partial class StandardPermissionProvider : IPermissionProvider
    {
        //admin area permissions
        public static readonly PermissionRecord AccessAdminPanel = new() { Name = "Access admin area", SystemName = "AccessAdminPanel", Category = "Standard" };
        public static readonly PermissionRecord AllowUserImpersonation = new() { Name = "Admin area. Allow User Impersonation", SystemName = "AllowUserImpersonation", Category = "Users" };
        public static readonly PermissionRecord ManageAttributes = new() { Name = "Admin area. Manage Attributes", SystemName = "ManageAttributes", Category = "Catalog" };
        public static readonly PermissionRecord ManageUsers = new() { Name = "Admin area. Manage Users", SystemName = "ManageUsers", Category = "Users" };
        public static readonly PermissionRecord ManagePartners = new() { Name = "Admin area. Manage Partners", SystemName = "ManagePartners", Category = "Users" };
        public static readonly PermissionRecord ManageAffiliates = new() { Name = "Admin area. Manage Affiliates", SystemName = "ManageAffiliates", Category = "Promo" };
        public static readonly PermissionRecord ManageCampaigns = new() { Name = "Admin area. Manage Campaigns", SystemName = "ManageCampaigns", Category = "Promo" };
        public static readonly PermissionRecord ManageNewsletterSubscribers = new() { Name = "Admin area. Manage Newsletter Subscribers", SystemName = "ManageNewsletterSubscribers", Category = "Promo" };
        public static readonly PermissionRecord ManagePolls = new() { Name = "Admin area. Manage Polls", SystemName = "ManagePolls", Category = "Content Management" };
        public static readonly PermissionRecord ManageNews = new() { Name = "Admin area. Manage News", SystemName = "ManageNews", Category = "Content Management" };
        public static readonly PermissionRecord ManageBlog = new() { Name = "Admin area. Manage Blog", SystemName = "ManageBlog", Category = "Content Management" };
        public static readonly PermissionRecord ManageWidgets = new() { Name = "Admin area. Manage Widgets", SystemName = "ManageWidgets", Category = "Content Management" };
        public static readonly PermissionRecord ManageTopics = new() { Name = "Admin area. Manage Topics", SystemName = "ManageTopics", Category = "Content Management" };
        public static readonly PermissionRecord ManageMessageTemplates = new() { Name = "Admin area. Manage Message Templates", SystemName = "ManageMessageTemplates", Category = "Content Management" };
        public static readonly PermissionRecord ManageCountries = new() { Name = "Admin area. Manage Countries", SystemName = "ManageCountries", Category = "Configuration" };
        public static readonly PermissionRecord ManageLanguages = new() { Name = "Admin area. Manage Languages", SystemName = "ManageLanguages", Category = "Configuration" };
        public static readonly PermissionRecord ManageSettings = new() { Name = "Admin area. Manage Settings", SystemName = "ManageSettings", Category = "Configuration" };
        public static readonly PermissionRecord ManageBlockchains = new() { Name = "Admin area. Manage Blockchains", SystemName = "ManageBlockchains", Category = "Configuration" };
        public static readonly PermissionRecord ManageBlockchainProviders = new() { Name = "Admin area. Manage Blockchain Providers", SystemName = "ManageBlockchainProviders", Category = "Configuration" };
        public static readonly PermissionRecord ManageTokens = new() { Name = "Admin area. Manage Tokens", SystemName = "ManageTokens", Category = "Configuration" };
        public static readonly PermissionRecord ManageGameplay = new() { Name = "Admin area. Manage Gameplay", SystemName = "ManageGameplay", Category = "Configuration" };
        public static readonly PermissionRecord ManagePlayers = new() { Name = "Admin area. Manage Players", SystemName = "ManagePlayers", Category = "Gameplay" };
        public static readonly PermissionRecord ManageTournaments = new() { Name = "Admin area. Manage Tournaments", SystemName = "ManageTournaments", Category = "Gameplay" };
        public static readonly PermissionRecord ManageGovernance = new() { Name = "Admin area. Manage Ranks", SystemName = "ManageGovernance", Category = "Gameplay" };
        public static readonly PermissionRecord ManageQuests = new() { Name = "Admin area. Manage Quests", SystemName = "ManageQuests", Category = "Gameplay" };
        public static readonly PermissionRecord ManageAchievements = new() { Name = "Admin area. Manage Achievements", SystemName = "ManageAchievements", Category = "Gameplay" };
        public static readonly PermissionRecord ManageMissions = new() { Name = "Admin area. Manage Activities", SystemName = "ManageMissions", Category = "Gameplay" };
        public static readonly PermissionRecord ManageExperience = new() { Name = "Admin area. Manage Experience", SystemName = "ManageExperience", Category = "Gameplay" };
        public static readonly PermissionRecord ManageExternalAuthenticationMethods = new() { Name = "Admin area. Manage External Authentication Methods", SystemName = "ManageExternalAuthenticationMethods", Category = "Configuration" };
        public static readonly PermissionRecord ManageMultifactorAuthenticationMethods = new() { Name = "Admin area. Manage Multi-factor Authentication Methods", SystemName = "ManageMultifactorAuthenticationMethods", Category = "Configuration" };
        public static readonly PermissionRecord ManageWalletAuthentication = new() { Name = "Admin area. Manage Wallet Authentication Methods", SystemName = "ManageWalletAuthentication", Category = "Configuration" };
        public static readonly PermissionRecord ManageGames = new() { Name = "Admin area. Manage Games", SystemName = "ManageGames", Category = "Configuration" };
        public static readonly PermissionRecord ManageGameTitles = new() { Name = "Admin area. Manage Game Titles", SystemName = "ManageGameTitles", Category = "Configuration" };
        public static readonly PermissionRecord ManageTaxSettings = new() { Name = "Admin area. Manage Tax Settings", SystemName = "ManageTaxSettings", Category = "Configuration" };
        public static readonly PermissionRecord ManageCurrencies = new() { Name = "Admin area. Manage Currencies", SystemName = "ManageCurrencies", Category = "Configuration" };
        public static readonly PermissionRecord ManageActivityLog = new() { Name = "Admin area. Manage Activity Log", SystemName = "ManageActivityLog", Category = "Configuration" };
        public static readonly PermissionRecord ManageAcl = new() { Name = "Admin area. Manage ACL", SystemName = "ManageACL", Category = "Configuration" };
        public static readonly PermissionRecord ManageEmailAccounts = new() { Name = "Admin area. Manage Email Accounts", SystemName = "ManageEmailAccounts", Category = "Configuration" };
        public static readonly PermissionRecord ManageNodes = new() { Name = "Admin area. Manage Nodes", SystemName = "ManageNodes", Category = "Configuration" };
        public static readonly PermissionRecord ManageModules = new() { Name = "Admin area. Manage Modules", SystemName = "ManageModules", Category = "Configuration" };
        public static readonly PermissionRecord ManageSystemLog = new() { Name = "Admin area. Manage System Log", SystemName = "ManageSystemLog", Category = "Configuration" };
        public static readonly PermissionRecord ManageMessageQueue = new() { Name = "Admin area. Manage Message Queue", SystemName = "ManageMessageQueue", Category = "Configuration" };
        public static readonly PermissionRecord ManageMaintenance = new() { Name = "Admin area. Manage Maintenance", SystemName = "ManageMaintenance", Category = "Configuration" };
        public static readonly PermissionRecord HtmlEditorManagePictures = new() { Name = "Admin area. HTML Editor. Manage pictures", SystemName = "HtmlEditor.ManagePictures", Category = "Configuration" };
        public static readonly PermissionRecord ManageScheduleTasks = new() { Name = "Admin area. Manage Schedule Tasks", SystemName = "ManageScheduleTasks", Category = "Configuration" };
        public static readonly PermissionRecord ManageAppSettings = new() { Name = "Admin area. Manage App Settings", SystemName = "ManageAppSettings", Category = "Configuration" };

        //public server permissions
        public static readonly PermissionRecord DisplayPrices = new() { Name = "Public node. Display Prices", SystemName = "DisplayPrices", Category = "PublicNode" };
        public static readonly PermissionRecord AccessGameplay = new() { Name = "Public node. Access gameplay contents", SystemName = "AccessGameplay", Category = "PublicNode" };
        public static readonly PermissionRecord AccessOfflineNode = new() { Name = "Public node. Access a offline node", SystemName = "AccessOfflineNode", Category = "PublicNode" };
        public static readonly PermissionRecord PublicNodeAllowNavigation = new() { Name = "Public node. Allow navigation", SystemName = "PublicNodeAllowNavigation", Category = "PublicNode" };
        public static readonly PermissionRecord AccessProfiling = new() { Name = "Public node. Access MiniProfiler results", SystemName = "AccessProfiling", Category = "PublicNode" };

        //Security
        public static readonly PermissionRecord EnableMultiFactorAuthentication = new() { Name = "Security. Enable Multi-factor authentication", SystemName = "EnableMultiFactorAuthentication", Category = "Security" };

        /// <summary>
        /// Get permissions
        /// </summary>
        /// <returns>Permissions</returns>
        public virtual IEnumerable<PermissionRecord> GetPermissions()
        {
            return new[]
            {
                AccessAdminPanel,
                AccessGameplay,
                AccessOfflineNode,
                AccessProfiling,
                AllowUserImpersonation,
                DisplayPrices,
                EnableMultiFactorAuthentication,
                HtmlEditorManagePictures,
                ManageAttributes,
                ManageUsers,
                ManagePartners,
                ManageAffiliates,
                ManageCampaigns,
                ManageNewsletterSubscribers,
                ManagePolls,
                ManageNews,
                ManageBlockchains,
                ManageBlockchainProviders,
                ManageTokens,
                ManageBlog,
                ManageWidgets,
                ManageTopics,
                ManageMessageTemplates,
                ManageCountries,
                ManageLanguages,
                ManageGameplay,
                ManagePlayers,
                ManageTournaments,
                ManageGovernance,
                ManageQuests,
                ManageMissions,
                ManageAchievements,
                ManageExperience,
                ManageSettings,
                ManageExternalAuthenticationMethods,
                ManageMultifactorAuthenticationMethods,
                ManageWalletAuthentication,
                ManageGames,
                ManageGameTitles,
                ManageTaxSettings,
                ManageCurrencies,
                ManageActivityLog,
                ManageAcl,
                ManageEmailAccounts,
                ManageNodes,
                ManageModules,
                ManageSystemLog,
                ManageMessageQueue,
                ManageMaintenance,
                ManageScheduleTasks,
                ManageAppSettings,
                PublicNodeAllowNavigation,
            };
        }

        /// <summary>
        /// Get default permissions
        /// </summary>
        /// <returns>Permissions</returns>
        public virtual HashSet<(string systemRoleName, PermissionRecord[] permissions)> GetDefaultPermissions()
        {
            return
            [
                (
                    UserDefaults.AdministratorsRoleName,
                    new[]
                    {
                        AccessAdminPanel,
                        AllowUserImpersonation,
                        ManageAttributes,
                        ManageUsers,
                        ManagePartners,
                        ManageAffiliates,
                        ManageCampaigns,
                        ManageNewsletterSubscribers,
                        ManagePolls,
                        ManageNews,
                        ManageBlockchains,
                        ManageBlockchainProviders,
                        ManageTokens,
                        ManageBlog,
                        ManageWidgets,
                        ManageTopics,
                        ManageMessageTemplates,
                        ManageCountries,
                        ManageLanguages,
                        ManageSettings,
                        ManageGameplay,
                        ManageTournaments,
                        ManagePlayers,
                        ManageGovernance,
                        ManageQuests,
                        ManageMissions,
                        ManageAchievements,
                        ManageExperience,
                        ManageExternalAuthenticationMethods,
                        ManageMultifactorAuthenticationMethods,
                        ManageWalletAuthentication,
                        ManageGames,
                        ManageGameTitles,
                        ManageTaxSettings,
                        ManageCurrencies,
                        ManageActivityLog,
                        ManageAcl,
                        ManageEmailAccounts,
                        ManageNodes,
                        ManageModules,
                        ManageSystemLog,
                        ManageMessageQueue,
                        ManageMaintenance,
                        HtmlEditorManagePictures,
                        ManageScheduleTasks,
                        ManageAppSettings,
                        DisplayPrices,
                        AccessGameplay,
                        PublicNodeAllowNavigation,
                        AccessOfflineNode,
                        AccessProfiling,
                        EnableMultiFactorAuthentication
                    }
                ),
                (
                    UserDefaults.ModeratorsRoleName,
                    new[]
                    {
                        DisplayPrices,
                        PublicNodeAllowNavigation
                    }
                ),
                (
                    UserDefaults.GuestsRoleName,
                    new[]
                    {
                        DisplayPrices,
                        PublicNodeAllowNavigation
                    }
                ),
                (
                    UserDefaults.RegisteredRoleName,
                    new[]
                    {
                        DisplayPrices,
                        PublicNodeAllowNavigation,
                        EnableMultiFactorAuthentication
                    }
                ),
                (
                    UserDefaults.PartnersRoleName,
                    new[]
                    {
                        AccessAdminPanel,
                        ManageGameTitles
                    }
                ),
                (
                    UserDefaults.PlayersRoleName,
                    new[]
                    {
                        AccessGameplay,
                    }
                )
            ];
        }
    }
}