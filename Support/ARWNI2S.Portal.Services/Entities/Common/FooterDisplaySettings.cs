using ARWNI2S.Infrastructure.Configuration;

namespace ARWNI2S.Portal.Services.Entities.Common
{
    /// <summary>
    /// Display default menu item settings
    /// </summary>
    public partial class FooterDisplaySettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether to display "sitemap" footer item
        /// </summary>
        public bool DisplaySitemapFooterItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "contact us" footer item
        /// </summary>
        public bool DisplayContactUsFooterItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "product search" footer item
        /// </summary>
        public bool DisplaySearchFooterItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "news" footer item
        /// </summary>
        public bool DisplayNewsFooterItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "blog" footer item
        /// </summary>
        public bool DisplayBlogFooterItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "new content" footer item
        /// </summary>
        public bool DisplayNewContentFooterItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "user info" footer item
        /// </summary>
        public bool DisplayUserInfoFooterItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "apply partner account" footer item
        /// </summary>
        public bool DisplayApplyPartnerAccountFooterItem { get; set; }
    }
}
