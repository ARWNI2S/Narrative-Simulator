//using DragonCorp.Metalink.Core.Domain.Catalog;
namespace ARWNI2S.Portal.Services.Authentication.Wallet
{
    /// <summary>
    /// Represents wallet authentication service implementation
    /// </summary>
    public partial class WalletAuthenticationService : IWalletAuthenticationService
    {
        #region Fields

        private readonly UserSettings _userSettings;
        private readonly WalletAuthenticationSettings _walletAuthenticationSettings;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IWalletExtensionAddonManager _walletExtensionAddonManager;
        private readonly IUserRegistrationService _userRegistrationService;
        private readonly IBlockchainService _blockchainService;
        private readonly ICryptoAddressService _cryptoAddressService;
        private readonly IUserService _userService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILocalizationService _localizationService;
        private readonly IRepository<WalletAuthenticationRecord> _walletAuthenticationRecordRepository;
        private readonly INodeContext _nodeContext;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly LocalizationSettings _localizationSettings;
        private static readonly string[] stringArray = new[] { "Registration is disabled" };

        #endregion

        #region Ctor

        public WalletAuthenticationService(UserSettings userSettings,
            WalletAuthenticationSettings walletAuthenticationSettings,
            IActionContextAccessor actionContextAccessor,
            IWalletExtensionAddonManager walletExtensionAddonManager,
            IUserRegistrationService userRegistrationService,
            IBlockchainService blockchainService,
            ICryptoAddressService cryptoAddressService,
            IUserService userService,
            IEventPublisher eventPublisher,
            IGenericAttributeService genericAttributeService,
            IHttpContextAccessor httpContextAccessor,
            ILocalizationService localizationService,
            IRepository<WalletAuthenticationRecord> walletAuthenticationRecordRepository,
            INodeContext nodeContext,
            IUrlHelperFactory urlHelperFactory,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            LocalizationSettings localizationSettings)
        {
            _userSettings = userSettings;
            _walletAuthenticationSettings = walletAuthenticationSettings;
            _actionContextAccessor = actionContextAccessor;
            _walletExtensionAddonManager = walletExtensionAddonManager;
            _userRegistrationService = userRegistrationService;
            _blockchainService = blockchainService;
            _cryptoAddressService = cryptoAddressService;
            _userService = userService;
            _eventPublisher = eventPublisher;
            _genericAttributeService = genericAttributeService;
            _httpContextAccessor = httpContextAccessor;
            _localizationService = localizationService;
            _walletAuthenticationRecordRepository = walletAuthenticationRecordRepository;
            _nodeContext = nodeContext;
            _urlHelperFactory = urlHelperFactory;
            _workContext = workContext;
            _workflowMessageService = workflowMessageService;
            _localizationSettings = localizationSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Authenticate user with existing associated wallet account
        /// </summary>
        /// <param name="associatedUser">Associated with passed wallet authentication parameters user</param>
        /// <param name="currentLoggedInUser">Current logged-in user</param>
        /// <param name="returnUrl">URL to which the user will return after authentication</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result of an authentication
        /// </returns>
        protected virtual async Task<IActionResult> AuthenticateExistingUserAsync(User associatedUser, User currentLoggedInUser, string returnUrl)
        {
            //log in guest user
            if (currentLoggedInUser == null)
                return await _userRegistrationService.SignInUserAsync(associatedUser, returnUrl, true);

            //account is already assigned to another user
            if (currentLoggedInUser.Id != associatedUser.Id)
                return await ErrorAuthenticationAsync(new[]
                {
                    await _localizationService.GetResourceAsync("Account.AssociatedWalletAuth.AccountAlreadyAssigned")
                }, returnUrl);

            //or the user try to log in as himself. bit weird
            return SuccessfulAuthentication(returnUrl);
        }

        /// <summary>
        /// Authenticate current user and associate new wallet account with user
        /// </summary>
        /// <param name="currentLoggedInUser">Current logged-in user</param>
        /// <param name="parameters">Authentication parameters received from wallet authentication method</param>
        /// <param name="returnUrl">URL to which the user will return after authentication</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result of an authentication
        /// </returns>
        protected virtual async Task<IActionResult> AuthenticateNewUserAsync(User currentLoggedInUser, WalletAuthenticationParameters parameters, string returnUrl)
        {
            //associate wallet account with logged-in user
            if (currentLoggedInUser != null)
            {
                await AssociateWalletAccountWithUserAsync(currentLoggedInUser, parameters);

                return SuccessfulAuthentication(returnUrl);
            }

            //or try to register new user
            if (_userSettings.UserRegistrationType != UserRegistrationType.Disabled)
                return await RegisterNewUserAsync(parameters, returnUrl);

            //registration is disabled
            return await ErrorAuthenticationAsync(stringArray, returnUrl);
        }

        /// <summary>
        /// Register new user
        /// </summary>
        /// <param name="parameters">Authentication parameters received from wallet authentication method</param>
        /// <param name="returnUrl">URL to which the user will return after authentication</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result of an authentication
        /// </returns>
        protected virtual async Task<IActionResult> RegisterNewUserAsync(WalletAuthenticationParameters parameters, string returnUrl)
        {
            //check whether the specified public address has been already registered
            if (await _userService.GetUserByPublicAddressAsync(parameters.PublicAddress) != null)
            {
                var alreadyExistsError = string.Format(await _localizationService.GetResourceAsync("Account.AssociatedWalletAuth.PublicAddressAlreadyExists"),
                    !string.IsNullOrEmpty(parameters.WalletNetworkIdentifier) ? parameters.WalletNetworkIdentifier : parameters.WalletIdentifier);
                return await ErrorAuthenticationAsync(new[] { alreadyExistsError }, returnUrl);
            }

            //registration is approved if validation isn't required
            var registrationIsApproved = _userSettings.UserRegistrationType == UserRegistrationType.Standard ||
                _userSettings.UserRegistrationType == UserRegistrationType.EmailValidation && !_walletAuthenticationSettings.RequireEmailValidation;

            //create registration request
            var user = await _workContext.GetCurrentUserAsync();
            var node = await _nodeContext.GetCurrentNodeAsync();
            var password = await GeneratePasswordAsync();
            var registrationRequest = new UserRegistrationRequest(user,
                string.Empty,
                parameters.PublicAddress,
                password,
                _userSettings.DefaultPasswordFormat,
                node.Id,
                registrationIsApproved,
                true);

            //whether registration request has been completed successfully
            var registrationResult = await _userRegistrationService.RegisterUserAsync(registrationRequest);
            if (!registrationResult.Success)
                return await ErrorAuthenticationAsync(registrationResult.Errors, returnUrl);

            //allow to save other user values by consuming this event
            await _eventPublisher.PublishAsync(new UserAutoRegisteredByWalletExtensionEvent(user, parameters));

            //raise user registered event
            await _eventPublisher.PublishAsync(new UserRegisteredEvent(user));

            //administrator notifications
            if (_userSettings.NotifyNewUserRegistration)
                await _workflowMessageService.SendUserRegisteredNotificationMessageAsync(user, _localizationSettings.DefaultAdminLanguageId);

            //associate wallet account with registered user
            await AssociateWalletAccountWithUserAsync(user, parameters);

            //authenticate
            var currentLanguage = await _workContext.GetWorkingLanguageAsync();
            if (registrationIsApproved)
            {
                await _workflowMessageService.SendUserWelcomeMessageAsync(user, currentLanguage.Id);

                //raise event       
                await _eventPublisher.PublishAsync(new UserActivatedEvent(user));

                return await _userRegistrationService.SignInUserAsync(user, returnUrl, true);
            }

            //registration is succeeded but is by web3 account so redirect to partial registration result (user data incomplete)
            if (registrationRequest.IsWeb3Registration)
                return new RedirectToRouteResult("PartialRegisterResult", new { resultId = (int)UserRegistrationType.EmailValidation, returnUrl });

            //registration is succeeded but isn't activated
            if (_userSettings.UserRegistrationType == UserRegistrationType.EmailValidation)
            {
                //email validation message
                await _genericAttributeService.SaveAttributeAsync(user, UserDefaults.AccountActivationTokenAttribute, Guid.NewGuid().ToString());
                await _workflowMessageService.SendUserEmailValidationMessageAsync(user, currentLanguage.Id);

                return new RedirectToRouteResult("RegisterResult", new { resultId = (int)UserRegistrationType.EmailValidation, returnUrl });
            }

            //registration is succeeded but isn't approved by admin
            if (_userSettings.UserRegistrationType == UserRegistrationType.AdminApproval)
                return new RedirectToRouteResult("RegisterResult", new { resultId = (int)UserRegistrationType.AdminApproval, returnUrl });

            return await ErrorAuthenticationAsync(new[] { await _localizationService.GetResourceAsync("Account.Register.Error") }, returnUrl);
        }

        /// <summary>
        /// Add errors that occurred during authentication
        /// </summary>
        /// <param name="errors">Collection of errors</param>
        /// <param name="returnUrl">URL to which the user will return after authentication</param>
        /// <returns>Result of an authentication</returns>
        protected virtual async Task<IActionResult> ErrorAuthenticationAsync(IEnumerable<string> errors, string returnUrl)
        {
            var session = _httpContextAccessor.HttpContext?.Session;

            if (session != null)
            {
                var existsErrors = (await session.GetAsync<IList<string>>(AuthenticationServicesDefaults.WalletAuthenticationErrorsSessionKey))?.ToList() ?? new List<string>();

                existsErrors.AddRange(errors);

                await session.SetAsync(AuthenticationServicesDefaults.WalletAuthenticationErrorsSessionKey, existsErrors);
            }

            return new RedirectToActionResult("Login", "User", !string.IsNullOrEmpty(returnUrl) ? new { ReturnUrl = returnUrl } : null);
        }

        /// <summary>
        /// Redirect the user after successful authentication
        /// </summary>
        /// <param name="returnUrl">URL to which the user will return after authentication</param>
        /// <returns>Result of an authentication</returns>
        protected virtual IActionResult SuccessfulAuthentication(string returnUrl)
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

            //redirect to the return URL if it's specified
            if (!string.IsNullOrEmpty(returnUrl) && urlHelper.IsLocalUrl(returnUrl))
                return new RedirectResult(returnUrl);

            return new RedirectToRouteResult("Homepage", null);
        }

