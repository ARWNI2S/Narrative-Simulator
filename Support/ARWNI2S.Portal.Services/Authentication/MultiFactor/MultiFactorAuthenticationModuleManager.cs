using ARWNI2S.Node.Core.Entities.Users;
using ARWNI2S.Node.Services.Plugins;
using ARWNI2S.Portal.Services.Entities.Users;
using ARWNI2S.Portal.Services.Users;

namespace ARWNI2S.Portal.Services.Authentication.MultiFactor
{
    /// <summary>
    /// Represents an multi-factor authentication module module implementation
    /// </summary>
    public partial class MultiFactorAuthenticationModuleManager : ModuleManager<IMultiFactorAuthenticationMethod>, IMultiFactorAuthenticationModuleManager
    {
        #region Fields

        private readonly MultiFactorAuthenticationSettings _multiFactorAuthenticationSettings;

        #endregion

        #region Ctor

        public MultiFactorAuthenticationModuleManager(MultiFactorAuthenticationSettings multiFactorAuthenticationSettings,
            PortalUserService userService,
            IModuleService moduleService) : base(userService, moduleService)
        {
            _multiFactorAuthenticationSettings = multiFactorAuthenticationSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Check is active multi-factor authentication methods
        /// </summary>
        /// <param name="user">Filter by user; pass null to load all modules</param>
        /// <param name="nodeId">Filter by server; pass 0 to load all modules</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue - if active multi-factor authentication methods
        /// </returns>
        public virtual async Task<bool> HasActiveModulesAsync(User user = null, int nodeId = 0)
        {
            return (await LoadActiveModulesAsync(_multiFactorAuthenticationSettings.ActiveAuthenticationMethodSystemNames, user, nodeId)).Any();
        }

        /// <summary>
        /// Load active multi-factor authentication methods
        /// </summary>
        /// <param name="user">Filter by user; pass null to load all modules</param>
        /// <param name="nodeId">Filter by server; pass 0 to load all modules</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of active multi-factor authentication methods
        /// </returns>
        public virtual async Task<IList<IMultiFactorAuthenticationMethod>> LoadActiveModulesAsync(User user = null, int nodeId = 0)
        {
            return await LoadActiveModulesAsync(_multiFactorAuthenticationSettings.ActiveAuthenticationMethodSystemNames, user, nodeId);
        }

        /// <summary>
        /// Check whether the passed multi-factor authentication method is active
        /// </summary>
        /// <param name="authenticationMethod">Authentication method to check</param>
        /// <returns>Result</returns>
        public virtual bool IsModuleActive(IMultiFactorAuthenticationMethod authenticationMethod)
        {
            return IsModuleActive(authenticationMethod, _multiFactorAuthenticationSettings.ActiveAuthenticationMethodSystemNames);
        }

        /// <summary>
        /// Check whether the multi-factor authentication method with the passed system name is active
        /// </summary>
        /// <param name="systemName">System name of authentication method to check</param>
        /// <param name="user">Filter by user; pass null to load all modules</param>
        /// <param name="nodeId">Filter by server; pass 0 to load all modules</param>
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
