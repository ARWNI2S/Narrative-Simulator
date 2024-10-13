//using DragonCorp.Metalink.Core.Domain.Catalog;
namespace ARWNI2S.Portal.Services.Authentication.Wallet
{
    /// <summary>
    /// Interface to register (configure) a wallet authentication service (addon)
    /// </summary>
    public interface IWalletAuthenticationRegistrar
    {
        /// <summary>
        /// Configure
        /// </summary>
        /// <param name="builder">Authentication builder</param>
        void Configure(AuthenticationBuilder builder);
    }
}
