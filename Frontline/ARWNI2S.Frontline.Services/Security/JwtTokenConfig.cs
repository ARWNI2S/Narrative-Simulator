using ARWNI2S.Infrastructure.Configuration;
using ARWNI2S.Node.Core.Security.Secrets;
using ARWNI2S.Node.Services.Authentication;
using Newtonsoft.Json;

namespace ARWNI2S.Frontline.Services.Security
{
    internal class JwtTokenConfig : IConfig
    {
        /// <summary>
        /// Gets a section name to load configuration
        /// </summary>
        [JsonIgnore]
        public string Name => JwtTokenDefaults.JwtConfigSectionName;

        [Secret]
        public string Key { get; set; }

        public string Issuer { get; set; } = AuthenticationServicesDefaults.ClaimsIssuer;

        public string Audience { get; set; } = $"{AuthenticationServicesDefaults.ClaimsIssuer}-client";

        public int TokenExpiryInMinutes { get; set; } = 60;


        /// <inheritdoc/>
        public int GetOrder() => 1;
    }
}
