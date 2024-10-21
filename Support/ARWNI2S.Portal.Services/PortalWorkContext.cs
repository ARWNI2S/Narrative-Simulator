using ARWNI2S.Infrastructure.Entities;
using ARWNI2S.Node.Core;
using ARWNI2S.Node.Core.Entities.Clustering;
using ARWNI2S.Node.Core.Entities.Localization;
using ARWNI2S.Node.Core.Entities.Users;
using ARWNI2S.Node.Services.Localization;
using ARWNI2S.Node.Services.Users;
using System.Globalization;

namespace ARWNI2S.Portal.Services
{
    /// <summary>
    /// Represents work context for web application
    /// </summary>
    public partial class PortalWorkContext : IWorkContext
    {
        #region Fields

        //private readonly CookieSettings _cookieSettings;
        //private readonly CurrencySettings _currencySettings;
        //private readonly TokenSettings _tokenSettings;
        //private readonly IAuthenticationService _authenticationService;
        //private readonly ICurrencyService _currencyService;
        //private readonly ITokenService _tokenService;
        private readonly IUserService _userService;
        //private readonly IGenericAttributeService _genericAttributeService;
        //private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILanguageService _languageService;
        //private readonly IBlockchainService _blockchainService;
        private readonly IClusteringContext _nodeContext;
        //private readonly INodeMappingService _nodeMappingService;
        //private readonly IUserAgentHelper _userAgentHelper;
        //private readonly IPlayerService _playerService;
        //private readonly IPartnerService _partnerService;
        //private readonly IWebHelper _webHelper;
        //private readonly PortalLocalizationSettings _localizationSettings;
        //private readonly TaxSettings _taxSettings;

        private CultureInfo _workingCulture;
        private User _cachedUser;
        //private User _originalUserIfImpersonated;
        //private Partner _cachedPartner;
        private Language _cachedLanguage;
        //private Currency _cachedCurrency;
        //private Blockchain _cachedBlockchain;
        //private Token _cachedToken;
        //private TaxDisplayType? _cachedTaxDisplayType;
        //private Player _cachedPlayer;

        #endregion

        #region Ctor

        public PortalWorkContext(
                //CookieSettings cookieSettings,
                //    CurrencySettings currencySettings,
                //    TokenSettings tokenSettings,
                //    IAuthenticationService authenticationService,
                //    ICurrencyService currencyService,
                //    ITokenService tokenService,
                IUserService userService,
                    //    IGenericAttributeService genericAttributeService,
                    //    IHttpContextAccessor httpContextAccessor,
                    ILanguageService languageService,
                //    IBlockchainService blockchainService,
                IClusteringContext nodeContext
            //    INodeMappingService nodeMappingService,
            //    IUserAgentHelper userAgentHelper,
            //    IPartnerService partnerService,
            //    IPlayerService playerService,
            //    IWebHelper webHelper,
            //    PortalLocalizationSettings localizationSettings,
            //    TaxSettings taxSettings
            )
        {
            //    _cookieSettings = cookieSettings;
            //    _currencySettings = currencySettings;
            //    _tokenSettings = tokenSettings;
            //    _authenticationService = authenticationService;
            //    _currencyService = currencyService;
            //    _tokenService = tokenService;
            _userService = userService;
            //    _genericAttributeService = genericAttributeService;
            //    _httpContextAccessor = httpContextAccessor;
            _languageService = languageService;
            //    _blockchainService = blockchainService;
            _nodeContext = nodeContext;
            //    _nodeMappingService = nodeMappingService;
            //    _userAgentHelper = userAgentHelper;
            //    _partnerService = partnerService;
            //    _playerService = playerService;
            //    _webHelper = webHelper;
            //    _localizationSettings = localizationSettings;
            //    _taxSettings = taxSettings;
        }

        #endregion

        #region Utilities

        private async Task<CultureInfo> GetCultureFromLanguageAsync(Language language)
        {
            return await Task.FromResult(CultureInfo.GetCultureInfo(language.LanguageCulture));
        }

