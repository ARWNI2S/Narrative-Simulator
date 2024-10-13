//using DragonCorp.Metalink.Core.Domain.Catalog;
namespace ARWNI2S.Portal.Services.Authentication.Wallet
{
    /// <summary>
    /// Wallet authentication parameters
    /// </summary>
    [Serializable]
    public partial class WalletAuthenticationParameters
    {
        public WalletAuthenticationParameters()
        {
            //Claims = new List<WalletAuthenticationClaim>();
        }

        /// <summary>
        /// Gets or sets the system name of wallet authentication method
        /// </summary>
        public string ProviderSystemName { get; set; }

        /// <summary>
        /// Gets or sets user wallet identifier 
        /// </summary>
        public string WalletIdentifier { get; set; }

        /// <summary>
        /// Gets or sets wallet network identifier 
        /// </summary>
        public string WalletNetworkIdentifier { get; set; }

        /// <summary>
        /// Gets or sets access token
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets user public address
        /// </summary>
        public string PublicAddress { get; set; }

        ///// <summary>
        ///// Gets or sets user email
        ///// </summary>
        //public string Email { get; set; }

        ///// <summary>
        ///// Gets or sets the additional user info as a list of a custom claims
        ///// </summary>
        //public IList<WalletAuthenticationClaim> Claims { get; set; }
    }
}