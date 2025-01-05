using System.Security.Claims;
using ARWNI2S.Backend.Services.Users;
using ARWNI2S.Backend.Services.Users.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace ARWNI2S.Backend.Services.Authentication
{
    /// <summary>
    /// Represents service using cookie middleware for the authentication
    /// </summary>
    public partial class CookieAuthenticationService : IAuthenticationService
    {
        #region Fields

        protected readonly UserSettings _userSettings;
        protected readonly IUserService _userService;
        protected readonly INiisContextAccessor _httpContextAccessor;

        protected User _cachedUser;

        #endregion

        #region Ctor

        public CookieAuthenticationService(UserSettings userSettings,
            IUserService userService,
            INiisContextAccessor httpContextAccessor)
        {
            _userSettings = userSettings;
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sign in
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="isPersistent">Whether the authentication session is persisted across multiple requests</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task SignInAsync(User user, bool isPersistent)
        {
            ArgumentNullException.ThrowIfNull(user);

            //create claims for user's username and email
            var claims = new List<Claim>();

            if (!string.IsNullOrEmpty(user.Username))
                claims.Add(new Claim(ClaimTypes.Name, user.Username, ClaimValueTypes.String, AuthenticationDefaults.ClaimsIssuer));

            if (!string.IsNullOrEmpty(user.Email))
                claims.Add(new Claim(ClaimTypes.Email, user.Email, ClaimValueTypes.Email, AuthenticationDefaults.ClaimsIssuer));

            //create principal for the current authentication scheme
            var userIdentity = new ClaimsIdentity(claims, AuthenticationDefaults.AuthenticationScheme);
            var userPrincipal = new ClaimsPrincipal(userIdentity);

            //set value indicating whether session is persisted and the time at which the authentication was issued
            var authenticationProperties = new AuthenticationProperties
            {
                IsPersistent = isPersistent,
                IssuedUtc = DateTime.UtcNow
            };

            //sign in
            await _httpContextAccessor.HttpContext.SignInAsync(AuthenticationDefaults.AuthenticationScheme, userPrincipal, authenticationProperties);

            //cache authenticated user
            _cachedUser = user;
        }

        /// <summary>
        /// Sign out
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task SignOutAsync()
        {
            //reset cached user
            _cachedUser = null;

            //and sign out from the current authentication scheme
            await _httpContextAccessor.HttpContext.SignOutAsync(AuthenticationDefaults.AuthenticationScheme);
        }

        /// <summary>
        /// Get authenticated user
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the user
        /// </returns>
        public virtual async Task<User> GetAuthenticatedUserAsync()
        {
            //whether there is a cached user
            if (_cachedUser != null)
                return _cachedUser;

            //try to get authenticated user identity
            var authenticateResult = await _httpContextAccessor.HttpContext.AuthenticateAsync(AuthenticationDefaults.AuthenticationScheme);
            if (!authenticateResult.Succeeded)
                return null;

            User user = null;
            if (_userSettings.UsernamesEnabled)
            {
                //try to get user by username
                var usernameClaim = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Name
                    && claim.Issuer.Equals(AuthenticationDefaults.ClaimsIssuer, StringComparison.InvariantCultureIgnoreCase));
                if (usernameClaim != null)
                    user = await _userService.GetUserByUsernameAsync(usernameClaim.Value);
            }
            else
            {
                //try to get user by email
                var emailClaim = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Email
                                                                                 && claim.Issuer.Equals(AuthenticationDefaults.ClaimsIssuer, StringComparison.InvariantCultureIgnoreCase));
                if (emailClaim != null)
                    user = await _userService.GetUserByEmailAsync(emailClaim.Value);
            }

            //whether the found user is available
            if (user == null || !user.Active || user.RequireReLogin || user.Deleted || !await _userService.IsRegisteredAsync(user))
                return null;

            static DateTime trimMilliseconds(DateTime dt) => new(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, 0, dt.Kind);

            //get the latest password
            var userPassword = await _userService.GetCurrentPasswordAsync(user.Id);
            //require a user to re-login after password changing
            var isPasswordChange = trimMilliseconds(userPassword.CreatedOnUtc).CompareTo(trimMilliseconds(authenticateResult.Properties.IssuedUtc?.DateTime ?? DateTime.UtcNow)) > 0;
            if (_userSettings.RequiredReLoginAfterPasswordChange && isPasswordChange)
                return null;

            //cache authenticated user
            _cachedUser = user;

            return _cachedUser;
        }

        #endregion
    }
}