        private async Task<Language> GetLanguageFromCultureAsync(CultureInfo cultureInfo)
        {
            var node = (NI2SNode)await _nodeContext.GetCurrentNodeAsync();
            var allNodeLanguages = await _languageService.GetAllLanguagesAsync(nodeId: node.Id);

            //check user language availability
            var detectedLanguage = allNodeLanguages.FirstOrDefault(language => language.LanguageCulture == cultureInfo.Name);

            //it not found, then try to get the default language for the current node (if specified)
            detectedLanguage ??= allNodeLanguages.FirstOrDefault(language => language.Id == node.DefaultLanguageId);

            //if the default language for the current node not found, then try to get the first one
            detectedLanguage ??= allNodeLanguages.FirstOrDefault();

            //if there are no languages for the current node try to get the first one regardless of the node
            detectedLanguage ??= (await _languageService.GetAllLanguagesAsync()).FirstOrDefault();

            return detectedLanguage;
        }

        ///// <summary>
        ///// Get dragoncorp user cookie
        ///// </summary>
        ///// <returns>String value of cookie</returns>
        //protected virtual string GetUserCookie()
        //{
        //    var cookieName = $"{CookieDefaults.Prefix}{CookieDefaults.UserCookie}";
        //    return _httpContextAccessor.HttpContext?.Request?.Cookies[cookieName];
        //}

        ///// <summary>
        ///// Set dragoncorp user cookie
        ///// </summary>
        ///// <param name="userGuid">Guid of the user</param>
        //protected virtual void SetUserCookie(Guid userGuid)
        //{
        //    if (_httpContextAccessor.HttpContext?.Response?.HasStarted ?? true)
        //        return;

        //    //delete current cookie value
        //    var cookieName = $"{CookieDefaults.Prefix}{CookieDefaults.UserCookie}";
        //    _httpContextAccessor.HttpContext.Response.Cookies.Delete(cookieName);

        //    //get date of cookie expiration
        //    var cookieExpires = _cookieSettings.UserCookieExpires;
        //    var cookieExpiresDate = DateTime.Now.AddHours(cookieExpires);

        //    //if passed guid is empty set cookie as expired
        //    if (userGuid == Guid.Empty)
        //        cookieExpiresDate = DateTime.Now.AddMonths(-1);

        //    //set new cookie value
        //    var options = new CookieOptions
        //    {
        //        HttpOnly = true,
        //        Expires = cookieExpiresDate,
        //        Secure = _webHelper.IsCurrentConnectionSecured()
        //    };
        //    _httpContextAccessor.HttpContext.Response.Cookies.Append(cookieName, userGuid.ToString(), options);
        //}

        ///// <summary>
        ///// Set language culture cookie
        ///// </summary>
        ///// <param name="language">Language</param>
        //protected virtual void SetLanguageCookie(Language language)
        //{
        //    if (_httpContextAccessor.HttpContext?.Response?.HasStarted ?? true)
        //        return;

        //    //delete current cookie value
        //    var cookieName = $"{CookieDefaults.Prefix}{CookieDefaults.CultureCookie}";
        //    _httpContextAccessor.HttpContext.Response.Cookies.Delete(cookieName);

        //    if (string.IsNullOrEmpty(language?.LanguageCulture))
        //        return;

        //    //set new cookie value
        //    var value = CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(language.LanguageCulture));
        //    var options = new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) };
        //    _httpContextAccessor.HttpContext.Response.Cookies.Append(cookieName, value, options);
        //}

        ///// <summary>
        ///// Get language from the request
        ///// </summary>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the found language
        ///// </returns>
        //protected virtual async Task<Language> GetLanguageFromRequestAsync()
        //{
        //    var requestCultureFeature = _httpContextAccessor.HttpContext?.Features.Get<IRequestCultureFeature>();
        //    if (requestCultureFeature is null)
        //        return null;

        //    //whether we should detect the current language by user settings
        //    if (requestCultureFeature.Provider is not SeoUrlCultureProvider && !_localizationSettings.AutomaticallyDetectLanguage)
        //        return null;

        //    //get request culture
        //    if (requestCultureFeature.RequestCulture is null)
        //        return null;

        //    //try to get language by culture name
        //    var requestLanguage = (await _languageService.GetAllLanguagesAsync()).FirstOrDefault(language =>
        //        language.LanguageCulture.Equals(requestCultureFeature.RequestCulture.Culture.Name, StringComparison.InvariantCultureIgnoreCase));

        //    //check language availability
        //    if (requestLanguage == null || !requestLanguage.Published || !await _nodeMappingService.AuthorizeAsync(requestLanguage))
        //        return null;

        //    return requestLanguage;
        //}

        #endregion

        #region Methods

        /// <summary>
        /// Gets the current user
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<User> GetCurrentUserAsync()
        {
            //whether there is a cached value
            if (_cachedUser != null)
                return _cachedUser;

            await SetCurrentUserAsync();

            return _cachedUser;
        }

