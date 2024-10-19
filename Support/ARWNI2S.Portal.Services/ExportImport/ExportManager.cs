using ARWNI2S.Infrastructure;
using ARWNI2S.Node.Core.Entities.Users;
using ARWNI2S.Node.Core.Services.Helpers;
using ARWNI2S.Node.Core.Entities.Localization;
using ARWNI2S.Node.Services;
using ARWNI2S.Node.Services.Clustering;
using ARWNI2S.Node.Services.Common;
using ARWNI2S.Node.Services.Localization;
using ARWNI2S.Node.Services.Logging;
using ARWNI2S.Portal.Services.Entities.Common;
using ARWNI2S.Portal.Services.Entities.Directory;
using ARWNI2S.Portal.Services.Entities.Gdpr;
using ARWNI2S.Portal.Services.Entities.Mailing;
using ARWNI2S.Portal.Services.Entities.Tax;
using ARWNI2S.Portal.Services.Entities.Users;
using ARWNI2S.Portal.Services.ExportImport.Help;
using ARWNI2S.Portal.Services.Gdpr;
using ARWNI2S.Portal.Services.Globalization;
using ARWNI2S.Portal.Services.Mailing;
using ARWNI2S.Portal.Services.Media;
using ARWNI2S.Portal.Services.Users;
using ClosedXML.Excel;
using System.Globalization;
using System.Text;
using System.Xml;
using ARWNI2S.Portal.Services.Clustering;

namespace ARWNI2S.Portal.Services.ExportImport
{
    /// <summary>
    /// Export manager
    /// </summary>
    public partial class ExportManager : IExportManager
    {
        #region Fields

        private readonly AddressSettings _addressSettings;
        private readonly PortalSettings _portalSettings;
        private readonly IUserActivityService _userActivityService;
        private readonly WebUserSettings _userSettings;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly ICountryService _countryService;
        private readonly IUserAttributeFormatter _userAttributeFormatter;
        private readonly PortalUserService _userService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IGdprService _gdprService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IPictureService _pictureService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IClusteringService _clusteringService;
        //private readonly IPartnerService _partnerService;
        private readonly PortalWorkContext _workContext;

        #endregion

        #region Ctor

