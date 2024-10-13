using ARWNI2S.Node.Data.Entities.Users;
using ARWNI2S.Node.Services.Plugins;
using ARWNI2S.Portal.Services.Entities.Users;
using ARWNI2S.Portal.Services.Users;

namespace ARWNI2S.Portal.Services.Authentication.External
{
    /// <summary>
    /// Represents an authentication addon module implementation
    /// </summary>
    public partial class AuthenticationModuleManager : ModuleManager<IExternalAuthenticationMethod>, IAuthenticationModuleManager
    {
        #region Fields

        private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;

        #endregion

        #region Ctor

        public AuthenticationModuleManager(ExternalAuthenticationSettings externalAuthenticationSettings,
            UserService userService,
            IModuleService addonService) : base(userService, addonService)
        {
            _externalAuthenticationSettings = externalAuthenticationSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load active authentication methods
        /// </summary>
        /// <param name="user">Filter by user; pass null to load all addons</param>
        /// <param name="nodeId">Filter by server; pass 0 to load all addons</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of active authentication methods
        /// </returns>
        public virtual async Task<IList<IExternalAuthenticationMethod>> LoadActiveModulesAsync(User user = null, int nodeId = 0)
        {
            return await LoadActiveModulesAsync(_externalAuthenticationSettings.ActiveAuthenticationMethodSystemNames, user, nodeId);
        }

        /// <summary>
        /// Check whether the passed authentication method is active
        /// </summary>
        /// <param name="authenticationMethod">Authentication method to check</param>
        /// <returns>Result</returns>
        public virtual bool IsModuleActive(IExternalAuthenticationMethod authenticationMethod)
        {
            return IsModuleActive(authenticationMethod, _externalAuthenticationSettings.ActiveAuthenticationMethodSystemNames);
        }

        /// <summary>
        /// Check whether the authentication method with the passed system name is active
        /// </summary>
        /// <param name="systemName">System name of authentication method to check</param>
        /// <param name="user">Filter by user; pass null to load all addons</param>
        /// <param name="nodeId">Filter by server; pass 0 to load all addons</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<bool> IsModuleActiveAsync(string systemName, User user = null, int nodeId = 0)
        {
            var authenticationMethod = await LoadModuleBySystemNameAsync(systemName, user, nodeId);
            return IsModuleActive(authenticationMethod);
        }

        #endregion
    }
}