        /// <summary>
        /// Sets the current user
        /// </summary>
        /// <param name="user">Current user</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task SetCurrentUserAsync(User user = null)
        {
            if (user == null)
            {
                ////check whether request is made by a background (schedule) task
                //if (_httpContextAccessor.HttpContext?.Request
                //    ?.Path.Equals(new PathString($"/{TaskServicesDefaults.ScheduleTaskPath}"), StringComparison.InvariantCultureIgnoreCase)
                //    ?? true)
                //{
                //    //in this case return built-in user record for background task
                //    user = await _userService.GetOrCreateBackgroundTaskUserAsync();
                //}

                if (user == null || user.Deleted || !user.Active || user.RequireReLogin)
                {
                    //check whether request is made by a search engine, in this case return built-in user record for search engines
                    //if (_userAgentHelper.IsSearchEngine())
                    //    user = await _userService.GetOrCreateSearchEngineUserAsync();
                }

                if (user == null || user.Deleted || !user.Active || user.RequireReLogin)
                {
                    //try to get registered user
                    //user = await _authenticationService.GetAuthenticatedUserAsync();
                }

                if (user != null && !user.Deleted && user.Active && !user.RequireReLogin)
                {
                    //get impersonate user if required
                    //var impersonatedUserId = await _genericAttributeService
                    //    .GetAttributeAsync<int?>(user, UserDefaults.ImpersonatedUserIdAttribute);
                    //if (impersonatedUserId.HasValue && impersonatedUserId.Value > 0)
                    //{
                    //    var impersonatedUser = await _userService.GetUserByIdAsync(impersonatedUserId.Value);
                    //    if (impersonatedUser != null && !impersonatedUser.Deleted &&
                    //        impersonatedUser.Active &&
                    //        !impersonatedUser.RequireReLogin)
                    //    {
                    //        //set impersonated user
                    //        _originalUserIfImpersonated = user;
                    //        user = impersonatedUser;
                    //    }
                    //}
                }

                if (user == null || user.Deleted || !user.Active || user.RequireReLogin)
                {
                    //get guest user
                    //var userCookie = GetUserCookie();
                    //if (Guid.TryParse(userCookie, out var userGuid))
                    //{
                    //    //get user from cookie (should not be registered)
                    //    var userByCookie = await _userService.GetUserByGuidAsync(userGuid);
                    //    if (userByCookie != null && !await _userService.IsRegisteredAsync(userByCookie))
                    //        user = userByCookie;
                    //}
                }

                if (user == null || user.Deleted || !user.Active || user.RequireReLogin)
                {
                    //create guest if not exists
                    //user = await _userService.InsertGuestUserAsync();
                }
            }

            if (!user.Deleted && user.Active && !user.RequireReLogin)
            {
                //set user cookie
                //SetUserCookie(user.UserGuid);

                //cache the found user
                _cachedUser = user;
            }

            await Task.CompletedTask;
        }

        public async Task<CultureInfo> GetWorkingCultureAsync()
        {
            _workingCulture ??= await GetCultureFromLanguageAsync(await GetWorkingLanguageAsync());
            return _workingCulture;
        }


        public async Task SetWorkingCultureAsync(CultureInfo cultureInfo)
        {
            await SetWorkingLanguageAsync(await GetLanguageFromCultureAsync(cultureInfo));
            _workingCulture = null;
        }


        /// <summary>
        /// Sets current user working language
        /// </summary>
        /// <param name="language">Language</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task SetWorkingLanguageAsync(Language language)
        {
            //save passed language identifier
            var user = await GetCurrentUserAsync();
            user.LanguageId = language?.Id;
            await _userService.UpdateUserAsync(user);

            //set cookie
            //SetLanguageCookie(language);

            //then reset the cached value
            _cachedLanguage = null;
        }

