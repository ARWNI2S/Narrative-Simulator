using DragonCorp.Metalink.Core.Configuration;

namespace DragonCorp.Metalink.Core.Domain.Users
{
    /// <summary>
    /// Wallet authentication settings
    /// </summary>
    public partial class WalletAuthenticationSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether email input is required.
        /// </summary>
        public bool RequireEmailInput { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether email validation is required.
        /// </summary>
        public bool RequireEmailValidation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether need to logging errors on authentication process 
        /// </summary>
        public bool LogErrors { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user is allowed to remove external authentication associations
        /// </summary>
        public bool AllowUsersToRemoveWallets { get; set; }

    }
}