        protected virtual async Task<string> GeneratePasswordAsync()
        {
            var maxLength = _userSettings.PasswordMaxLength;
            var minLength = _userSettings.PasswordMinLength;
            var requireDigit = _userSettings.PasswordRequireDigit;
            var requireLowercase = _userSettings.PasswordRequireLowercase;
            var requireNonAlphanumeric = _userSettings.PasswordRequireNonAlphanumeric;
            var requireUppercase = _userSettings.PasswordRequireUppercase;

            var password = string.Empty;
            Random rnd = new((int)(DateTime.UtcNow.Ticks & 0xFFFFFFFF));

            return await Task.Run(() =>
            {
                while (password.Length < minLength || password.Length > maxLength ||
                requireDigit && !password.Any(char.IsDigit) ||
                requireLowercase && !password.Any(char.IsLower) ||
                requireNonAlphanumeric && !password.Any(ch => !char.IsLetterOrDigit(ch)) ||
                requireUppercase && !password.Any(char.IsUpper))
                {
                    password = Path.GetRandomFileName();
                    password = password.Replace(".", "")[..rnd.Next(minLength, password.Length >= maxLength ? maxLength : password.Length)];
                }

                return password;
            });
        }

        #endregion

        #region Methods

        #region Authentication

        /// <summary>
        /// Authenticate user by passed parameters
        /// </summary>
        /// <param name="parameters">Wallet authentication parameters</param>
        /// <param name="returnUrl">URL to which the user will return after authentication</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result of an authentication
        /// </returns>
        public virtual async Task<IActionResult> AuthenticateAsync(WalletAuthenticationParameters parameters, string returnUrl = null)
        {
            ArgumentNullException.ThrowIfNull(parameters);

            var user = await _workContext.GetCurrentUserAsync();
            var node = await _nodeContext.GetCurrentNodeAsync();
            if (!await _walletExtensionAddonManager.IsAddonActiveAsync(parameters.ProviderSystemName, user, node.Id))
                return await ErrorAuthenticationAsync(new[] { await _localizationService.GetResourceAsync("Account.WalletAuthenticationMethod.LoadError") }, returnUrl);

            //get current logged-in user
            var currentLoggedInUser = await _userService.IsRegisteredAsync(user) ? user : null;

            //authenticate associated user if already exists
            var associatedUser = await GetUserByWalletAuthenticationParametersAsync(parameters);
            if (associatedUser != null)
                return await AuthenticateExistingUserAsync(associatedUser, currentLoggedInUser, returnUrl);

            //or associate and authenticate new user
            return await AuthenticateNewUserAsync(currentLoggedInUser, parameters, returnUrl);
        }