        /// <summary>
        /// Gets current user working language
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<Language> GetWorkingLanguageAsync()
        {
            //whether there is a cached value
            if (_cachedLanguage != null)
                return _cachedLanguage;

            var user = await GetCurrentUserAsync();
            var node = (NI2SNode)await _nodeContext.GetCurrentNodeAsync();

            //whether we should detect the language from the request
            //var detectedLanguage = await GetLanguageFromRequestAsync();

            //get current saved language identifier
            var currentLanguageId = user.LanguageId;

            //if the language is detected we need to save it
            //if (detectedLanguage != null)
            //{
            //    //save the detected language identifier if it differs from the current one
            //    if (detectedLanguage.Id != currentLanguageId)
            //        await SetWorkingLanguageAsync(detectedLanguage);
            //}
            //else
            //{
            var allNodeLanguages = await _languageService.GetAllLanguagesAsync(nodeId: node.Id);

            //check user language availability
            var detectedLanguage = allNodeLanguages.FirstOrDefault(language => language.Id == currentLanguageId);

            //it not found, then try to get the default language for the current node (if specified)
            detectedLanguage ??= allNodeLanguages.FirstOrDefault(language => language.Id == node.DefaultLanguageId);

            //if the default language for the current node not found, then try to get the first one
            detectedLanguage ??= allNodeLanguages.FirstOrDefault();

            //if there are no languages for the current node try to get the first one regardless of the node
            detectedLanguage ??= (await _languageService.GetAllLanguagesAsync()).FirstOrDefault();

            //SetLanguageCookie(detectedLanguage);
            //}

            //cache the found language
            _cachedLanguage = detectedLanguage;

            return _cachedLanguage;
        }

        ///// <summary>
        ///// Gets current user working currency
        ///// </summary>
        ///// <returns>A task that represents the asynchronous operation</returns>
        //public virtual async Task<Currency> GetWorkingCurrencyAsync()
        //{
        //    //whether there is a cached value
        //    if (_cachedCurrency != null)
        //        return _cachedCurrency;

        //    var adminAreaUrl = $"{_webHelper.GetNodeLocation()}admin";

        //    //return primary node currency when we're in admin area/mode
        //    if (_webHelper.GetThisPageUrl(false).StartsWith(adminAreaUrl, StringComparison.InvariantCultureIgnoreCase))
        //    {
        //        var primaryNodeCurrency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryNodeCurrencyId);
        //        if (primaryNodeCurrency != null)
        //        {
        //            _cachedCurrency = primaryNodeCurrency;
        //            return primaryNodeCurrency;
        //        }
        //    }

        //    var user = await GetCurrentUserAsync();
        //    var node = await _nodeContext.GetCurrentNodeAsync();

        //    if (user.IsSearchEngineAccount())
        //    {
        //        _cachedCurrency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryNodeCurrencyId)
        //            ?? (await _currencyService.GetAllCurrenciesAsync(nodeId: node.Id)).FirstOrDefault();

        //        return _cachedCurrency;
        //    }

        //    var allNodeCurrencies = await _currencyService.GetAllCurrenciesAsync(nodeId: node.Id);

        //    //check user currency availability
        //    var userCurrency = allNodeCurrencies.FirstOrDefault(currency => currency.Id == user.CurrencyId);
        //    if (userCurrency == null)
        //    {
        //        //it not found, then try to get the default currency for the current language (if specified)
        //        var language = await GetWorkingLanguageAsync();
        //        userCurrency = allNodeCurrencies
        //            .FirstOrDefault(currency => currency.Id == language.DefaultCurrencyId);
        //    }

        //    //if the default currency for the current node not found, then try to get the first one
        //    userCurrency ??= allNodeCurrencies.FirstOrDefault();

        //    //if there are no currencies for the current node try to get the first one regardless of the node
        //    userCurrency ??= (await _currencyService.GetAllCurrenciesAsync()).FirstOrDefault();

        //    //cache the found currency
        //    _cachedCurrency = userCurrency;

        //    return _cachedCurrency;
        //}

        ///// <summary>
        ///// Sets current user working currency
        ///// </summary>
        ///// <param name="currency">Currency</param>
        ///// <returns>A task that represents the asynchronous operation</returns>
        //public virtual async Task SetWorkingCurrencyAsync(Currency currency)
        //{
        //    //save passed currency identifier
        //    var user = await GetCurrentUserAsync();
        //    if (user.IsSearchEngineAccount())
        //        return;

        //    user.CurrencyId = currency?.Id;
        //    await _userService.UpdateUserAsync(user);

        //    //then reset the cached value
        //    _cachedCurrency = null;
        //}

        ///// <summary>
        ///// Sets current user working blockchain
        ///// </summary>
        ///// <param name="blockchain">Language</param>
        ///// <returns>A task that represents the asynchronous operation</returns>
        //public virtual async Task SetWorkingBlockchainAsync(Blockchain blockchain)
        //{
        //    //URGENT USER ACTIVE OR PREFERRED BLOCKCHAIN
        //    //save passed blockchain identifier
        //    var user = await GetCurrentUserAsync();
        //    //user.BlockchainId = blockchain?.Id;
        //    await _userService.UpdateUserAsync(user);

