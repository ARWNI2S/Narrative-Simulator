//using DragonCorp.Metalink.Core.Domain.Catalog;
using Newtonsoft.Json;

namespace ARWNI2S.Portal.Services.Authentication.Wallet.Internal
{
    internal record SignatureDTO
    {
        [JsonProperty("publicAddress")]
        public string PublicAddress { get; set; }

        [JsonProperty("signature")]
        public string Signature { get; set; }

        [JsonProperty("__RequestVerificationToken")]
        public string AntiForgeryToken { get; set; }
    }
}
