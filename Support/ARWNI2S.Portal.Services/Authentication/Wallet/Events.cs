//using DragonCorp.Metalink.Core.Domain.Catalog;
namespace ARWNI2S.Portal.Services.Authentication.Wallet
{
    /// <summary>
    /// User auto registered by wallet extension event
    /// </summary>
    public partial class UserAutoRegisteredByWalletExtensionEvent
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="parameters">Parameters</param>
        public UserAutoRegisteredByWalletExtensionEvent(User user, WalletAuthenticationParameters parameters)
        {
            User = user;
            AuthenticationParameters = parameters;
        }

        /// <summary>
        /// Gets or sets user
        /// </summary>
        public User User { get; }

        /// <summary>
        /// Gets or sets wallet authentication parameters
        /// </summary>
        public WalletAuthenticationParameters AuthenticationParameters { get; }
    }
}