        //    //URGENT SET WALLET? WALLET COOKIE?
        //    //set cookie
        //    //SetBlockchainCookie(blockchain);

        //    //then reset the cached value
        //    _cachedBlockchain = null;
        //}

        ///// <summary>
        ///// Gets current user working blockchain
        ///// </summary>
        ///// <returns>A task that represents the asynchronous operation</returns>
        //public virtual async Task<Blockchain> GetWorkingBlockchainAsync()
        //{
        //    //whether there is a cached value
        //    if (_cachedBlockchain != null)
        //        return _cachedBlockchain;

        //    var user = await GetCurrentUserAsync();
        //    var node = await _nodeContext.GetCurrentNodeAsync();


        //    //whether we should detect the blockchain from the request
        //    //URGENT GET FROM CONNECTED WALLET
        //    //var detectedBlockchain = await GetBlockchainFromRequestAsync();

        //    //get current saved blockchain identifier
        //    //URGENT GET FROM USER
        //    //var currentBlockchainId = user.BlockchainId;

        //    //if the blockchain is detected we need to save it
        //    //if (detectedBlockchain != null)
        //    //{
        //    //    //save the detected blockchain identifier if it differs from the current one
        //    //    if (detectedBlockchain.Id != currentBlockchainId)
        //    //        await SetWorkingBlockchainAsync(detectedBlockchain);
        //    //}
        //    //else
        //    //{
        //    var allNodeBlockchains = await _blockchainService.GetAllBlockchainsAsync(nodeId: node.Id);

        //    //    //check user blockchain availability
        //    //    detectedBlockchain = allNodeBlockchains.FirstOrDefault(blockchain => blockchain.Id == currentBlockchainId);
        //    var selectedBlockchain = allNodeBlockchains.FirstOrDefault(blockchain => blockchain.Id == 1);

        //    //    //it not found, then try to get the default blockchain for the current node (if specified)
        //    //    detectedBlockchain ??= allNodeBlockchains.FirstOrDefault(blockchain => blockchain.Id == node.DefaultBlockchainId);

        //    //    //if the default blockchain for the current node not found, then try to get the first one
        //    //    detectedBlockchain ??= allNodeBlockchains.FirstOrDefault();

        //    //    //if there are no blockchains for the current node try to get the first one regardless of the node
        //    //    detectedBlockchain ??= (await _blockchainService.GetAllBlockchainsAsync()).FirstOrDefault();

        //    //    SetBlockchainCookie(detectedBlockchain);
        //    //}

        //    ////cache the found blockchain
        //    //_cachedBlockchain = detectedBlockchain;
        //    _cachedBlockchain = selectedBlockchain;
        //    return _cachedBlockchain;
        //}


        ///// <summary>
        ///// Gets current user working token
        ///// </summary>
        ///// <returns>A task that represents the asynchronous operation</returns>
        //public virtual async Task<Token> GetWorkingTokenAsync()
        //{
        //    //whether there is a cached value
        //    if (_cachedToken != null)
        //        return _cachedToken;

        //    var adminAreaUrl = $"{_webHelper.GetNodeLocation()}admin";

        //    //return primary node token when we're in admin area/mode
        //    if (_webHelper.GetThisPageUrl(false).StartsWith(adminAreaUrl, StringComparison.InvariantCultureIgnoreCase))
        //    {
        //        var primaryNodeToken = await _tokenService.GetTokenByIdAsync(_tokenSettings.PrimaryNodeTokenId);
        //        if (primaryNodeToken != null)
        //        {
        //            _cachedToken = primaryNodeToken;
        //            return primaryNodeToken;
        //        }
        //    }

        //    var user = await GetCurrentUserAsync();
        //    var node = await _nodeContext.GetCurrentNodeAsync();

        //    if (user.IsSearchEngineAccount())
        //    {
        //        _cachedToken = await _tokenService.GetTokenByIdAsync(_tokenSettings.PrimaryNodeTokenId)
        //            ?? (await _tokenService.GetAllTokensAsync(nodeId: node.Id)).FirstOrDefault();

        //        return _cachedToken;
        //    }

