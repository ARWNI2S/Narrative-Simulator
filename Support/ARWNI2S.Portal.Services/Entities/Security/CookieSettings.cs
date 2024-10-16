using ARWNI2S.Infrastructure.Configuration;

namespace ARWNI2S.Portal.Services.Entities.Security
{
    public partial class CookieSettings : ISettings
    {
        /// <summary>
        /// Expiration time on hours for the "Compare products" cookie
        /// </summary>
        public int CompareProductsCookieExpires { get; set; }

        /// <summary>
        /// Expiration time on hours for the "Recently viewed products" cookie
        /// </summary>
        public int RecentlyViewedProductsCookieExpires { get; set; }

        /// <summary>
        /// Expiration time on hours for the "User" cookie
        /// </summary>
        public int UserCookieExpires { get; set; }
    }
}
