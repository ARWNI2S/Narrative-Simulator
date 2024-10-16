using ARWNI2S.Infrastructure;
using ARWNI2S.Node.Services.Localization;
using ARWNI2S.Portal.Services.Entities.Common;
using ARWNI2S.Portal.Services.Rss;
using Microsoft.Net.Http.Headers;

namespace ARWNI2S.Portal.Services.Common
{
    /// <summary>
    /// Represents the HTTP client to request dragonCorp official site
    /// </summary>
    public partial class MetalinkHttpClient
    {
        #region Fields

        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILanguageService _languageService;
        private readonly IWebHelper _webHelper;
        private readonly PortalWorkContext _workContext;

        #endregion

        #region Ctor

        public MetalinkHttpClient(AdminAreaSettings adminAreaSettings,
            HttpClient client,
            IHttpContextAccessor httpContextAccessor,
            ILanguageService languageService,
            IWebHelper webHelper,
            PortalWorkContext workContext)
        {
            //configure client
            client.BaseAddress = new Uri("https://www.dragoncorp.org/");
            client.Timeout = TimeSpan.FromSeconds(5);
            client.DefaultRequestHeaders.Add(HeaderNames.UserAgent, $"dragonCorp-{NI2SVersion.CURRENT_VERSION}");

            _adminAreaSettings = adminAreaSettings;
            _httpClient = client;
            _httpContextAccessor = httpContextAccessor;
            _languageService = languageService;
            _webHelper = webHelper;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Check whether the site is available
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the asynchronous task whose result determines that request is completed
        /// </returns>
        public virtual async Task PingAsync()
        {
            await _httpClient.GetStringAsync("/");
        }

        /// <summary>
        /// Get official news RSS
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the asynchronous task whose result contains news RSS feed
        /// </returns>
        public virtual async Task<RssFeed> GetNewsRssAsync()
        {
            //prepare URL to request
            var language = _languageService.GetTwoLetterIsoLanguageName(await _workContext.GetWorkingLanguageAsync());
            var url = string.Format(CommonServicesDefaults.DraCoNewsRssPath,
                NI2SVersion.FULL_VERSION,
                _webHelper.IsLocalRequest(_httpContextAccessor.HttpContext.Request),
                _adminAreaSettings.HideAdvertisementsOnAdminArea,
                _webHelper.GetNodeLocation(),
                language).ToLowerInvariant();

            //get news feed
            await using var stream = await _httpClient.GetStreamAsync(url);
            return await RssFeed.LoadAsync(stream);
        }

        ///// <summary>
        ///// Notification about the successful installation
        ///// </summary>
        ///// <param name="email">Admin email</param>
        ///// <param name="languageCode">Language code</param>
        ///// <param name="culture">Culture name</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the asynchronous task whose result contains the result string
        ///// </returns>
        //public virtual async Task<string> InstallationCompletedAsync(string email, string languageCode, string culture)
        //{
        //    //prepare URL to request
        //    var url = string.Format(CommonServicesDefaults.DraCoInstallationCompletedPath,
        //        NI2SVersion.FULL_VERSION,
        //        _webHelper.IsLocalRequest(_httpContextAccessor.HttpContext.Request),
        //        WebUtility.UrlEncode(email),
        //        _webHelper.GetNodeLocation(),
        //        languageCode,
        //        culture)
        //        .ToLowerInvariant();

        //    //this request takes some more time
        //    _httpClient.Timeout = TimeSpan.FromSeconds(30);

        //    return await _httpClient.GetStringAsync(url);
        //}

        ///// <summary>
        ///// Get a response regarding available categories of marketplace extensions
        ///// </summary>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the asynchronous task whose result contains the result string
        ///// </returns>
        //public virtual async Task<string> GetExtensionsCategoriesAsync()
        //{
        //    //prepare URL to request
        //    var language = _languageService.GetTwoLetterIsoLanguageName(await _workContext.GetWorkingLanguageAsync());
        //    var url = string.Format(CommonServicesDefaults.DraCoExtensionsCategoriesPath, language).ToLowerInvariant();

        //    //get XML response
        //    return await _httpClient.GetStringAsync(url);
        //}

        ///// <summary>
        ///// Get a response regarding available versions of marketplace extensions
        ///// </summary>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the asynchronous task whose result contains the result string
        ///// </returns>
        //public virtual async Task<string> GetExtensionsVersionsAsync()
        //{
        //    //prepare URL to request
        //    var language = _languageService.GetTwoLetterIsoLanguageName(await _workContext.GetWorkingLanguageAsync());
        //    var url = string.Format(CommonServicesDefaults.DraCoExtensionsVersionsPath, language).ToLowerInvariant();

        //    //get XML response
        //    return await _httpClient.GetStringAsync(url);
        //}

        ///// <summary>
        ///// Get a response regarding marketplace extensions
        ///// </summary>
        ///// <param name="categoryId">Category identifier</param>
        ///// <param name="versionId">Version identifier</param>
        ///// <param name="price">Price; 0 - all, 10 - free, 20 - paid</param>
        ///// <param name="searchTerm">Search term</param>
        ///// <param name="pageIndex">Page index</param>
        ///// <param name="pageSize">Page size</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the asynchronous task whose result contains the result string
        ///// </returns>
        //public virtual async Task<string> GetExtensionsAsync(int categoryId = 0,
        //    int versionId = 0, int price = 0, string searchTerm = null,
        //    int pageIndex = 0, int pageSize = int.MaxValue)
        //{
        //    //prepare URL to request
        //    var language = _languageService.GetTwoLetterIsoLanguageName(await _workContext.GetWorkingLanguageAsync());
        //    var url = string.Format(CommonServicesDefaults.DraCoExtensionsPath,
        //        categoryId, versionId, price, WebUtility.UrlEncode(searchTerm), pageIndex, pageSize, language).ToLowerInvariant();

        //    //get XML response
        //    return await _httpClient.GetStringAsync(url);
        //}

        #endregion
    }
}