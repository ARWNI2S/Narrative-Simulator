using ARWNI2S.Engine.Infrastructure;
using ARWNI2S.Environment;
using ARWNI2S.Events;
using ARWNI2S.Framework.Common;
using ARWNI2S.Framework.Localization;
using ARWNI2S.Framework.Security;
using ARWNI2S.Framework.Users.Entities;
using ARWNI2S.Framework.Users.Security;

namespace ARWNI2S.Framework.Users
{
    /// <summary>
    /// User registration service
    /// </summary>
    public partial class UserRegistrationService : IUserRegistrationService
    {
        #region Fields

        protected readonly UserSettings _userSettings;
        //protected readonly IActionContextAccessor _actionContextAccessor;
        protected readonly IAuthenticationService _authenticationService;
        //protected readonly IUserActivityService _userActivityService;
        protected readonly IUserService _userService;
        protected readonly IEncryptionService _encryptionService;
        protected readonly IEventPublisher _eventPublisher;
        protected readonly IGenericAttributeService _genericAttributeService;
        protected readonly ILocalizationService _localizationService;
        protected readonly IMultiFactorAuthenticationPluginManager _multiFactorAuthenticationPluginManager;
        protected readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        protected readonly INotificationService _notificationService;
        protected readonly IPermissionService _permissionService;
        //protected readonly IRewardPointService _rewardPointService;
        //protected readonly IShoppingCartService _shoppingCartService;
        //protected readonly IStoreContext _storeContext;
        //protected readonly IStoreService _storeService;
        //protected readonly IUrlHelperFactory _urlHelperFactory;
        protected readonly IWorkingContext _workingContext;
        protected readonly IWorkflowMessageService _workflowMessageService;
        protected readonly RewardPointsSettings _rewardPointsSettings;

        #endregion

        #region Ctor

