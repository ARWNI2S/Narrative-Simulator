//using DragonCorp.Metalink.Core.Domain.Catalog;
namespace ARWNI2S.Portal.Services.Authentication.Wallet
{
    public class WalletAuthenticationDefaults
    {
        public static readonly string DisplayName = "CryptoWallet";

        public static readonly string PublicAddressKey = "publicAddress";

        public static readonly string DateTimeKey = "issuedOnUtc";

        public static readonly string WalletConnectedAttibute = DisplayName + ".Connected";

        public static readonly string WalletNetworkAttibute = DisplayName + ".Network";
    }
}