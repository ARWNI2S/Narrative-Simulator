using ARWNI2S.Node.Data.Entities.Users;
using ARWNI2S.Node.Services.Plugins;

namespace ARWNI2S.Portal.Services.Authentication.MultiFactor
{
    /// <summary>
    /// Represents an multi-factor authentication addon module
    /// </summary>
    public partial interface IMultiFactorAuthenticationModuleManager : IModuleManager<IMultiFactorAuthenticationMethod>
    {
        /// <summary>
        /// Check is active multi-factor authentication methods
        /// </summary>
        /// <param name="user">Filter by user; pass null to load all addons</param>
        /// <param name="nodeId">Filter by server; pass 0 to load all addons</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue - if active multi-factor authentication methods
        /// </returns>
        Task<bool> HasActiveModulesAsync(User user = null, int nodeId = 0);

        /// <summary>
        /// Load active multi-factor authentication methods
        /// </summary>
        /// <param name="user">Filter by user; pass null to load all addons</param>
        /// <param name="nodeId">Filter by server; pass 0 to load all addons</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of active multi-factor authentication methods
        /// </returns>
        Task<IList<IMultiFactorAuthenticationMethod>> LoadActiveModulesAsync(User user = null, int nodeId = 0);

        /// <summary>
        /// Check whether the passed multi-factor authentication method is active
        /// </summary>
        /// <param name="authenticationMethod">Multi-factor authentication method to check</param>
        /// <returns>Result</returns>
        bool IsModuleActive(IMultiFactorAuthenticationMethod authenticationMethod);

        /// <summary>
        /// Check whether the multi-factor authentication method with the passed system name is active
        /// </summary>
        /// <param name="systemName">System name of multi-factor authentication method to check</param>
        /// <param name="user">Filter by user; pass null to load all addons</param>
        /// <param name="nodeId">Filter by server; pass 0 to load all addons</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<bool> IsModuleActiveAsync(string systemName, User user = null, int nodeId = 0);
    }
}
