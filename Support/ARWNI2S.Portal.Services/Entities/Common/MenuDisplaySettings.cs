using ARWNI2S.Infrastructure.Configuration;

namespace ARWNI2S.Portal.Services.Entities.Common
{
    /// <summary>
    /// Display default menu item settings
    /// </summary>
    public partial class MenuDisplaySettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether to display "home page" menu item
        /// </summary>
        public bool DisplayHomepageMenuItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "new content" menu item
        /// </summary>
        public bool DisplayNewContentMenuItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "content search" menu item
        /// </summary>
        public bool DisplayContentSearchMenuItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "user info" menu item
        /// </summary>
        public bool DisplayUserInfoMenuItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "blog" menu item
        /// </summary>
        public bool DisplayBlogMenuItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "news" menu item
        /// </summary>
        public bool DisplayNewsMenuItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "games" menu item
        /// </summary>
        public bool DisplayGamesMenuItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "quests" menu item
        /// </summary>
        public bool DisplayQuestsMenuItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "tournaments" menu item
        /// </summary>
        public bool DisplayTournamentsMenuItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "contact us" menu item
        /// </summary>
        public bool DisplayContactUsMenuItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display inventory menu item
        /// </summary>
        public bool DisplayInventoryMenuItem { get; set; }
    }
}