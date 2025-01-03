using static ARWNI2S.Framework.Users.Security.StandardPermission;

namespace ARWNI2S.Framework.Users.Security
{
    /// <summary>
    /// Default permission config manager
    /// </summary>
    public partial class DefaultPermissionConfigManager : IPermissionConfigManager
    {
        public IList<PermissionConfig> AllConfigs => new List<PermissionConfig>
        {
            #region Security
        
            new ("Security. Enable Multi-factor authentication", StandardPermission.Security.ENABLE_MULTI_FACTOR_AUTHENTICATION, nameof(StandardPermission.Security), UserDefaults.AdministratorsRoleName, UserDefaults.RegisteredRoleName),
            new ("Access admin area", StandardPermission.Security.ACCESS_ADMIN_PANEL, nameof(StandardPermission.Security), UserDefaults.AdministratorsRoleName, UserDefaults.VendorsRoleName),

            #endregion

            #region Users
        
            new ("Admin area. Users. View", Users.CUSTOMERS_VIEW, nameof(Users), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Users. Create, edit, delete", Users.CUSTOMERS_CREATE_EDIT_DELETE, nameof(Users), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Users. Import and export", Users.CUSTOMERS_IMPORT_EXPORT, nameof(Users), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Users. Allow impersonation", Users.CUSTOMERS_IMPERSONATION, nameof(Users), UserDefaults.AdministratorsRoleName),
            new ("Admin area. User roles. View", Users.CUSTOMER_ROLES_VIEW, nameof(Users), UserDefaults.AdministratorsRoleName),
            new ("Admin area. User roles. Create, edit, delete", Users.CUSTOMER_ROLES_CREATE_EDIT_DELETE, nameof(Users), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Vendors. View", Users.VENDORS_VIEW, nameof(Users), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Vendors. Create, edit, delete", Users.VENDORS_CREATE_EDIT_DELETE, nameof(Users), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Activity Log. View", Users.ACTIVITY_LOG_VIEW, nameof(Users), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Activity Log. Delete", Users.ACTIVITY_LOG_DELETE, nameof(Users), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Activity Log. Manage types", Users.ACTIVITY_LOG_MANAGE_TYPES, nameof(Users), UserDefaults.AdministratorsRoleName),
            new ("Admin area. GDPR. Manage", Users.GDPR_MANAGE, nameof(Users), UserDefaults.AdministratorsRoleName),

            #endregion

            #region Orders
        
            new ("Admin area. Current Carts. Manage", Orders.CURRENT_CARTS_MANAGE, nameof(Orders), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Orders. View", Orders.ORDERS_VIEW, nameof(Orders), UserDefaults.AdministratorsRoleName, UserDefaults.VendorsRoleName),
            new ("Admin area. Orders. Create, edit, delete", Orders.ORDERS_CREATE_EDIT_DELETE, nameof(Orders), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Orders. Import and export", Orders.ORDERS_IMPORT_EXPORT, nameof(Orders), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Shipments. View", Orders.SHIPMENTS_VIEW, nameof(Orders), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Shipments. Create, edit, delete", Orders.SHIPMENTS_CREATE_EDIT_DELETE, nameof(Orders), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Return requests. View", Orders.RETURN_REQUESTS_VIEW, nameof(Orders), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Return requests. Create, edit, delete", Orders.RETURN_REQUESTS_CREATE_EDIT_DELETE, nameof(Orders), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Recurring payments. View", Orders.RECURRING_PAYMENTS_VIEW, nameof(Orders), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Recurring payments. Create, edit, delete", Orders.RECURRING_PAYMENTS_CREATE_EDIT_DELETE, nameof(Orders), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Gift cards. View", Orders.GIFT_CARDS_VIEW, nameof(Orders), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Gift cards. Create, edit, delete", Orders.GIFT_CARDS_CREATE_EDIT_DELETE, nameof(Orders), UserDefaults.AdministratorsRoleName),

            #endregion

            #region Reports
        
            new ("Admin area. Reports. Sales summary", Reports.SALES_SUMMARY, nameof(Reports), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Reports. Country sales", Reports.COUNTRY_SALES, nameof(Reports), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Reports. Low stock", Reports.LOW_STOCK, nameof(Reports), UserDefaults.AdministratorsRoleName, UserDefaults.VendorsRoleName),
            new ("Admin area. Reports. Bestsellers", Reports.BESTSELLERS, nameof(Reports), UserDefaults.AdministratorsRoleName, UserDefaults.VendorsRoleName),
            new ("Admin area. Reports. Products never purchased", Reports.PRODUCTS_NEVER_PURCHASED, nameof(Reports), UserDefaults.AdministratorsRoleName, UserDefaults.VendorsRoleName),
            new ("Admin area. Reports. Registered users", Reports.REGISTERED_CUSTOMERS, nameof(Reports), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Reports. Users by order total", Reports.CUSTOMERS_BY_ORDER_TOTAL, nameof(Reports), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Reports. Users by number of orders", Reports.CUSTOMERS_BY_NUMBER_OF_ORDERS, nameof(Reports), UserDefaults.AdministratorsRoleName),

            #endregion

            #region Catalog
        
            new ("Admin area. Products. View", Catalog.PRODUCTS_VIEW, nameof(Catalog), UserDefaults.AdministratorsRoleName, UserDefaults.VendorsRoleName),
            new ("Admin area. Products. Create, edit, delete", Catalog.PRODUCTS_CREATE_EDIT_DELETE, nameof(Catalog), UserDefaults.AdministratorsRoleName, UserDefaults.VendorsRoleName),
            new ("Admin area. Products. Import and export", Catalog.PRODUCTS_IMPORT_EXPORT, nameof(Catalog), UserDefaults.AdministratorsRoleName, UserDefaults.VendorsRoleName),
            new ("Admin area. Categories. View", Catalog.CATEGORIES_VIEW, nameof(Catalog), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Categories. Create, edit, delete", Catalog.CATEGORIES_CREATE_EDIT_DELETE, nameof(Catalog), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Categories. Import and export", Catalog.CATEGORIES_IMPORT_EXPORT, nameof(Catalog), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Manufacturer. View", Catalog.MANUFACTURER_VIEW, nameof(Catalog), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Manufacturer. Create, edit, delete", Catalog.MANUFACTURER_CREATE_EDIT_DELETE, nameof(Catalog), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Manufacturer. Import and export", Catalog.MANUFACTURER_IMPORT_EXPORT, nameof(Catalog), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Product reviews. View", Catalog.PRODUCT_REVIEWS_VIEW, nameof(Catalog), UserDefaults.AdministratorsRoleName, UserDefaults.VendorsRoleName),
            new ("Admin area. Product reviews. Create, edit, delete", Catalog.PRODUCT_REVIEWS_CREATE_EDIT_DELETE, nameof(Catalog), UserDefaults.AdministratorsRoleName, UserDefaults.VendorsRoleName),
            new ("Admin area. Product tags. View", Catalog.PRODUCT_TAGS_VIEW, nameof(Catalog), UserDefaults.AdministratorsRoleName, UserDefaults.VendorsRoleName),
            new ("Admin area. Product tags. Create, edit, delete", Catalog.PRODUCT_TAGS_CREATE_EDIT_DELETE, nameof(Catalog), UserDefaults.AdministratorsRoleName, UserDefaults.VendorsRoleName),
            new ("Admin area. Product attributes. View", Catalog.PRODUCT_ATTRIBUTES_VIEW, nameof(Catalog), UserDefaults.AdministratorsRoleName, UserDefaults.VendorsRoleName),
            new ("Admin area. Product attributes. Create, edit, delete", Catalog.PRODUCT_ATTRIBUTES_CREATE_EDIT_DELETE, nameof(Catalog), UserDefaults.AdministratorsRoleName, UserDefaults.VendorsRoleName),
            new ("Admin area. Specification attributes. View", Catalog.SPECIFICATION_ATTRIBUTES_VIEW, nameof(Catalog), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Specification attributes. Create, edit, delete", Catalog.SPECIFICATION_ATTRIBUTES_CREATE_EDIT_DELETE, nameof(Catalog), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Checkout attributes. View", Catalog.CHECKOUT_ATTRIBUTES_VIEW, nameof(Catalog), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Checkout attributes. Create, edit, delete", Catalog.CHECKOUT_ATTRIBUTES_CREATE_EDIT_DELETE, nameof(Catalog), UserDefaults.AdministratorsRoleName),

            #endregion

            #region Promotions
        
            new ("Admin area. Discounts. View", Promotions.DISCOUNTS_VIEW, nameof(Promotions), UserDefaults.AdministratorsRoleName, UserDefaults.VendorsRoleName),
            new ("Admin area. Discounts. Create, edit, delete", Promotions.DISCOUNTS_CREATE_EDIT_DELETE, nameof(Promotions), UserDefaults.AdministratorsRoleName, UserDefaults.VendorsRoleName),
            new ("Admin area. Affiliates. View", Promotions.AFFILIATES_VIEW, nameof(Promotions), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Affiliates. Create, edit, delete", Promotions.AFFILIATES_CREATE_EDIT_DELETE, nameof(Promotions), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Newsletter Subscribers. View", Promotions.SUBSCRIBERS_VIEW, nameof(Promotions), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Newsletter Subscribers. Create, edit, delete", Promotions.SUBSCRIBERS_CREATE_EDIT_DELETE, nameof(Promotions), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Newsletter Subscribers. Import and export", Promotions.SUBSCRIBERS_IMPORT_EXPORT, nameof(Promotions), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Campaigns. View", Promotions.CAMPAIGNS_VIEW, nameof(Promotions), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Campaigns. Create and Edit", Promotions.CAMPAIGNS_CREATE_EDIT, nameof(Promotions), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Campaigns. Delete", Promotions.CAMPAIGNS_DELETE, nameof(Promotions), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Campaigns. Send emails", Promotions.CAMPAIGNS_SEND_EMAILS, nameof(Promotions), UserDefaults.AdministratorsRoleName),

            #endregion

            #region Content management
        
            new ("Admin area. Topics. View", ContentManagement.TOPICS_VIEW, nameof(ContentManagement), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Topics. Create, edit, delete", ContentManagement.TOPICS_CREATE_EDIT_DELETE, nameof(ContentManagement), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Message Templates. View", ContentManagement.MESSAGE_TEMPLATES_VIEW, nameof(ContentManagement), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Message Templates. Create, edit, delete", ContentManagement.MESSAGE_TEMPLATES_CREATE_EDIT_DELETE, nameof(ContentManagement), UserDefaults.AdministratorsRoleName),
            new ("Admin area. News. View", ContentManagement.NEWS_VIEW, nameof(ContentManagement), UserDefaults.AdministratorsRoleName),
            new ("Admin area. News. Create, edit, delete", ContentManagement.NEWS_CREATE_EDIT_DELETE, nameof(ContentManagement), UserDefaults.AdministratorsRoleName),
            new ("Admin area. News comments. View", ContentManagement.NEWS_COMMENTS_VIEW, nameof(ContentManagement), UserDefaults.AdministratorsRoleName),
            new ("Admin area. News comments. Create, edit, delete", ContentManagement.NEWS_COMMENTS_CREATE_EDIT_DELETE, nameof(ContentManagement), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Blog. View", ContentManagement.BLOG_VIEW, nameof(ContentManagement), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Blog. Create, edit, delete", ContentManagement.BLOG_CREATE_EDIT_DELETE, nameof(ContentManagement), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Blog comments. View", ContentManagement.BLOG_COMMENTS_VIEW, nameof(ContentManagement), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Blog comments. Create, edit, delete", ContentManagement.BLOG_COMMENTS_CREATE_EDIT_DELETE, nameof(ContentManagement), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Polls. View", ContentManagement.POLLS_VIEW, nameof(ContentManagement), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Polls. Create, edit, delete", ContentManagement.POLLS_CREATE_EDIT_DELETE, nameof(ContentManagement), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Forums. View", ContentManagement.FORUMS_VIEW, nameof(ContentManagement), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Forums. Create, edit, delete", ContentManagement.FORUMS_CREATE_EDIT_DELETE, nameof(ContentManagement), UserDefaults.AdministratorsRoleName),

            #endregion

            #region Configuration
        
            new ("Admin area. Widgets. Manage", StandardPermission.Configuration.MANAGE_WIDGETS, nameof(StandardPermission.Configuration), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Countries. Manage", StandardPermission.Configuration.MANAGE_COUNTRIES, nameof(StandardPermission.Configuration), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Languages. Manage", StandardPermission.Configuration.MANAGE_LANGUAGES, nameof(StandardPermission.Configuration), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Settings. Manage", StandardPermission.Configuration.MANAGE_SETTINGS, nameof(StandardPermission.Configuration), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Payment Methods. Manage", StandardPermission.Configuration.MANAGE_PAYMENT_METHODS, nameof(StandardPermission.Configuration), UserDefaults.AdministratorsRoleName),
            new ("Admin area. External Authentication Methods. Manage", StandardPermission.Configuration.MANAGE_EXTERNAL_AUTHENTICATION_METHODS, nameof(StandardPermission.Configuration), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Multi-factor Authentication Methods. Manage", StandardPermission.Configuration.MANAGE_MULTIFACTOR_AUTHENTICATION_METHODS, nameof(StandardPermission.Configuration), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Tax Settings. Manage", StandardPermission.Configuration.MANAGE_TAX_SETTINGS, nameof(StandardPermission.Configuration), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Shipping Settings. Manage", StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS, nameof(StandardPermission.Configuration), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Currencies. Manage", StandardPermission.Configuration.MANAGE_CURRENCIES, nameof(StandardPermission.Configuration), UserDefaults.AdministratorsRoleName),
            new ("Admin area. ACL. Manage", StandardPermission.Configuration.MANAGE_ACL, nameof(StandardPermission.Configuration), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Email Accounts. Manage", StandardPermission.Configuration.MANAGE_EMAIL_ACCOUNTS, nameof(StandardPermission.Configuration), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Stores. Manage", StandardPermission.Configuration.MANAGE_STORES, nameof(StandardPermission.Configuration), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Plugins. Manage", StandardPermission.Configuration.MANAGE_PLUGINS, nameof(StandardPermission.Configuration), UserDefaults.AdministratorsRoleName),
        
            #endregion

            #region System
        
            new ("Admin area. System Log. Manage", StandardPermission.System.MANAGE_SYSTEM_LOG, nameof(StandardPermission.System), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Message Queue. Manage", StandardPermission.System.MANAGE_MESSAGE_QUEUE, nameof(StandardPermission.System), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Maintenance. Manage", StandardPermission.System.MANAGE_MAINTENANCE, nameof(StandardPermission.System), UserDefaults.AdministratorsRoleName),
            new ("Admin area. HTML Editor. Manage pictures", StandardPermission.System.HTML_EDITOR_MANAGE_PICTURES, nameof(StandardPermission.System), UserDefaults.AdministratorsRoleName),
            new ("Admin area. Schedule Tasks. Manage", StandardPermission.System.MANAGE_SCHEDULE_TASKS, nameof(StandardPermission.System), UserDefaults.AdministratorsRoleName),
            new ("Admin area. App Settings. Manage", StandardPermission.System.MANAGE_APP_SETTINGS, nameof(StandardPermission.System), UserDefaults.AdministratorsRoleName),

            #endregion
        
            #region Public store
        
            new ("Public store. Display Prices", PublicStore.DISPLAY_PRICES, nameof(PublicStore), UserDefaults.AdministratorsRoleName, UserDefaults.RegisteredRoleName, UserDefaults.GuestsRoleName, UserDefaults.ForumModeratorsRoleName),
            new ("Public store. Enable shopping cart", PublicStore.ENABLE_SHOPPING_CART, nameof(PublicStore), UserDefaults.AdministratorsRoleName, UserDefaults.RegisteredRoleName, UserDefaults.GuestsRoleName, UserDefaults.ForumModeratorsRoleName),
            new ("Public store. Enable wishlist", PublicStore.ENABLE_WISHLIST, nameof(PublicStore), UserDefaults.AdministratorsRoleName, UserDefaults.RegisteredRoleName, UserDefaults.GuestsRoleName, UserDefaults.ForumModeratorsRoleName),
            new ("Public store. Allow navigation", PublicStore.PUBLIC_STORE_ALLOW_NAVIGATION, nameof(PublicStore), UserDefaults.AdministratorsRoleName, UserDefaults.RegisteredRoleName, UserDefaults.GuestsRoleName, UserDefaults.ForumModeratorsRoleName),
            new ("Public store. Access a closed store", PublicStore.ACCESS_CLOSED_STORE, nameof(PublicStore), UserDefaults.AdministratorsRoleName),

            #endregion
        };
    }
}