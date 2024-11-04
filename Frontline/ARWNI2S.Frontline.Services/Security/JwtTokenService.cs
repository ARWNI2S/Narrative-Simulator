using ARWNI2S.Node.Core.Configuration;
using ARWNI2S.Node.Core.Entities.Users;
using ARWNI2S.Node.Services.Authentication;
using ARWNI2S.Node.Services.Security;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ARWNI2S.Frontline.Services.Security
{
    public partial class JwtTokenService : IUserTokenService
    {
        private readonly JwtTokenConfig _configuration;

        public JwtTokenService(NI2SSettings ni2sSettings)
        {
            _configuration = ni2sSettings.Get<JwtTokenConfig>();
        }

        public JwtSecurityToken GenerateToken(User user)
        {
            //create claims for user's username and email
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.UserGuid.ToString()),
            };

            if (!string.IsNullOrEmpty(user.Username))
                claims.Add(new Claim(ClaimTypes.Name, user.Username, ClaimValueTypes.String, AuthenticationServicesDefaults.ClaimsIssuer));

            if (!string.IsNullOrEmpty(user.Email))
                claims.Add(new Claim(ClaimTypes.Email, user.Email, ClaimValueTypes.Email, AuthenticationServicesDefaults.ClaimsIssuer));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            return new JwtSecurityToken(
                issuer: _configuration.Issuer,
                audience: _configuration.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration.TokenExpiryInMinutes)),
                signingCredentials: creds);
        }

        public string SerializeToken(User user) => SerializeToken(GenerateToken(user));

        public string SerializeToken(JwtSecurityToken token)
        {
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