        #endregion

        /// <summary>
        /// Get the crypto addresses by identifier
        /// </summary>
        /// <param name="walletAuthenticationRecordId">Crypto address identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<WalletAuthenticationRecord> GetWalletAuthenticationRecordByIdAsync(int walletAuthenticationRecordId)
        {
            return await _walletAuthenticationRecordRepository.GetByIdAsync(walletAuthenticationRecordId, cache => default);
        }

        /// <summary>
        /// Get list of the crypto addresses by user
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<IList<WalletAuthenticationRecord>> GetUserWalletAuthenticationRecordsAsync(User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            var walletAuthenticationRecords = _walletAuthenticationRecordRepository.Table.Where(ca => ca.UserId == user.Id);

            return await walletAuthenticationRecords.ToListAsync();
        }

        /// <summary>
        /// Delete the crypto address
        /// </summary>
        /// <param name="walletAuthenticationRecord">Crypto address</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteWalletAuthenticationRecordAsync(WalletAuthenticationRecord walletAuthenticationRecord)
        {
            ArgumentNullException.ThrowIfNull(walletAuthenticationRecord);

            await _walletAuthenticationRecordRepository.DeleteAsync(walletAuthenticationRecord, false);
        }

        /// <summary>
        /// Get the crypto address
        /// </summary>
        /// <param name="parameters">Wallet authentication parameters</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<WalletAuthenticationRecord> GetWalletAuthenticationRecordByWalletAuthenticationParametersAsync(WalletAuthenticationParameters parameters)
        {
            ArgumentNullException.ThrowIfNull(parameters);

            var walletAuthenticationRecord = await _walletAuthenticationRecordRepository.Table.FirstOrDefaultAsync(address =>
                address.WalletIdentifier.Equals(parameters.WalletIdentifier) && address.ProviderSystemName.Equals(parameters.ProviderSystemName));

            return walletAuthenticationRecord;
        }