        public UserRegistrationService(UserSettings userSettings,
            //IActionContextAccessor actionContextAccessor,
            //IAuthenticationService authenticationService,
            //IUserActivityService userActivityService,
            IUserService userService,
            //IEncryptionService encryptionService,
            IEventPublisher eventPublisher,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            //IMultiFactorAuthenticationPluginManager multiFactorAuthenticationPluginManager,
            //INewsLetterSubscriptionService newsLetterSubscriptionService,
            //INotificationService notificationService,
            //IPermissionService permissionService,
            //IRewardPointService rewardPointService,
            //IShoppingCartService shoppingCartService,
            //IStoreContext storeContext,
            //IStoreService storeService,
            //IUrlHelperFactory urlHelperFactory,
            //IWorkingContext workingContext,
            //IWorkflowMessageService workflowMessageService,
            RewardPointsSettings rewardPointsSettings)
        {
            _userSettings = userSettings;
            //_actionContextAccessor = actionContextAccessor;
            //_authenticationService = authenticationService;
            //_userActivityService = userActivityService;
            _userService = userService;
            //_encryptionService = encryptionService;
            _eventPublisher = eventPublisher;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            //_multiFactorAuthenticationPluginManager = multiFactorAuthenticationPluginManager;
            //_newsLetterSubscriptionService = newsLetterSubscriptionService;
            //_notificationService = notificationService;
            //_permissionService = permissionService;
            //_rewardPointService = rewardPointService;
            //_shoppingCartService = shoppingCartService;
            //_storeContext = storeContext;
            //_storeService = storeService;
            //_urlHelperFactory = urlHelperFactory;
            //_workingContext = workingContext;
            _workflowMessageService = workflowMessageService;
            _rewardPointsSettings = rewardPointsSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Check whether the entered password matches with a saved one
        /// </summary>
        /// <param name="userPassword">User password</param>
        /// <param name="enteredPassword">The entered password</param>
        /// <returns>True if passwords match; otherwise false</returns>
        protected bool PasswordsMatch(UserPassword userPassword, string enteredPassword)
        {
            if (userPassword == null || string.IsNullOrEmpty(enteredPassword))
                return false;

            var savedPassword = string.Empty;
            switch (userPassword.PasswordFormat)
            {
                case PasswordFormat.Clear:
                    savedPassword = enteredPassword;
                    break;
                case PasswordFormat.Encrypted:
                    savedPassword = _encryptionService.EncryptText(enteredPassword);
                    break;
                case PasswordFormat.Hashed:
                    savedPassword = _encryptionService.CreatePasswordHash(enteredPassword, userPassword.PasswordSalt, _userSettings.HashedPasswordFormat);
                    break;
            }

            if (userPassword.Password == null)
                return false;

            return userPassword.Password.Equals(savedPassword);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Validate user
        /// </summary>
        /// <param name="usernameOrEmail">Username or email</param>
        /// <param name="password">Password</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<UserLoginResults> ValidateUserAsync(string usernameOrEmail, string password)
        {
            var user = _userSettings.UsernamesEnabled ?
                await _userService.GetUserByUsernameAsync(usernameOrEmail) :
                await _userService.GetUserByEmailAsync(usernameOrEmail);

            if (user == null)
                return UserLoginResults.UserNotExist;
            if (user.Deleted)
                return UserLoginResults.Deleted;
            if (!user.Active)
                return UserLoginResults.NotActive;
            //only registered can login
            if (!await _userService.IsRegisteredAsync(user))
                return UserLoginResults.NotRegistered;
            //check whether a user is locked out
            if (user.CannotLoginUntilDateUtc.HasValue && user.CannotLoginUntilDateUtc.Value > DateTime.UtcNow)
                return UserLoginResults.LockedOut;

            if (!PasswordsMatch(await _userService.GetCurrentPasswordAsync(user.Id), password))
            {
                //wrong password
                user.FailedLoginAttempts++;
                if (_userSettings.FailedPasswordAllowedAttempts > 0 &&
                    user.FailedLoginAttempts >= _userSettings.FailedPasswordAllowedAttempts)
                {
                    //lock out
                    user.CannotLoginUntilDateUtc = DateTime.UtcNow.AddMinutes(_userSettings.FailedPasswordLockoutMinutes);
                    //reset the counter
                    user.FailedLoginAttempts = 0;
                }

                await _userService.UpdateUserAsync(user);

                return UserLoginResults.WrongPassword;
            }

            var selectedProvider = await _permissionService.AuthorizeAsync(StandardPermission.Security.ENABLE_MULTI_FACTOR_AUTHENTICATION, user)
                ? await _genericAttributeService.GetAttributeAsync<string>(user, UserDefaults.SelectedMultiFactorAuthenticationProviderAttribute)
                : null;
            var store = await _storeContext.GetCurrentStoreAsync();
            var methodIsActive = await _multiFactorAuthenticationPluginManager.IsPluginActiveAsync(selectedProvider, user, store.Id);
            if (methodIsActive)
                return UserLoginResults.MultiFactorAuthenticationRequired;

            if (!string.IsNullOrEmpty(selectedProvider))
                _notificationService.WarningNotification(await _localizationService.GetResourceAsync("MultiFactorAuthentication.Notification.SelectedMethodIsNotActive"));

            //update login details
            user.FailedLoginAttempts = 0;
            user.CannotLoginUntilDateUtc = null;
            user.RequireReLogin = false;
            user.LastLoginDateUtc = DateTime.UtcNow;
            await _userService.UpdateUserAsync(user);

            return UserLoginResults.Successful;
        }

        /// <summary>
        /// Register user
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<UserRegistrationResult> RegisterUserAsync(UserRegistrationRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (request.User == null)
                throw new ArgumentException("Can't load current user");

            var result = new UserRegistrationResult();
            if (request.User.IsSearchEngineAccount())
            {
                result.AddError("Search engine can't be registered");
                return result;
            }

            if (request.User.IsBackgroundTaskAccount())
            {
                result.AddError("Background task account can't be registered");
                return result;
            }

            if (await _userService.IsRegisteredAsync(request.User))
            {
                result.AddError("Current user is already registered");
                return result;
            }

            if (string.IsNullOrEmpty(request.Email))
            {
                result.AddError(await _localizationService.GetResourceAsync("Account.Register.Errors.EmailIsNotProvided"));
                return result;
            }

            if (!CommonHelper.IsValidEmail(request.Email))
            {
                result.AddError(await _localizationService.GetResourceAsync("Common.WrongEmail"));
                return result;
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                result.AddError(await _localizationService.GetResourceAsync("Account.Register.Errors.PasswordIsNotProvided"));
                return result;
            }

            if (_userSettings.UsernamesEnabled && string.IsNullOrEmpty(request.Username))
            {
                result.AddError(await _localizationService.GetResourceAsync("Account.Register.Errors.UsernameIsNotProvided"));
                return result;
            }

            //validate unique user
            if (await _userService.GetUserByEmailAsync(request.Email) != null)
            {
                result.AddError(await _localizationService.GetResourceAsync("Account.Register.Errors.EmailAlreadyExists"));
                return result;
            }

            if (_userSettings.UsernamesEnabled && await _userService.GetUserByUsernameAsync(request.Username) != null)
            {
                result.AddError(await _localizationService.GetResourceAsync("Account.Register.Errors.UsernameAlreadyExists"));
                return result;
            }

            //at this point request is valid
            request.User.Username = request.Username;
            request.User.Email = request.Email;

            var userPassword = new UserPassword
            {
                UserId = request.User.Id,
                PasswordFormat = request.PasswordFormat,
                CreatedOnUtc = DateTime.UtcNow
            };
            switch (request.PasswordFormat)
            {
                case PasswordFormat.Clear:
                    userPassword.Password = request.Password;
                    break;
                case PasswordFormat.Encrypted:
                    userPassword.Password = _encryptionService.EncryptText(request.Password);
                    break;
                case PasswordFormat.Hashed:
                    var saltKey = _encryptionService.CreateSaltKey(UserServicesDefaults.PasswordSaltKeySize);
                    userPassword.PasswordSalt = saltKey;
                    userPassword.Password = _encryptionService.CreatePasswordHash(request.Password, saltKey, _userSettings.HashedPasswordFormat);
                    break;
            }

            await _userService.InsertUserPasswordAsync(userPassword);

            request.User.Active = request.IsApproved;

            //add to 'Registered' role
            var registeredRole = await _userService.GetUserRoleBySystemNameAsync(UserDefaults.RegisteredRoleName) ?? throw new NI2SException("'Registered' role could not be loaded");

            await _userService.AddUserRoleMappingAsync(new UserUserRoleMapping { UserId = request.User.Id, UserRoleId = registeredRole.Id });

            //remove from 'Guests' role            
            if (await _userService.IsGuestAsync(request.User))
            {
                var guestRole = await _userService.GetUserRoleBySystemNameAsync(UserDefaults.GuestsRoleName);
                await _userService.RemoveUserRoleMappingAsync(request.User, guestRole);
            }

            //add reward points for user registration (if enabled)
            if (_rewardPointsSettings.Enabled && _rewardPointsSettings.PointsForRegistration > 0)
            {
                var endDate = _rewardPointsSettings.RegistrationPointsValidity > 0
                    ? (DateTime?)DateTime.UtcNow.AddDays(_rewardPointsSettings.RegistrationPointsValidity.Value) : null;
                await _rewardPointService.AddRewardPointsHistoryEntryAsync(request.User, _rewardPointsSettings.PointsForRegistration,
                    request.StoreId, await _localizationService.GetResourceAsync("RewardPoints.Message.EarnedForRegistration"), endDate: endDate);
            }

            await _userService.UpdateUserAsync(request.User);

            return result;
        }

        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<ChangePasswordResult> ChangePasswordAsync(ChangePasswordRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            var result = new ChangePasswordResult();
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                result.AddError(await _localizationService.GetResourceAsync("Account.ChangePassword.Errors.EmailIsNotProvided"));
                return result;
            }

            if (string.IsNullOrWhiteSpace(request.NewPassword))
            {
                result.AddError(await _localizationService.GetResourceAsync("Account.ChangePassword.Errors.PasswordIsNotProvided"));
                return result;
            }

            var user = await _userService.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                result.AddError(await _localizationService.GetResourceAsync("Account.ChangePassword.Errors.EmailNotFound"));
                return result;
            }

            //request isn't valid
            if (request.ValidateRequest && !PasswordsMatch(await _userService.GetCurrentPasswordAsync(user.Id), request.OldPassword))
            {
                result.AddError(await _localizationService.GetResourceAsync("Account.ChangePassword.Errors.OldPasswordDoesntMatch"));
                return result;
            }

            //check for duplicates
            if (_userSettings.UnduplicatedPasswordsNumber > 0)
            {
                //get some of previous passwords
                var previousPasswords = await _userService.GetUserPasswordsAsync(user.Id, passwordsToReturn: _userSettings.UnduplicatedPasswordsNumber);

                var newPasswordMatchesWithPrevious = previousPasswords.Any(password => PasswordsMatch(password, request.NewPassword));
                if (newPasswordMatchesWithPrevious)
                {
                    result.AddError(await _localizationService.GetResourceAsync("Account.ChangePassword.Errors.PasswordMatchesWithPrevious"));
                    return result;
                }
            }

            //at this point request is valid
            var userPassword = new UserPassword
            {
                UserId = user.Id,
                PasswordFormat = request.NewPasswordFormat,
                CreatedOnUtc = DateTime.UtcNow
            };
            switch (request.NewPasswordFormat)
            {
                case PasswordFormat.Clear:
                    userPassword.Password = request.NewPassword;
                    break;
                case PasswordFormat.Encrypted:
                    userPassword.Password = _encryptionService.EncryptText(request.NewPassword);
                    break;
                case PasswordFormat.Hashed:
                    var saltKey = _encryptionService.CreateSaltKey(UserServicesDefaults.PasswordSaltKeySize);
                    userPassword.PasswordSalt = saltKey;
                    userPassword.Password = _encryptionService.CreatePasswordHash(request.NewPassword, saltKey,
                        request.HashedPasswordFormat ?? _userSettings.HashedPasswordFormat);
                    break;
            }

            await _userService.InsertUserPasswordAsync(userPassword);

            if (user.MustChangePassword)
            {
                user.MustChangePassword = false;
                await _userService.UpdateUserAsync(user);
            }

            //publish event
            await _eventPublisher.PublishAsync(new UserPasswordChangedEvent(userPassword));

            return result;
        }

