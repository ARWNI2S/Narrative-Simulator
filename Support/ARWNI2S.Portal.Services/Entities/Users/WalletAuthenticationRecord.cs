namespace DragonCorp.Metalink.Core.Domain.Users
{
    /// <summary>
    /// Represents a wallet authentication record
    /// </summary>
    public partial class WalletAuthenticationRecord : BaseEntity
    {
        /// <summary>
        /// Gets or sets the user identifier
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the associated crypto address
        /// </summary>
        public int CryptoAddressId { get; set; }

        /// <summary>
        /// Gets or sets the wallet identifier
        /// </summary>
        public string WalletIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the wallet display identifier
        /// </summary>
        public string WalletNetworkIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the authentication token
        /// </summary>
        public string AuthToken { get; set; }

        /// <summary>
        /// Gets or sets the authentication access token
        /// </summary>
        public string AuthAccessToken { get; set; }

        /// <summary>
        /// Gets or sets the provider
        /// </summary>
        public string ProviderSystemName { get; set; }


    }
}
