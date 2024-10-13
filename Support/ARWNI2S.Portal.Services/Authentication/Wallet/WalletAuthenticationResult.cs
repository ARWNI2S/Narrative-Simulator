//using DragonCorp.Metalink.Core.Domain.Catalog;
namespace ARWNI2S.Portal.Services.Authentication.Wallet
{
    public class WalletAuthenticationResult
    {
        private WalletAuthenticationResult(string accessToken, string expiresIn)
        {
            AccessToken = accessToken;
            ExpiresIn = expiresIn;
        }

        private WalletAuthenticationResult(Exception error)
        {
            Error = error;
        }

        public static WalletAuthenticationResult Success(string accessToken, string expiresIn)
        {
            return new WalletAuthenticationResult(accessToken, expiresIn);
        }

        public static WalletAuthenticationResult Failed(Exception error)
        {
            return new WalletAuthenticationResult(error);
        }

        public string AccessToken { get; private set; }
        public string ExpiresIn { get; private set; }
        public Exception Error { get; set; }
    }
}
