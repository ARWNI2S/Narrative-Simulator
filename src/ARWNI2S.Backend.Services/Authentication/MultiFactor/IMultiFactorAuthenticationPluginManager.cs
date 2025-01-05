﻿using ARWNI2S.Backend.Services.Users.Entities;
using ARWNI2S.Engine.Plugins;

namespace ARWNI2S.Backend.Services.Authentication.MultiFactor
{
    /// <summary>
    /// Represents an multi-factor authentication plugin manager
    /// </summary>
    public partial interface IMultiFactorAuthenticationPluginManager : IPluginManager<IMultiFactorAuthenticationMethod>
    {
        /// <summary>
        /// Check is active multi-factor authentication methods
        /// </summary>
        /// <param name="user">Filter by user; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains true if we have active multi-factor authentication methods
        /// </returns>
        Task<bool> HasActivePluginsAsync(/*User user = null*/int storeId = 0);

        /// <summary>
        /// Load active multi-factor authentication methods
        /// </summary>
        /// <param name="user">Filter by user; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of active multi-factor authentication methods
        /// </returns>
        Task<IList<IMultiFactorAuthenticationMethod>> LoadActivePluginsAsync(/*User user = null*/int storeId = 0);

        /// <summary>
        /// Check whether the passed multi-factor authentication method is active
        /// </summary>
        /// <param name="authenticationMethod">Multi-factor authentication method to check</param>
        /// <returns>Result</returns>
        bool IsPluginActive(IMultiFactorAuthenticationMethod authenticationMethod);

        /// <summary>
        /// Check whether the multi-factor authentication method with the passed system name is active
        /// </summary>
        /// <param name="systemName">System name of multi-factor authentication method to check</param>
        /// <param name="user">Filter by user; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<bool> IsPluginActiveAsync(string systemName/*, User user = null*/, int storeId = 0);
    }
}