        //    var allNodeTokens = await _tokenService.GetAllTokensAsync(nodeId: node.Id);

        //    //check user token availability
        //    var userToken = allNodeTokens.FirstOrDefault(token => token.Id == user.TokenId);
        //    if (userToken == null)
        //    {
        //        //it not found, then try to get the default token for the current blockchain (if specified)
        //        var blockchain = await GetWorkingBlockchainAsync();
        //        userToken = allNodeTokens
        //            .FirstOrDefault(token => token.Id == blockchain.PrimaryTokenId);
        //    }

        //    //if the default token for the current node not found, then try to get the first one
        //    userToken ??= allNodeTokens.FirstOrDefault();

        //    //if there are no currencies for the current node try to get the first one regardless of the node
        //    userToken ??= (await _tokenService.GetAllTokensAsync()).FirstOrDefault();

        //    //cache the found token
        //    _cachedToken = userToken;

        //    return _cachedToken;
        //}

        ///// <summary>
        ///// Sets current user working token
        ///// </summary>
        ///// <param name="token">Token</param>
        ///// <returns>A task that represents the asynchronous operation</returns>
        //public virtual async Task SetWorkingTokenAsync(Token token)
        //{
        //    //save passed token identifier
        //    var user = await GetCurrentUserAsync();
        //    if (user.IsSearchEngineAccount())
        //        return;

        //    user.TokenId = token?.Id;
        //    await _userService.UpdateUserAsync(user);

        //    //then reset the cached value
        //    _cachedToken = null;
        //}

        ///// <summary>
        ///// Gets or sets current tax display type
        ///// </summary>
        ///// <returns>A task that represents the asynchronous operation</returns>
        //public virtual async Task<TaxDisplayType> GetTaxDisplayTypeAsync()
        //{
        //    //whether there is a cached value
        //    if (_cachedTaxDisplayType.HasValue)
        //        return _cachedTaxDisplayType.Value;

        //    var taxDisplayType = TaxDisplayType.IncludingTax;
        //    var user = await GetCurrentUserAsync();

        //    //whether users are allowed to select tax display type
        //    if (_taxSettings.AllowUsersToSelectTaxDisplayType && user != null)
        //    {
        //        //try to get previously saved tax display type
        //        var taxDisplayTypeId = user.TaxDisplayTypeId;
        //        if (taxDisplayTypeId.HasValue)
        //            taxDisplayType = (TaxDisplayType)taxDisplayTypeId.Value;
        //        else
        //        {
        //            //default tax type by user roles
        //            var defaultRoleTaxDisplayType = await _userService.GetUserDefaultTaxDisplayTypeAsync(user);
        //            if (defaultRoleTaxDisplayType != null)
        //                taxDisplayType = defaultRoleTaxDisplayType.Value;
        //        }
        //    }
        //    else
        //    {
        //        //default tax type by user roles
        //        var defaultRoleTaxDisplayType = await _userService.GetUserDefaultTaxDisplayTypeAsync(user);
        //        if (defaultRoleTaxDisplayType != null)
        //            taxDisplayType = defaultRoleTaxDisplayType.Value;
        //        else
        //        {
        //            //or get the default tax display type
        //            taxDisplayType = _taxSettings.TaxDisplayType;
        //        }
        //    }

        //    //cache the value
        //    _cachedTaxDisplayType = taxDisplayType;

        //    return _cachedTaxDisplayType.Value;
        //}

        ///// <returns>A task that represents the asynchronous operation</returns>
        //public virtual async Task SetTaxDisplayTypeAsync(TaxDisplayType taxDisplayType)
        //{
        //    //whether users are allowed to select tax display type
        //    if (!_taxSettings.AllowUsersToSelectTaxDisplayType)
        //        return;

        //    //save passed value
        //    var user = await GetCurrentUserAsync();
        //    user.TaxDisplayType = taxDisplayType;
        //    await _userService.UpdateUserAsync(user);

        //    //then reset the cached value
        //    _cachedTaxDisplayType = null;
        //}

        ///// <summary>
        ///// Gets or sets value indicating whether we're in admin area
        ///// </summary>
        //public virtual bool IsAdmin { get; set; }

        #endregion

        async Task<INI2SUser> IWorkContext.GetCurrentUserAsync() { return await GetCurrentUserAsync(); }

        async Task IWorkContext.SetCurrentUserAsync(INI2SUser user)
        {
            if (user is User u)
                await SetCurrentUserAsync(u);
        }

    }
}