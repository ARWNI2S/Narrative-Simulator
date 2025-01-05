using ARWNI2S.Backend.Services.Users;
using ARWNI2S.Backend.Services.Users.Entities;
using ARWNI2S.Engine.Data;
using ARWNI2S.Engine.Infrastructure;
using ARWNI2S.Environment;
using ARWNI2S.Events;
using ARWNI2S.Framework.Common;
using ARWNI2S.Framework.Localization;
using ARWNI2S.Framework.Localization.Entities;
using LinqToDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace ARWNI2S.Backend.Services.Authentication.External
{
    /// <summary>
    /// Represents external authentication service implementation
    /// </summary>
    public partial class ExternalAuthenticationService : IExternalAuthenticationService
    {
        #region Fields

        protected readonly UserSettings _userSettings;
        protected readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
        protected readonly IActionContextAccessor _actionContextAccessor;
        protected readonly IAuthenticationPluginManager _authenticationPluginManager;
        protected readonly IUserRegistrationService _userRegistrationService;
        protected readonly IUserService _userService;
        protected readonly IEventPublisher _eventPublisher;
        protected readonly IGenericAttributeService _genericAttributeService;
        protected readonly INiisContextAccessor _httpContextAccessor;
        protected readonly ILocalizationService _localizationService;
        protected readonly IRepository<ExternalAuthenticationRecord> _externalAuthenticationRecordRepository;
        protected readonly IStoreContext _storeContext;
        protected readonly IUrlHelperFactory _urlHelperFactory;
        protected readonly IWorkingContext _workingContext;
        protected readonly IWorkflowMessageService _workflowMessageService;
        protected readonly LocalizationSettings _localizationSettings;

        #endregion

        #region Ctor

        public ExternalAuthenticationService(UserSettings userSettings,
            ExternalAuthenticationSettings externalAuthenticationSettings,
            IActionContextAccessor actionContextAccessor,
            IAuthenticationPluginManager authenticationPluginManager,
            IUserRegistrationService userRegistrationService,
            IUserService userService,
            IEventPublisher eventPublisher,
            IGenericAttributeService genericAttributeService,
            INiisContextAccessor httpContextAccessor,
            ILocalizationService localizationService,
            IRepository<ExternalAuthenticationRecord> externalAuthenticationRecordRepository,
            IStoreContext storeContext,
            IUrlHelperFactory urlHelperFactory,
            IWorkingContext workingContext,
            IWorkflowMessageService workflowMessageService,
            LocalizationSettings localizationSettings)
        {
            _userSettings = userSettings;
            _externalAuthenticationSettings = externalAuthenticationSettings;
            _actionContextAccessor = actionContextAccessor;
            _authenticationPluginManager = authenticationPluginManager;
            _userRegistrationService = userRegistrationService;
            _userService = userService;
            _eventPublisher = eventPublisher;
            _genericAttributeService = genericAttributeService;
            _httpContextAccessor = httpContextAccessor;
            _localizationService = localizationService;
            _externalAuthenticationRecordRepository = externalAuthenticationRecordRepository;
            _storeContext = storeContext;
            _urlHelperFactory = urlHelperFactory;
            _workingContext = workingContext;
            _workflowMessageService = workflowMessageService;
            _localizationSettings = localizationSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Authenticate user with existing associated external account
        /// </summary>
        /// <param name="associatedUser">Associated with passed external authentication parameters user</param>
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
                return await _userRegistrationService.SignInUserAsync(associatedUser, returnUrl);

            //account is already assigned to another user
            if (currentLoggedInUser.Id != associatedUser.Id)
                return await ErrorAuthenticationAsync(new[]
                {
                    await _localizationService.GetResourceAsync("Account.AssociatedExternalAuth.AccountAlreadyAssigned")
                }, returnUrl);

            //or the user try to log in as himself. bit weird
            return SuccessfulAuthentication(returnUrl);
        }

        /// <summary>
        /// Authenticate current user and associate new external account with user
        /// </summary>
        /// <param name="currentLoggedInUser">Current logged-in user</param>
        /// <param name="parameters">Authentication parameters received from external authentication method</param>
        /// <param name="returnUrl">URL to which the user will return after authentication</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result of an authentication
        /// </returns>
        protected virtual async Task<IActionResult> AuthenticateNewUserAsync(User currentLoggedInUser, ExternalAuthenticationParameters parameters, string returnUrl)
        {
            //associate external account with logged-in user
            if (currentLoggedInUser != null)
            {
                await AssociateExternalAccountWithUserAsync(currentLoggedInUser, parameters);

                return SuccessfulAuthentication(returnUrl);
            }

            //or try to register new user
            if (_userSettings.UserRegistrationType != UserRegistrationType.Disabled)
                return await RegisterNewUserAsync(parameters, returnUrl);

            //registration is disabled
            return await ErrorAuthenticationAsync(new[] { "Registration is disabled" }, returnUrl);
        }

        /// <summary>
        /// Register new user
        /// </summary>
        /// <param name="parameters">Authentication parameters received from external authentication method</param>
        /// <param name="returnUrl">URL to which the user will return after authentication</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result of an authentication
        /// </returns>
        protected virtual async Task<IActionResult> RegisterNewUserAsync(ExternalAuthenticationParameters parameters, string returnUrl)
        {
            //check whether the specified email has been already registered
            if (await _userService.GetUserByEmailAsync(parameters.Email) != null)
            {
                var alreadyExistsError = string.Format(await _localizationService.GetResourceAsync("Account.AssociatedExternalAuth.EmailAlreadyExists"),
                    !string.IsNullOrEmpty(parameters.ExternalDisplayIdentifier) ? parameters.ExternalDisplayIdentifier : parameters.ExternalIdentifier);
                return await ErrorAuthenticationAsync(new[] { alreadyExistsError }, returnUrl);
            }

            //registration is approved if validation isn't required
            var registrationIsApproved = _userSettings.UserRegistrationType == UserRegistrationType.Standard ||
                                         _userSettings.UserRegistrationType == UserRegistrationType.EmailValidation && !_externalAuthenticationSettings.RequireEmailValidation;

            //create registration request
            var user = await _workingContext.GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var registrationRequest = new UserRegistrationRequest(user,
                parameters.Email, parameters.Email,
                CommonHelper.GenerateRandomDigitCode(20),
                PasswordFormat.Hashed,
                store.Id,
                registrationIsApproved);

            //whether registration request has been completed successfully
            var registrationResult = await _userRegistrationService.RegisterUserAsync(registrationRequest);
            if (!registrationResult.Success)
                return await ErrorAuthenticationAsync(registrationResult.Errors, returnUrl);

            //allow to save other user values by consuming this event
            await _eventPublisher.PublishAsync(new UserAutoRegisteredByExternalMethodEvent(user, parameters));

            //raise user registered event
            await _eventPublisher.PublishAsync(new UserRegisteredEvent(user));

            //store owner notifications
            if (_userSettings.NotifyNewUserRegistration)
                await _workflowMessageService.SendUserRegisteredStoreOwnerNotificationMessageAsync(user, _localizationSettings.DefaultAdminLanguageId);

            //associate external account with registered user
            await AssociateExternalAccountWithUserAsync(user, parameters);

            //authenticate
            var currentLanguage = await _workingContext.GetWorkingLanguageAsync();
            if (registrationIsApproved)
            {
                await _workflowMessageService.SendUserWelcomeMessageAsync(user, currentLanguage.Id);

                //raise event       
                await _eventPublisher.PublishAsync(new UserActivatedEvent(user));

                return await _userRegistrationService.SignInUserAsync(user, returnUrl, true);
            }

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

            return await ErrorAuthenticationAsync(new[] { "Error on registration" }, returnUrl);
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
                var existsErrors = (await session.GetAsync<IList<string>>(AuthenticationDefaults.ExternalAuthenticationErrorsSessionKey))?.ToList() ?? new List<string>();

                existsErrors.AddRange(errors);

                await session.SetAsync(AuthenticationDefaults.ExternalAuthenticationErrorsSessionKey, existsErrors);
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

        #endregion

        #region Methods

        #region Authentication

        /// <summary>
        /// Authenticate user by passed parameters
        /// </summary>
        /// <param name="parameters">External authentication parameters</param>
        /// <param name="returnUrl">URL to which the user will return after authentication</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result of an authentication
        /// </returns>
        public virtual async Task<IActionResult> AuthenticateAsync(ExternalAuthenticationParameters parameters, string returnUrl = null)
        {
            ArgumentNullException.ThrowIfNull(parameters);

            var user = await _workingContext.GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            if (!await _authenticationPluginManager.IsPluginActiveAsync(parameters.ProviderSystemName, user, store.Id))
                return await ErrorAuthenticationAsync(new[] { "External authentication method cannot be loaded" }, returnUrl);

            //get current logged-in user
            var currentLoggedInUser = await _userService.IsRegisteredAsync(user) ? user : null;

            //authenticate associated user if already exists
            var associatedUser = await GetUserByExternalAuthenticationParametersAsync(parameters);
            if (associatedUser != null)
                return await AuthenticateExistingUserAsync(associatedUser, currentLoggedInUser, returnUrl);

            //or associate and authenticate new user
            return await AuthenticateNewUserAsync(currentLoggedInUser, parameters, returnUrl);
        }

        #endregion

        /// <summary>
        /// Get the external authentication records by identifier
        /// </summary>
        /// <param name="externalAuthenticationRecordId">External authentication record identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<ExternalAuthenticationRecord> GetExternalAuthenticationRecordByIdAsync(int externalAuthenticationRecordId)
        {
            return await _externalAuthenticationRecordRepository.GetByIdAsync(externalAuthenticationRecordId, cache => default, useShortTermCache: true);
        }

        /// <summary>
        /// Get list of the external authentication records by user
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<IList<ExternalAuthenticationRecord>> GetUserExternalAuthenticationRecordsAsync(User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            var associationRecords = _externalAuthenticationRecordRepository.Table.Where(ear => ear.UserId == user.Id);

            return await associationRecords.ToListAsync();
        }

        /// <summary>
        /// Delete the external authentication record
        /// </summary>
        /// <param name="externalAuthenticationRecord">External authentication record</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteExternalAuthenticationRecordAsync(ExternalAuthenticationRecord externalAuthenticationRecord)
        {
            ArgumentNullException.ThrowIfNull(externalAuthenticationRecord);

            await _externalAuthenticationRecordRepository.DeleteAsync(externalAuthenticationRecord, false);
        }

        /// <summary>
        /// Get the external authentication record
        /// </summary>
        /// <param name="parameters">External authentication parameters</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<ExternalAuthenticationRecord> GetExternalAuthenticationRecordByExternalAuthenticationParametersAsync(ExternalAuthenticationParameters parameters)
        {
            ArgumentNullException.ThrowIfNull(parameters);

            var associationRecord = await _externalAuthenticationRecordRepository.Table.FirstOrDefaultAsync(record =>
                record.ExternalIdentifier.Equals(parameters.ExternalIdentifier) && record.ProviderSystemName.Equals(parameters.ProviderSystemName));

            return associationRecord;
        }

        /// <summary>
        /// Associate external account with user
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="parameters">External authentication parameters</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task AssociateExternalAccountWithUserAsync(User user, ExternalAuthenticationParameters parameters)
        {
            ArgumentNullException.ThrowIfNull(user);

            var externalAuthenticationRecord = new ExternalAuthenticationRecord
            {
                UserId = user.Id,
                Email = parameters.Email,
                ExternalIdentifier = parameters.ExternalIdentifier,
                ExternalDisplayIdentifier = parameters.ExternalDisplayIdentifier,
                OAuthAccessToken = parameters.AccessToken,
                ProviderSystemName = parameters.ProviderSystemName
            };

            await _externalAuthenticationRecordRepository.InsertAsync(externalAuthenticationRecord, false);
        }

        /// <summary>
        /// Get the particular user with specified parameters
        /// </summary>
        /// <param name="parameters">External authentication parameters</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the user
        /// </returns>
        public virtual async Task<User> GetUserByExternalAuthenticationParametersAsync(ExternalAuthenticationParameters parameters)
        {
            ArgumentNullException.ThrowIfNull(parameters);

            var associationRecord = await _externalAuthenticationRecordRepository.Table.FirstOrDefaultAsync(record =>
                record.ExternalIdentifier.Equals(parameters.ExternalIdentifier) && record.ProviderSystemName.Equals(parameters.ProviderSystemName));
            if (associationRecord == null)
                return null;

            return await _userService.GetUserByIdAsync(associationRecord.UserId);
        }

        /// <summary>
        /// Remove the association
        /// </summary>
        /// <param name="parameters">External authentication parameters</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task RemoveAssociationAsync(ExternalAuthenticationParameters parameters)
        {
            ArgumentNullException.ThrowIfNull(parameters);

            var associationRecord = await _externalAuthenticationRecordRepository.Table.FirstOrDefaultAsync(record =>
                record.ExternalIdentifier.Equals(parameters.ExternalIdentifier) && record.ProviderSystemName.Equals(parameters.ProviderSystemName));

            if (associationRecord != null)
                await _externalAuthenticationRecordRepository.DeleteAsync(associationRecord, false);
        }


        #endregion
    }
}