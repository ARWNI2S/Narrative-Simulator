using ARWNI2S.Infrastructure.Engine.Features;
using System.Security.Claims;

namespace ARWNI2S.Engine.Features
{
    /// <summary>
    /// Default implementation for <see cref="IAuthenticationFeature"/>.
    /// </summary>
    public class CoreAuthenticationFeature : IAuthenticationFeature
    {
        /// <inheritdoc />
        public ClaimsPrincipal User { get; set; }
    }
}