        /// <summary>
        /// Associate wallet account with user
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="parameters">Wallet authentication parameters</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task AssociateWalletAccountWithUserAsync(User user, WalletAuthenticationParameters parameters)
        {
            ArgumentNullException.ThrowIfNull(user);

            var cryptoAddress = await _cryptoAddressService.GetCryptoAddressAsync(parameters.PublicAddress, userId: user.Id, walletProvider: parameters.WalletIdentifier);

            var blockchain = await _blockchainService.GetBlockchainByWalletNetworkIdentifierAsync(parameters.WalletNetworkIdentifier);

            if (cryptoAddress == null)
            {
                cryptoAddress = new CryptoAddress
                {
                    Address = parameters.PublicAddress,
                    BlockchainId = blockchain != null ? blockchain.Id : 0,
                    UserId = user.Id,
                    WalletProvider = parameters.WalletIdentifier
                };

                await _cryptoAddressService.InsertCryptoAddressAsync(cryptoAddress);

                if (!await _userService.IsPlayerAsync(user))
                    await _eventPublisher.PublishAsync(new NonPlayerUserCryptoAddressAddedEvent(user, cryptoAddress));

            }

            var walletAuthenticationRecord = new WalletAuthenticationRecord
            {
                UserId = user.Id,
                CryptoAddressId = cryptoAddress.Id,
                WalletIdentifier = parameters.WalletIdentifier,
                WalletNetworkIdentifier = parameters.WalletNetworkIdentifier,
                AuthAccessToken = parameters.AccessToken,
                ProviderSystemName = parameters.ProviderSystemName
            };

            await _walletAuthenticationRecordRepository.InsertAsync(walletAuthenticationRecord, false);
        }

        /// <summary>
        /// Get the particular user with specified parameters
        /// </summary>
        /// <param name="parameters">Wallet authentication parameters</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the user
        /// </returns>
        public virtual async Task<User> GetUserByWalletAuthenticationParametersAsync(WalletAuthenticationParameters parameters)
        {
            ArgumentNullException.ThrowIfNull(parameters);

            var walletAuthenticationRecord = await _walletAuthenticationRecordRepository.Table.Where(address =>
                address.WalletIdentifier.Equals(parameters.WalletIdentifier) &&
                address.ProviderSystemName.Equals(parameters.ProviderSystemName))?
                .FirstOrDefaultAwaitAsync(async address => (await _cryptoAddressService.GetCryptoAddressByIdAsync(address.Id)).Address == parameters.PublicAddress);
            if (walletAuthenticationRecord == null)
                return null;

            return await _userService.GetUserByIdAsync(walletAuthenticationRecord.UserId);
        }

        /// <summary>
        /// Remove the association
        /// </summary>
        /// <param name="parameters">Wallet authentication parameters</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task RemoveAssociationAsync(WalletAuthenticationParameters parameters)
        {
            ArgumentNullException.ThrowIfNull(parameters);

            var walletAuthenticationRecord = await _walletAuthenticationRecordRepository.Table.FirstOrDefaultAsync(address =>
                address.WalletIdentifier.Equals(parameters.WalletIdentifier) && address.ProviderSystemName.Equals(parameters.ProviderSystemName));

            if (walletAuthenticationRecord != null)
                await _walletAuthenticationRecordRepository.DeleteAsync(walletAuthenticationRecord, false);
        }


        #endregion
    }
}