        /// <summary>
        /// Login passed user
        /// </summary>
        /// <param name="user">User to login</param>
        /// <param name="returnUrl">URL to which the user will return after authentication</param>
        /// <param name="isPersist">Is remember me</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result of an authentication
        /// </returns>
        public virtual async Task<SignInUserResult> SignInUserAsync(User user, string returnUrl, bool isPersist = false)
        {
            var currentUser = await _workingContext.GetCurrentUserAsync();
            if (currentUser?.Id != user.Id)
            {
                if (currentUser.AffiliateId != 0)
                {
                    user.AffiliateId = currentUser.AffiliateId;
                    await _userService.UpdateUserAsync(user);
                }
                //migrate shopping cart
                await _shoppingCartService.MigrateShoppingCartAsync(currentUser, user, true);

                await _workingContext.SetCurrentUserAsync(user);
            }

            //sign in new user
            await _authenticationService.SignInAsync(user, isPersist);

            //raise event       
            await _eventPublisher.PublishAsync(new UserLoggedinEvent(user));

            //activity log
            await _userActivityService.InsertActivityAsync(user, "PublicStore.Login",
                await _localizationService.GetResourceAsync("ActivityLog.PublicStore.Login"), user);

            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

            //redirect to the return URL if it's specified
            if (!string.IsNullOrEmpty(returnUrl) && urlHelper.IsLocalUrl(returnUrl))
                return new RedirectResult(returnUrl);

            return new RedirectToRouteResult("Homepage", null);
        }

