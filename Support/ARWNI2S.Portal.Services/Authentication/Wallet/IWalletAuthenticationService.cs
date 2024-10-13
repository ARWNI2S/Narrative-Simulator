//using DragonCorp.Metalink.Core.Domain.Catalog;
namespace ARWNI2S.Portal.Services.Authentication.Wallet
{
    /// <summary>
    /// Wallet authentication service
    /// </summary>
    public partial interface IWalletAuthenticationService
    {
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
        Task<IActionResult> AuthenticateAsync(WalletAuthenticationParameters parameters, string returnUrl = null);

        #endregion

        /// <summary>
        /// Get the crypto addresses by identifier
        /// </summary>
        /// <param name="walletAuthenticationRecordId">Crypto address identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<WalletAuthenticationRecord> GetWalletAuthenticationRecordByIdAsync(int walletAuthenticationRecordId);

        /// <summary>
        /// Get list of the crypto addresses by user
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<IList<WalletAuthenticationRecord>> GetUserWalletAuthenticationRecordsAsync(User user);

        /// <summary>
        /// Delete the crypto address
        /// </summary>
        /// <param name="walletAuthenticationRecord">Crypto address</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteWalletAuthenticationRecordAsync(WalletAuthenticationRecord walletAuthenticationRecord);

        /// <summary>
        /// Get the crypto address
        /// </summary>
        /// <param name="parameters">Wallet authentication parameters</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<WalletAuthenticationRecord> GetWalletAuthenticationRecordByWalletAuthenticationParametersAsync(WalletAuthenticationParameters parameters);

        /// <summary>
        /// Associate wallet account with user
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="parameters">Wallet authentication parameters</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task AssociateWalletAccountWithUserAsync(User user, WalletAuthenticationParameters parameters);

        /// <summary>
        /// Get the particular user with specified parameters
        /// </summary>
        /// <param name="parameters">Wallet authentication parameters</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the user
        /// </returns>
        Task<User> GetUserByWalletAuthenticationParametersAsync(WalletAuthenticationParameters parameters);

        /// <summary>
        /// Remove the association
        /// </summary>
        /// <param name="parameters">Wallet authentication parameters</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task RemoveAssociationAsync(WalletAuthenticationParameters parameters);


        #endregion
    }
}
