using ARWNI2S.Configuration;

namespace ARWNI2S.Framework.Common.Entities
{
    /// <summary>
    /// Display default menu item settings
    /// </summary>
    public partial class DisplayDefaultMenuItemSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether to display "home page" menu item
        /// </summary>
        public bool DisplayHomepageMenuItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "new products" menu item
        /// </summary>
        public bool DisplayNewProductsMenuItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "product search" menu item
        /// </summary>
        public bool DisplayProductSearchMenuItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "user info" menu item
        /// </summary>
        public bool DisplayUserInfoMenuItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "blog" menu item
        /// </summary>
        public bool DisplayBlogMenuItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "forums" menu item
        /// </summary>
        public bool DisplayForumsMenuItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "contact us" menu item
        /// </summary>
        public bool DisplayContactUsMenuItem { get; set; }
    }
}