        /// <summary>
        /// Sets a user email
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="newEmail">New email</param>
        /// <param name="requireValidation">Require validation of new email address</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task SetEmailAsync(User user, string newEmail, bool requireValidation)
        {
            ArgumentNullException.ThrowIfNull(user);

            if (newEmail == null)
                throw new NI2SException("Email cannot be null");

            newEmail = newEmail.Trim();
            var oldEmail = user.Email;

            if (!CommonHelper.IsValidEmail(newEmail))
                throw new NI2SException(await _localizationService.GetResourceAsync("Account.EmailUsernameErrors.NewEmailIsNotValid"));

            if (newEmail.Length > 100)
                throw new NI2SException(await _localizationService.GetResourceAsync("Account.EmailUsernameErrors.EmailTooLong"));

            var user2 = await _userService.GetUserByEmailAsync(newEmail);
            if (user2 != null && user.Id != user2.Id)
                throw new NI2SException(await _localizationService.GetResourceAsync("Account.EmailUsernameErrors.EmailAlreadyExists"));

            if (requireValidation)
            {
                //re-validate email
                user.EmailToRevalidate = newEmail;
                await _userService.UpdateUserAsync(user);

                //email re-validation message
                await _genericAttributeService.SaveAttributeAsync(user, UserDefaults.EmailRevalidationTokenAttribute, Guid.NewGuid().ToString());
                await _workflowMessageService.SendUserEmailRevalidationMessageAsync(user, (await _workingContext.GetWorkingLanguageAsync()).Id);
            }
            else
            {
                user.Email = newEmail;
                await _userService.UpdateUserAsync(user);

                if (string.IsNullOrEmpty(oldEmail) || oldEmail.Equals(newEmail, StringComparison.InvariantCultureIgnoreCase))
                    return;

                //update newsletter subscription (if required)
                foreach (var store in await _storeService.GetAllStoresAsync())
                {
                    var subscriptionOld = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreIdAsync(oldEmail, store.Id);

                    if (subscriptionOld == null)
                        continue;

                    subscriptionOld.Email = newEmail;
                    await _newsLetterSubscriptionService.UpdateNewsLetterSubscriptionAsync(subscriptionOld);
                }
            }
        }

        /// <summary>
        /// Sets a user username
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="newUsername">New Username</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task SetUsernameAsync(User user, string newUsername)
        {
            ArgumentNullException.ThrowIfNull(user);

            if (!_userSettings.UsernamesEnabled)
                throw new NI2SException("Usernames are disabled");

            newUsername = newUsername.Trim();

            if (newUsername.Length > UserServicesDefaults.UserUsernameLength)
                throw new NI2SException(await _localizationService.GetResourceAsync("Account.EmailUsernameErrors.UsernameTooLong"));

            var user2 = await _userService.GetUserByUsernameAsync(newUsername);
            if (user2 != null && user.Id != user2.Id)
                throw new NI2SException(await _localizationService.GetResourceAsync("Account.EmailUsernameErrors.UsernameAlreadyExists"));

            user.Username = newUsername;
            await _userService.UpdateUserAsync(user);
        }

        #endregion
    }
}