        public ExportManager(AddressSettings addressSettings,
            PortalSettings portalSettings,
            IUserActivityService userActivityService,
            WebUserSettings userSettings,
            DateTimeSettings dateTimeSettings,
            ICountryService countryService,
            IUserAttributeFormatter userAttributeFormatter,
            PortalUserService userService,
            IDateTimeHelper dateTimeHelper,
            IGdprService gdprService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IPictureService pictureService,
            IStateProvinceService stateProvinceService,
            IClusteringService clusteringService,
            //IPartnerService partnerService,
            PortalWorkContext workContext)
        {
            _addressSettings = addressSettings;
            _portalSettings = portalSettings;
            _userActivityService = userActivityService;
            _userSettings = userSettings;
            _dateTimeSettings = dateTimeSettings;
            _countryService = countryService;
            _userAttributeFormatter = userAttributeFormatter;
            _userService = userService;
            _dateTimeHelper = dateTimeHelper;
            _gdprService = gdprService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _newsLetterSubscriptionService = newsLetterSubscriptionService;
            _pictureService = pictureService;
            _stateProvinceService = stateProvinceService;
            _clusteringService = clusteringService;
            //_partnerService = partnerService;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        ///// <returns>A task that represents the asynchronous operation</returns>
        //protected virtual async Task<int> WriteCategoriesAsync(XmlWriter xmlWriter, int parentCategoryId, int totalCategories)
        //{
        //    var categories = await _categoryService.GetAllCategoriesByParentCategoryIdAsync(parentCategoryId, true);
        //    if (categories == null || !categories.Any())
        //        return totalCategories;

        //    totalCategories += categories.Count;

        //    var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

        //    foreach (var category in categories)
        //    {
        //        await xmlWriter.WriteStartElementAsync("Category");

        //        await xmlWriter.WriteStringAsync("Id", category.Id);

        //        await WriteLocalizedPropertyXmlAsync(category, c => c.Name, xmlWriter, languages);
        //        await WriteLocalizedPropertyXmlAsync(category, c => c.Description, xmlWriter, languages);
        //        await xmlWriter.WriteStringAsync("CategoryTemplateId", category.CategoryTemplateId);
        //        await WriteLocalizedPropertyXmlAsync(category, c => c.MetaKeywords, xmlWriter, languages, await IgnoreExportCategoryPropertyAsync());
        //        await WriteLocalizedPropertyXmlAsync(category, c => c.MetaDescription, xmlWriter, languages, await IgnoreExportCategoryPropertyAsync());
        //        await WriteLocalizedPropertyXmlAsync(category, c => c.MetaTitle, xmlWriter, languages, await IgnoreExportCategoryPropertyAsync());
        //        await WriteLocalizedSeNameXmlAsync(category, xmlWriter, languages, await IgnoreExportCategoryPropertyAsync());
        //        await xmlWriter.WriteStringAsync("ParentCategoryId", category.ParentCategoryId);
        //        await xmlWriter.WriteStringAsync("PictureId", category.PictureId);
        //        await xmlWriter.WriteStringAsync("PageSize", category.PageSize, await IgnoreExportCategoryPropertyAsync());
        //        await xmlWriter.WriteStringAsync("AllowUsersToSelectPageSize", category.AllowUsersToSelectPageSize, await IgnoreExportCategoryPropertyAsync());
        //        await xmlWriter.WriteStringAsync("PageSizeOptions", category.PageSizeOptions, await IgnoreExportCategoryPropertyAsync());
        //        await xmlWriter.WriteStringAsync("PriceRangeFiltering", category.PriceRangeFiltering, await IgnoreExportCategoryPropertyAsync());
        //        await xmlWriter.WriteStringAsync("PriceFrom", category.PriceFrom, await IgnoreExportCategoryPropertyAsync());
        //        await xmlWriter.WriteStringAsync("PriceTo", category.PriceTo, await IgnoreExportCategoryPropertyAsync());
        //        await xmlWriter.WriteStringAsync("ManuallyPriceRange", category.ManuallyPriceRange, await IgnoreExportCategoryPropertyAsync());
        //        await xmlWriter.WriteStringAsync("ShowOnHomepage", category.ShowOnHomepage, await IgnoreExportCategoryPropertyAsync());
        //        await xmlWriter.WriteStringAsync("IncludeInTopMenu", category.IncludeInTopMenu, await IgnoreExportCategoryPropertyAsync());
        //        await xmlWriter.WriteStringAsync("Published", category.Published, await IgnoreExportCategoryPropertyAsync());
        //        await xmlWriter.WriteStringAsync("Deleted", category.Deleted, true);
        //        await xmlWriter.WriteStringAsync("DisplayOrder", category.DisplayOrder);
        //        await xmlWriter.WriteStringAsync("CreatedOnUtc", category.CreatedOnUtc, await IgnoreExportCategoryPropertyAsync());
        //        await xmlWriter.WriteStringAsync("UpdatedOnUtc", category.UpdatedOnUtc, await IgnoreExportCategoryPropertyAsync());

        //        await xmlWriter.WriteStartElementAsync("Products");
        //        var productCategories = await _categoryService.GetProductCategoriesByCategoryIdAsync(category.Id, showHidden: true);
        //        foreach (var productCategory in productCategories)
        //        {
        //            var product = await _productService.GetProductByIdAsync(productCategory.ProductId);
        //            if (product == null || product.Deleted)
        //                continue;

        //            await xmlWriter.WriteStartElementAsync("ProductCategory");
        //            await xmlWriter.WriteStringAsync("ProductCategoryId", productCategory.Id);
        //            await xmlWriter.WriteStringAsync("ProductId", productCategory.ProductId);
        //            await WriteLocalizedPropertyXmlAsync(product, p => p.Name, xmlWriter, languages, overriddenNodeName: "ProductName");
        //            await xmlWriter.WriteStringAsync("IsFeaturedProduct", productCategory.IsFeaturedProduct);
        //            await xmlWriter.WriteStringAsync("DisplayOrder", productCategory.DisplayOrder);
        //            await xmlWriter.WriteEndElementAsync();
        //        }

        //        await xmlWriter.WriteEndElementAsync();

        //        await xmlWriter.WriteStartElementAsync("SubCategories");
        //        totalCategories = await WriteCategoriesAsync(xmlWriter, category.Id, totalCategories);
        //        await xmlWriter.WriteEndElementAsync();
        //        await xmlWriter.WriteEndElementAsync();
        //    }

        //    return totalCategories;
        //}

        /// <summary>
        /// Returns the path to the image file by ID
        /// </summary>
        /// <param name="pictureId">Picture ID</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the path to the image file
        /// </returns>
        protected virtual async Task<string> GetPicturesAsync(int pictureId)
        {
            var picture = await _pictureService.GetPictureByIdAsync(pictureId);

            return await _pictureService.GetThumbLocalPathAsync(picture);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task<bool> IgnoreExportCategoryPropertyAsync()
        {
            try
            {
                return !await _genericAttributeService.GetAttributeAsync<bool>(await _workContext.GetCurrentUserAsync(), "category-advanced-mode");
            }
            catch (ArgumentNullException)
            {
                return false;
            }
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task<object> GetCustomUserAttributesAsync(User user)
        {
            return await _userAttributeFormatter.FormatAttributesAsync(user.CustomUserAttributesXML, ";");
        }

        ///// <returns>A task that represents the asynchronous operation</returns>
        //private async Task WriteLocalizedPropertyXmlAsync<TEntity, TPropType>(TEntity entity, Expression<Func<TEntity, TPropType>> keySelector,
        //    XmlWriter xmlWriter, IList<Language> languages, bool ignore = false, string overriddenNodeName = null)
        //    where TEntity : BaseEntity, ILocalizedEntity
        //{
        //    if (ignore)
        //        return;

        //    if (entity == null)
        //        throw new ArgumentNullException(nameof(entity));

        //    if (keySelector.Body is not MemberExpression member)
        //        throw new ArgumentException($"Expression '{keySelector}' refers to a method, not a property.");

        //    if (member.Member is not PropertyInfo propInfo)
        //        throw new ArgumentException($"Expression '{keySelector}' refers to a field, not a property.");

        //    var localeKeyGroup = entity.GetType().Name;
        //    var localeKey = propInfo.Name;

        //    var nodeName = localeKey;
        //    if (!string.IsNullOrWhiteSpace(overriddenNodeName))
        //        nodeName = overriddenNodeName;

        //    await xmlWriter.WriteStartElementAsync(nodeName);
        //    await xmlWriter.WriteStringAsync("Standard", propInfo.GetValue(entity));

        //    if (languages.Count >= 2)
        //    {
        //        await xmlWriter.WriteStartElementAsync("Locales");

        //        var properties = await _localizedEntityService.GetEntityLocalizedPropertiesAsync(entity.Id, localeKeyGroup, localeKey);
        //        foreach (var language in languages)
        //            if (properties.FirstOrDefault(lp => lp.LanguageId == language.Id) is LocalizedProperty localizedProperty)
        //                await xmlWriter.WriteStringAsync(language.UniqueSeoCode, localizedProperty.LocaleValue);

        //        await xmlWriter.WriteEndElementAsync();
        //    }

        //    await xmlWriter.WriteEndElementAsync();
        //}

        ///// <returns>A task that represents the asynchronous operation</returns>
        //private async Task WriteLocalizedSeNameXmlAsync<TEntity>(TEntity entity, XmlWriter xmlWriter, IList<Language> languages,
        //    bool ignore = false, string overriddenNodeName = null)
        //    where TEntity : BaseEntity, ISlugSupported
        //{
        //    if (ignore)
        //        return;

        //    if (entity == null)
        //        throw new ArgumentNullException(nameof(entity));

        //    var nodeName = "SEName";
        //    if (!string.IsNullOrWhiteSpace(overriddenNodeName))
        //        nodeName = overriddenNodeName;

        //    await xmlWriter.WriteStartElementAsync(nodeName);
        //    await xmlWriter.WriteStringAsync("Standard", await _urlRecordService.GetSeNameAsync(entity, 0));

        //    if (languages.Count >= 2)
        //    {
        //        await xmlWriter.WriteStartElementAsync("Locales");

        //        foreach (var language in languages)
        //            if (await _urlRecordService.GetSeNameAsync(entity, language.Id, returnDefaultValue: false) is string seName && !string.IsNullOrWhiteSpace(seName))
        //                await xmlWriter.WriteStringAsync(language.UniqueSeoCode, seName);

        //        await xmlWriter.WriteEndElementAsync();
        //    }

        //    await xmlWriter.WriteEndElementAsync();
        //}

        #endregion

        #region Methods

        /// <summary>
        /// Export user list to XLSX
        /// </summary>
        /// <param name="users">Users</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<byte[]> ExportUsersToXlsxAsync(IList<User> users)
        {
            //var partners = await _partnerService.GetPartnersByUserIdsAsync(users.Select(c => c.Id).ToArray());

            //object getPartner(User user)
            //{
            //    if (!_portalSettings.ExportImportRelatedEntitiesByName)
            //        return user.PartnerId;

            //    return partners.FirstOrDefault(v => v.Id == user.PartnerId)?.Name ?? string.Empty;
            //}

            async Task<object> getCountry(User user)
            {
                var countryId = user.CountryId;

                if (!_portalSettings.ExportImportRelatedEntitiesByName)
                    return countryId;

                var country = await _countryService.GetCountryByIdAsync(countryId);

                return country?.Name ?? string.Empty;
            }

            async Task<object> getStateProvince(User user)
            {
                var stateProvinceId = user.StateProvinceId;

                if (!_portalSettings.ExportImportRelatedEntitiesByName)
                    return stateProvinceId;

                var stateProvince = await _stateProvinceService.GetStateProvinceByIdAsync(stateProvinceId);

                return stateProvince?.Name ?? string.Empty;
            }

            //property manager 
            var manager = new PropertyManager<User, Language>(new[]
            {
                new PropertyByName<User, Language>("UserId", (p, l) => p.Id),
                new PropertyByName<User, Language>("UserGuid", (p, l) => p.UserGuid),
                new PropertyByName<User, Language>("Email", (p, l) => p.Email),
                new PropertyByName<User, Language>("Username", (p, l) => p.Username),
                new PropertyByName<User, Language>("IsTaxExempt", (p, l) => p.IsTaxExempt),
                new PropertyByName<User, Language>("AffiliateId", (p, l) => p.AffiliateId),
                //new PropertyByName<User, Language>("Partner",  (p, l) => getPartner(p)),
                new PropertyByName<User, Language>("Active", (p, l) => p.Active),
                new PropertyByName<User, Language>("UserRoles",  async (p, l) =>  string.Join(", ",
                    (await _userService.GetUserRolesAsync(p)).Select(role => _portalSettings.ExportImportRelatedEntitiesByName ? role.Name : role.Id.ToString()))),
                new PropertyByName<User, Language>("IsGuest", async (p, l) => await _userService.IsGuestAsync(p)),
                new PropertyByName<User, Language>("IsRegistered", async (p, l) => await _userService.IsRegisteredAsync(p)),
                new PropertyByName<User, Language>("IsAdministrator", async (p, l) => await _userService.IsAdminAsync(p)),
                new PropertyByName<User, Language>("IsForumModerator", async (p, l) => await _userService.IsModeratorAsync(p)),
                new PropertyByName<User, Language>("IsPartner", async (p, l) => await _userService.IsPartnerAsync(p)),
                new PropertyByName<User, Language>("CreatedOnUtc", (p, l) => p.CreatedOnUtc),
                //attributes
                new PropertyByName<User, Language>("FirstName", (p, l) => p.FirstName, !_userSettings.FirstNameEnabled),
                new PropertyByName<User, Language>("LastName", (p, l) => p.LastName, !_userSettings.LastNameEnabled),
                new PropertyByName<User, Language>("Gender", (p, l) => p.Gender, !_userSettings.GenderEnabled),
                new PropertyByName<User, Language>("Company", (p, l) => p.Company, !_userSettings.CompanyEnabled),
                new PropertyByName<User, Language>("StreetAddress", (p, l) => p.StreetAddress, !_userSettings.StreetAddressEnabled),
                new PropertyByName<User, Language>("StreetAddress2", (p, l) => p.StreetAddress2, !_userSettings.StreetAddress2Enabled),
                new PropertyByName<User, Language>("ZipPostalCode", (p, l) => p.ZipPostalCode, !_userSettings.ZipPostalCodeEnabled),
                new PropertyByName<User, Language>("City", (p, l) => p.City, !_userSettings.CityEnabled),
                new PropertyByName<User, Language>("County", (p, l) => p.County, !_userSettings.CountyEnabled),
                new PropertyByName<User, Language>("Country",  async (p, l) => await getCountry(p), !_userSettings.CountryEnabled),
                new PropertyByName<User, Language>("StateProvince",  async (p, l) => await getStateProvince(p), !_userSettings.StateProvinceEnabled),
                new PropertyByName<User, Language>("Phone", (p, l) => p.Phone, !_userSettings.PhoneEnabled),
                new PropertyByName<User, Language>("Fax", (p, l) => p.Fax, !_userSettings.FaxEnabled),
                new PropertyByName<User, Language>("VatNumber", (p, l) => p.VatNumber),
                new PropertyByName<User, Language>("VatNumberStatusId", (p, l) => p.VatNumberStatusId),
                new PropertyByName<User, Language>("VatNumberStatus", (p, l) => p.VatNumberStatusId)
                {
                    DropDownElements = await VatNumberStatus.Unknown.ToSelectListAsync(useLocalization: false)
                },
                new PropertyByName<User, Language>("TimeZone", (p, l) => p.TimeZoneId, !_dateTimeSettings.AllowUsersToSetTimeZone),
                new PropertyByName<User, Language>("AvatarPictureId", async (p, l) => await _genericAttributeService.GetAttributeAsync<int>(p, UserDefaults.AvatarPictureIdAttribute), !_userSettings.AllowUsersToUploadAvatars),
                new PropertyByName<User, Language>("ForumPostCount", async (p, l) => await _genericAttributeService.GetAttributeAsync<int>(p, UserDefaults.ForumPostCountAttribute)),
                //new PropertyByName<User, Language>("Signature", async (p, l) => await _genericAttributeService.GetAttributeAsync<string>(p, UserDefaults.SignatureAttribute)),
                new PropertyByName<User, Language>("CustomUserAttributes", async (p, l) => await GetCustomUserAttributesAsync(p))
            }, _portalSettings);

            //activity log
            await _userActivityService.InsertActivityAsync(SystemKeywords.AdminArea.ExportUsers,
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.ExportUsers"), users.Count));

            return await manager.ExportToXlsxAsync(users);
        }

        /// <summary>
        /// Export user list to XML
        /// </summary>
        /// <param name="users">Users</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result in XML format
        /// </returns>
        public virtual async Task<string> ExportUsersToXmlAsync(IList<User> users)
        {
            var settings = new XmlWriterSettings
            {
                Async = true,
                ConformanceLevel = ConformanceLevel.Auto
            };

            await using var stringWriter = new StringWriter();
            await using var xmlWriter = XmlWriter.Create(stringWriter, settings);

            await xmlWriter.WriteStartDocumentAsync();
            await xmlWriter.WriteStartElementAsync("Users");
            await xmlWriter.WriteAttributeStringAsync("Version", NI2SVersion.CURRENT_VERSION);

            foreach (var user in users)
            {
                await xmlWriter.WriteStartElementAsync("User");
                await xmlWriter.WriteElementStringAsync("UserId", null, user.Id.ToString());
                await xmlWriter.WriteElementStringAsync("UserGuid", null, user.UserGuid.ToString());
                await xmlWriter.WriteElementStringAsync("Email", null, user.Email);
                await xmlWriter.WriteElementStringAsync("Username", null, user.Username);

                await xmlWriter.WriteElementStringAsync("IsTaxExempt", null, user.IsTaxExempt.ToString());
                await xmlWriter.WriteElementStringAsync("AffiliateId", null, user.AffiliateId.ToString());
                await xmlWriter.WriteElementStringAsync("PartnerId", null, user.PartnerId.ToString());
                await xmlWriter.WriteElementStringAsync("Active", null, user.Active.ToString());

                await xmlWriter.WriteElementStringAsync("IsGuest", null, (await _userService.IsGuestAsync(user)).ToString());
                await xmlWriter.WriteElementStringAsync("IsRegistered", null, (await _userService.IsRegisteredAsync(user)).ToString());
                await xmlWriter.WriteElementStringAsync("IsAdministrator", null, (await _userService.IsAdminAsync(user)).ToString());
                await xmlWriter.WriteElementStringAsync("IsForumModerator", null, (await _userService.IsModeratorAsync(user)).ToString());
                await xmlWriter.WriteElementStringAsync("CreatedOnUtc", null, user.CreatedOnUtc.ToString(CultureInfo.InvariantCulture));

                await xmlWriter.WriteElementStringAsync("FirstName", null, user.FirstName);
                await xmlWriter.WriteElementStringAsync("LastName", null, user.LastName);
                await xmlWriter.WriteElementStringAsync("Gender", null, user.Gender);
                await xmlWriter.WriteElementStringAsync("Company", null, user.Company);

                await xmlWriter.WriteElementStringAsync("CountryId", null, user.CountryId.ToString());
                await xmlWriter.WriteElementStringAsync("StreetAddress", null, user.StreetAddress);
                await xmlWriter.WriteElementStringAsync("StreetAddress2", null, user.StreetAddress2);
                await xmlWriter.WriteElementStringAsync("ZipPostalCode", null, user.ZipPostalCode);
                await xmlWriter.WriteElementStringAsync("City", null, user.City);
                await xmlWriter.WriteElementStringAsync("County", null, user.County);
                await xmlWriter.WriteElementStringAsync("StateProvinceId", null, user.StateProvinceId.ToString());
                await xmlWriter.WriteElementStringAsync("Phone", null, user.Phone);
                await xmlWriter.WriteElementStringAsync("Fax", null, user.Fax);
                await xmlWriter.WriteElementStringAsync("VatNumber", null, user.VatNumber);
                await xmlWriter.WriteElementStringAsync("VatNumberStatusId", null, user.VatNumberStatusId.ToString());
                await xmlWriter.WriteElementStringAsync("TimeZoneId", null, user.TimeZoneId);

                foreach (var node in await _clusteringService.GetAllNodesAsync())
                {
                    var newsletter = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndNodeIdAsync(user.Email, node.Id);
                    var subscribedToNewsletters = newsletter != null && newsletter.Active;
                    await xmlWriter.WriteElementStringAsync($"Newsletter-in-node-{node.Id}", null, subscribedToNewsletters.ToString());
                }

                await xmlWriter.WriteElementStringAsync("AvatarPictureId", null, (await _genericAttributeService.GetAttributeAsync<int>(user, UserDefaults.AvatarPictureIdAttribute)).ToString());
                await xmlWriter.WriteElementStringAsync("ForumPostCount", null, (await _genericAttributeService.GetAttributeAsync<int>(user, UserDefaults.ForumPostCountAttribute)).ToString());
                //await xmlWriter.WriteElementStringAsync("Signature", null, await _genericAttributeService.GetAttributeAsync<string>(user, UserDefaults.SignatureAttribute));

                if (!string.IsNullOrEmpty(user.CustomUserAttributesXML))
                {
                    var selectedUserAttributes = new StringReader(user.CustomUserAttributesXML);
                    var selectedUserAttributesXmlReader = XmlReader.Create(selectedUserAttributes);
                    await xmlWriter.WriteNodeAsync(selectedUserAttributesXmlReader, false);
                }

                await xmlWriter.WriteEndElementAsync();
            }

            await xmlWriter.WriteEndElementAsync();
            await xmlWriter.WriteEndDocumentAsync();
            await xmlWriter.FlushAsync();

            //activity log
            await _userActivityService.InsertActivityAsync(SystemKeywords.AdminArea.ExportUsers,
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.ExportUsers"), users.Count));

            return stringWriter.ToString();
        }

        /// <summary>
        /// Export newsletter subscribers to TXT
        /// </summary>
        /// <param name="subscriptions">Subscriptions</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result in TXT (string) format
        /// </returns>
        public virtual async Task<string> ExportNewsletterSubscribersToTxtAsync(IList<NewsLetterSubscription> subscriptions)
        {
            ArgumentNullException.ThrowIfNull(subscriptions);

            const char separator = ',';
            var sb = new StringBuilder();

            sb.Append(await _localizationService.GetResourceAsync("Admin.Promotions.NewsLetterSubscriptions.Fields.Email"));
            sb.Append(separator);
            sb.Append(await _localizationService.GetResourceAsync("Admin.Promotions.NewsLetterSubscriptions.Fields.Active"));
            sb.Append(separator);
            sb.Append(await _localizationService.GetResourceAsync("Admin.Promotions.NewsLetterSubscriptions.Fields.Node"));
            sb.Append(Environment.NewLine);

            foreach (var subscription in subscriptions)
            {
                sb.Append(subscription.Email);
                sb.Append(separator);
                sb.Append(subscription.Active);
                sb.Append(separator);
                sb.Append(subscription.NodeId);
                sb.Append(Environment.NewLine);
            }

            //activity log
            await _userActivityService.InsertActivityAsync(SystemKeywords.AdminArea.ExportNewsLetterSubscriptions,
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.ExportNewsLetterSubscriptions"), subscriptions.Count));

            return sb.ToString();
        }

        /// <summary>
        /// Export states to TXT
        /// </summary>
        /// <param name="states">States</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result in TXT (string) format
        /// </returns>
        public virtual async Task<string> ExportStatesToTxtAsync(IList<StateProvince> states)
        {
            ArgumentNullException.ThrowIfNull(states);

            const char separator = ',';
            var sb = new StringBuilder();
            foreach (var state in states)
            {
                sb.Append((await _countryService.GetCountryByIdAsync(state.CountryId)).TwoLetterIsoCode);
                sb.Append(separator);
                sb.Append(state.Name);
                sb.Append(separator);
                sb.Append(state.Abbreviation);
                sb.Append(separator);
                sb.Append(state.Published);
                sb.Append(separator);
                sb.Append(state.DisplayOrder);
                sb.Append(Environment.NewLine); //new line
            }

            //activity log
            await _userActivityService.InsertActivityAsync(SystemKeywords.AdminArea.ExportStates,
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.ExportStates"), states.Count));

            return sb.ToString();
        }

        /// <summary>
        /// Export user info (GDPR request) to XLSX 
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="nodeId">Node identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the user GDPR info
        /// </returns>
        public virtual async Task<byte[]> ExportUserGdprInfoToXlsxAsync(User user, int nodeId)
        {
            ArgumentNullException.ThrowIfNull(user);

            ////lambda expressions for choosing correct order address
            //async Task<Address> orderAddress(Order o) => await _addressService.GetAddressByIdAsync((o.PickupInNode ? o.PickupAddressId : o.ShippingAddressId) ?? 0);
            //async Task<Address> orderBillingAddress(Order o) => await _addressService.GetAddressByIdAsync(o.BillingAddressId);

            //user info and user attributes
            var userManager = new PropertyManager<User, Language>(new[]
            {
                new PropertyByName<User, Language>("Email", (p, l) => p.Email),
                new PropertyByName<User, Language>("Username", (p, l) => p.Username, !_userSettings.UsernamesEnabled), 
                //attributes
                new PropertyByName<User, Language>("First name", (p, l) => p.FirstName, !_userSettings.FirstNameEnabled),
                new PropertyByName<User, Language>("Last name", (p, l) => p.LastName, !_userSettings.LastNameEnabled),
                new PropertyByName<User, Language>("Gender", (p, l) => p.Gender, !_userSettings.GenderEnabled),
                new PropertyByName<User, Language>("Date of birth", (p, l) => p.DateOfBirth, !_userSettings.DateOfBirthEnabled),
                new PropertyByName<User, Language>("Company", (p, l) => p.Company, !_userSettings.CompanyEnabled),
                new PropertyByName<User, Language>("Street address", (p, l) => p.StreetAddress, !_userSettings.StreetAddressEnabled),
                new PropertyByName<User, Language>("Street address 2", (p, l) => p.StreetAddress2, !_userSettings.StreetAddress2Enabled),
                new PropertyByName<User, Language>("Zip / postal code", (p, l) => p.ZipPostalCode, !_userSettings.ZipPostalCodeEnabled),
                new PropertyByName<User, Language>("City", (p, l) => p.City, !_userSettings.CityEnabled),
                new PropertyByName<User, Language>("County", (p, l) => p.County, !_userSettings.CountyEnabled),
                new PropertyByName<User, Language>("Country", async (p, l) => (await _countryService.GetCountryByIdAsync(p.CountryId))?.Name ?? string.Empty, !_userSettings.CountryEnabled),
                new PropertyByName<User, Language>("State province", async (p, l) => (await _stateProvinceService.GetStateProvinceByIdAsync(p.StateProvinceId))?.Name ?? string.Empty, !(_userSettings.StateProvinceEnabled && _userSettings.CountryEnabled)),
                new PropertyByName<User, Language>("Phone", (p, l) => p.Phone, !_userSettings.PhoneEnabled),
                new PropertyByName<User, Language>("Fax", (p, l) => p.Fax, !_userSettings.FaxEnabled),
                new PropertyByName<User, Language>("User attributes",  async (p, l) => await GetCustomUserAttributesAsync(p))
            }, _portalSettings);

            //user orders
            var currentLanguage = await _workContext.GetWorkingLanguageAsync();
            //var orderManager = new PropertyManager<Order, Language>(new[]
            //{
            //    new PropertyByName<Order, Language>("Order Number", (p, l) => p.CustomOrderNumber),
            //    new PropertyByName<Order, Language>("Order status", async (p, l) => await _localizationService.GetLocalizedEnumAsync(p.OrderStatus)),
            //    new PropertyByName<Order, Language>("Order total", async (p, l) => await _priceFormatter.FormatPriceAsync(_currencyService.ConvertCurrency(p.OrderTotal, p.CurrencyRate), true, p.UserCurrencyCode, false, currentLanguage.Id)),
            //    new PropertyByName<Order, Language>("Shipping method", (p, l) => p.ShippingMethod),
            //    new PropertyByName<Order, Language>("Created on", async (p, l) => (await _dateTimeHelper.ConvertToUserTimeAsync(p.CreatedOnUtc, DateTimeKind.Utc)).ToString("D")),
            //    new PropertyByName<Order, Language>("Billing first name", async (p, l) => (await orderBillingAddress(p))?.FirstName ?? string.Empty),
            //    new PropertyByName<Order, Language>("Billing last name", async (p, l) => (await orderBillingAddress(p))?.LastName ?? string.Empty),
            //    new PropertyByName<Order, Language>("Billing email", async (p, l) => (await orderBillingAddress(p))?.Email ?? string.Empty),
            //    new PropertyByName<Order, Language>("Billing company", async (p, l) => (await orderBillingAddress(p))?.Company ?? string.Empty, !_addressSettings.CompanyEnabled),
            //    new PropertyByName<Order, Language>("Billing country", async (p, l) => await _countryService.GetCountryByAddressAsync(await orderBillingAddress(p)) is Country country ? await _localizationService.GetLocalizedAsync(country, c => c.Name) : string.Empty, !_addressSettings.CountryEnabled),
            //    new PropertyByName<Order, Language>("Billing state province", async (p, l) => await _stateProvinceService.GetStateProvinceByAddressAsync(await orderBillingAddress(p)) is StateProvince stateProvince ? await _localizationService.GetLocalizedAsync(stateProvince, sp => sp.Name) : string.Empty, !_addressSettings.StateProvinceEnabled),
            //    new PropertyByName<Order, Language>("Billing county", async (p, l) => (await orderBillingAddress(p))?.County ?? string.Empty, !_addressSettings.CountyEnabled),
            //    new PropertyByName<Order, Language>("Billing city", async (p, l) => (await orderBillingAddress(p))?.City ?? string.Empty, !_addressSettings.CityEnabled),
            //    new PropertyByName<Order, Language>("Billing address 1", async (p, l) => (await orderBillingAddress(p))?.Address1 ?? string.Empty, !_addressSettings.StreetAddressEnabled),
            //    new PropertyByName<Order, Language>("Billing address 2", async (p, l) => (await orderBillingAddress(p))?.Address2 ?? string.Empty, !_addressSettings.StreetAddress2Enabled),
            //    new PropertyByName<Order, Language>("Billing zip postal code", async (p, l) => (await orderBillingAddress(p))?.ZipPostalCode ?? string.Empty, !_addressSettings.ZipPostalCodeEnabled),
            //    new PropertyByName<Order, Language>("Billing phone number", async (p, l) => (await orderBillingAddress(p))?.PhoneNumber ?? string.Empty, !_addressSettings.PhoneEnabled),
            //    new PropertyByName<Order, Language>("Billing fax number", async (p, l) => (await orderBillingAddress(p))?.FaxNumber ?? string.Empty, !_addressSettings.FaxEnabled),
            //    new PropertyByName<Order, Language>("Shipping first name", async (p, l) => (await orderAddress(p))?.FirstName ?? string.Empty),
            //    new PropertyByName<Order, Language>("Shipping last name", async (p, l) => (await orderAddress(p))?.LastName ?? string.Empty),
            //    new PropertyByName<Order, Language>("Shipping email", async (p, l) => (await orderAddress(p))?.Email ?? string.Empty),
            //    new PropertyByName<Order, Language>("Shipping company", async (p, l) => (await orderAddress(p))?.Company ?? string.Empty, !_addressSettings.CompanyEnabled),
            //    new PropertyByName<Order, Language>("Shipping country", async (p, l) => await _countryService.GetCountryByAddressAsync(await orderAddress(p)) is Country country ? await _localizationService.GetLocalizedAsync(country, c => c.Name) : string.Empty, !_addressSettings.CountryEnabled),
            //    new PropertyByName<Order, Language>("Shipping state province", async (p, l) => await _stateProvinceService.GetStateProvinceByAddressAsync(await orderAddress(p)) is StateProvince stateProvince ? await _localizationService.GetLocalizedAsync(stateProvince, sp => sp.Name) : string.Empty, !_addressSettings.StateProvinceEnabled),
            //    new PropertyByName<Order, Language>("Shipping county", async (p, l) => (await orderAddress(p))?.County ?? string.Empty, !_addressSettings.CountyEnabled),
            //    new PropertyByName<Order, Language>("Shipping city", async (p, l) => (await orderAddress(p))?.City ?? string.Empty, !_addressSettings.CityEnabled),
            //    new PropertyByName<Order, Language>("Shipping address 1", async (p, l) => (await orderAddress(p))?.Address1 ?? string.Empty, !_addressSettings.StreetAddressEnabled),
            //    new PropertyByName<Order, Language>("Shipping address 2", async (p, l) => (await orderAddress(p))?.Address2 ?? string.Empty, !_addressSettings.StreetAddress2Enabled),
            //    new PropertyByName<Order, Language>("Shipping zip postal code",
            //        async (p, l) => (await orderAddress(p))?.ZipPostalCode ?? string.Empty, !_addressSettings.ZipPostalCodeEnabled),
            //    new PropertyByName<Order, Language>("Shipping phone number", async (p, l) => (await orderAddress(p))?.PhoneNumber ?? string.Empty, !_addressSettings.PhoneEnabled),
            //    new PropertyByName<Order, Language>("Shipping fax number", async (p, l) => (await orderAddress(p))?.FaxNumber ?? string.Empty, !_addressSettings.FaxEnabled)
            //}, _portalSettings);

            //var orderItemsManager = new PropertyManager<OrderItem, Language>(new[]
            //{
            //    new PropertyByName<OrderItem, Language>("SKU", async (oi, l) => await _productService.FormatSkuAsync(await _productService.GetProductByIdAsync(oi.ProductId), oi.AttributesXml)),
            //    new PropertyByName<OrderItem, Language>("Name", async (oi, l) => await _localizationService.GetLocalizedAsync(await _productService.GetProductByIdAsync(oi.ProductId), p => p.Name)),
            //    new PropertyByName<OrderItem, Language>("Price", async (oi, l) => await _priceFormatter.FormatPriceAsync(_currencyService.ConvertCurrency((await _orderService.GetOrderByIdAsync(oi.OrderId)).UserTaxDisplayType == TaxDisplayType.IncludingTax ? oi.UnitPriceInclTax : oi.UnitPriceExclTax, (await _orderService.GetOrderByIdAsync(oi.OrderId)).CurrencyRate), true, (await _orderService.GetOrderByIdAsync(oi.OrderId)).UserCurrencyCode, false, currentLanguage.Id)),
            //    new PropertyByName<OrderItem, Language>("Quantity", (oi, l) => oi.Quantity),
            //    new PropertyByName<OrderItem, Language>("Total", async (oi, l) => await _priceFormatter.FormatPriceAsync((await _orderService.GetOrderByIdAsync(oi.OrderId)).UserTaxDisplayType == TaxDisplayType.IncludingTax ? oi.PriceInclTax : oi.PriceExclTax))
            //}, _portalSettings);

            //var orders = await _orderService.SearchOrdersAsync(userId: user.Id);

            //user addresses
            var addressManager = new PropertyManager<Address, Language>(new[]
            {
                new PropertyByName<Address, Language>("First name", (p, l) => p.FirstName),
                new PropertyByName<Address, Language>("Last name", (p, l) => p.LastName),
                new PropertyByName<Address, Language>("Email", (p, l) => p.Email),
                new PropertyByName<Address, Language>("Company", (p, l) => p.Company, !_addressSettings.CompanyEnabled),
                new PropertyByName<Address, Language>("Country", async (p, l) => await _countryService.GetCountryByAddressAsync(p) is Country country ? await _localizationService.GetLocalizedAsync(country, c => c.Name) : string.Empty, !_addressSettings.CountryEnabled),
                new PropertyByName<Address, Language>("State province", async (p, l) => await _stateProvinceService.GetStateProvinceByAddressAsync(p) is StateProvince stateProvince ? await _localizationService.GetLocalizedAsync(stateProvince, sp => sp.Name) : string.Empty, !_addressSettings.StateProvinceEnabled),
                new PropertyByName<Address, Language>("County", (p, l) => p.County, !_addressSettings.CountyEnabled),
                new PropertyByName<Address, Language>("City", (p, l) => p.City, !_addressSettings.CityEnabled),
                new PropertyByName<Address, Language>("Address 1", (p, l) => p.Address1, !_addressSettings.StreetAddressEnabled),
                new PropertyByName<Address, Language>("Address 2", (p, l) => p.Address2, !_addressSettings.StreetAddress2Enabled),
                new PropertyByName<Address, Language>("Zip / postal code", (p, l) => p.ZipPostalCode, !_addressSettings.ZipPostalCodeEnabled),
                new PropertyByName<Address, Language>("Phone number", (p, l) => p.PhoneNumber, !_addressSettings.PhoneEnabled),
                new PropertyByName<Address, Language>("Fax number", (p, l) => p.FaxNumber, !_addressSettings.FaxEnabled),
                new PropertyByName<Address, Language>("Custom attributes", async (p, l) => await _userAttributeFormatter.FormatAttributesAsync(p.CustomAttributes, ";"))
            }, _portalSettings);

            //user private messages
            //var systemMessageManager = new PropertyManager<SystemMessage, Language>(new[]
            //{
            //    new PropertyByName<SystemMessage, Language>("From", async (pm, l) => await _userService.GetUserByIdAsync(pm.FromUserId) is User cFrom ? _userSettings.UsernamesEnabled ? cFrom.Username : cFrom.Email : string.Empty),
            //    new PropertyByName<SystemMessage, Language>("To", async (pm, l) => await _userService.GetUserByIdAsync(pm.ToUserId) is User cTo ? _userSettings.UsernamesEnabled ? cTo.Username : cTo.Email : string.Empty),
            //    new PropertyByName<SystemMessage, Language>("Subject", (pm, l) => pm.Subject),
            //    new PropertyByName<SystemMessage, Language>("Text", (pm, l) => pm.Text),
            //    new PropertyByName<SystemMessage, Language>("Created on", async (pm, l) => (await _dateTimeHelper.ConvertToUserTimeAsync(pm.CreatedOnUtc, DateTimeKind.Utc)).ToString("D"))
            //}, _portalSettings);

            //List<SystemMessage> pmList = null;
            //if (_forumSettings.AllowSystemMessages)
            //{
            //    pmList = (await _forumService.GetAllSystemMessagesAsync(nodeId, user.Id, 0, null, null, null, null)).ToList();
            //    pmList.AddRange((await _forumService.GetAllSystemMessagesAsync(nodeId, 0, user.Id, null, null, null, null)).ToList());
            //}

            //user GDPR logs
            var gdprLogManager = new PropertyManager<GdprLog, Language>(new[]
            {
                new PropertyByName<GdprLog, Language>("Request type", async (log, l) => await _localizationService.GetLocalizedEnumAsync(log.RequestType)),
                new PropertyByName<GdprLog, Language>("Request details", (log, l) => log.RequestDetails),
                new PropertyByName<GdprLog, Language>("Created on", async (log, l) => (await _dateTimeHelper.ConvertToUserTimeAsync(log.CreatedOnUtc, DateTimeKind.Utc)).ToString("D"))
            }, _portalSettings);

            var gdprLog = await _gdprService.GetAllLogAsync(user.Id);

            await using var stream = new MemoryStream();
            // ok, we can run the real code of the sample now
            using (var workbook = new XLWorkbook())
            {
                // uncomment this line if you want the XML written out to the outputDir
                //xlPackage.DebugMode = true; 

                // get handles to the worksheets
                // Worksheet names cannot be more than 31 characters
                var userInfoWorksheet = workbook.Worksheets.Add("User info");
                var fWorksheet = workbook.Worksheets.Add("DataForFilters");
                fWorksheet.Visibility = XLWorksheetVisibility.VeryHidden;

                //user info and user attributes
                var userInfoRow = 2;
                userManager.CurrentObject = user;
                userManager.WriteDefaultCaption(userInfoWorksheet);
                await userManager.WriteDefaultToXlsxAsync(userInfoWorksheet, userInfoRow);

                //user addresses
                if (await _userService.GetAddressesByUserIdAsync(user.Id) is IList<Address> addresses && addresses.Any())
                {
                    userInfoRow += 2;

                    var cell = userInfoWorksheet.Row(userInfoRow).Cell(1);
                    cell.Value = "Address List";
                    userInfoRow += 1;
                    addressManager.SetCaptionStyle(cell);
                    addressManager.WriteDefaultCaption(userInfoWorksheet, userInfoRow);

                    foreach (var userAddress in addresses)
                    {
                        userInfoRow += 1;
                        addressManager.CurrentObject = userAddress;
                        await addressManager.WriteDefaultToXlsxAsync(userInfoWorksheet, userInfoRow);
                    }
                }

                //user orders
                //if (orders.Any())
                //{
                //    var ordersWorksheet = workbook.Worksheets.Add("Orders");

                //    orderManager.WriteDefaultCaption(ordersWorksheet);

                //    var orderRow = 1;

                //    foreach (var order in orders)
                //    {
                //        orderRow += 1;
                //        orderManager.CurrentObject = order;
                //        await orderManager.WriteDefaultToXlsxAsync(ordersWorksheet, orderRow);

                //        //products
                //        var orederItems = await _orderService.GetOrderItemsAsync(order.Id);

                //        if (!orederItems.Any())
                //            continue;

                //        orderRow += 1;

                //        orderItemsManager.WriteDefaultCaption(ordersWorksheet, orderRow, 2);
                //        ordersWorksheet.Row(orderRow).OutlineLevel = 1;
                //        ordersWorksheet.Row(orderRow).Collapse();

                //        foreach (var orederItem in orederItems)
                //        {
                //            orderRow++;
                //            orderItemsManager.CurrentObject = orederItem;
                //            await orderItemsManager.WriteDefaultToXlsxAsync(ordersWorksheet, orderRow, 2, fWorksheet);
                //            ordersWorksheet.Row(orderRow).OutlineLevel = 1;
                //            ordersWorksheet.Row(orderRow).Collapse();
                //        }
                //    }
                //}

                //user private messages
                //if (pmList?.Any() ?? false)
                //{
                //    var systemMessageWorksheet = workbook.Worksheets.Add("Private messages");
                //    systemMessageManager.WriteDefaultCaption(systemMessageWorksheet);

                //    var systemMessageRow = 1;

                //    foreach (var systemMessage in pmList)
                //    {
                //        systemMessageRow += 1;

                //        systemMessageManager.CurrentObject = systemMessage;
                //        await systemMessageManager.WriteDefaultToXlsxAsync(systemMessageWorksheet, systemMessageRow);
                //    }
                //}

                //user GDPR logs
                if (gdprLog.Any())
                {
                    var gdprLogWorksheet = workbook.Worksheets.Add("GDPR requests (log)");
                    gdprLogManager.WriteDefaultCaption(gdprLogWorksheet);

                    var gdprLogRow = 1;

                    foreach (var log in gdprLog)
                    {
                        gdprLogRow += 1;

                        gdprLogManager.CurrentObject = log;
                        await gdprLogManager.WriteDefaultToXlsxAsync(gdprLogWorksheet, gdprLogRow);
                    }
                }

                workbook.SaveAs(stream);
            }

            return stream.ToArray();
        }

        #endregion
    }
}
