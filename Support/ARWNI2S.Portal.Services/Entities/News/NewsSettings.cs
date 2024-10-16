﻿using ARWNI2S.Infrastructure.Configuration;

namespace ARWNI2S.Portal.Services.Entities.News
{
    /// <summary>
    /// News settings
    /// </summary>
    public partial class NewsSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether news are enabled
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether not registered user can leave comments
        /// </summary>
        public bool AllowNotRegisteredUsersToLeaveComments { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to notify about new news comments
        /// </summary>
        public bool NotifyAboutNewNewsComments { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show news on the main page
        /// </summary>
        public bool ShowNewsOnMainPage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating news count displayed on the main page
        /// </summary>
        public int MainPageNewsCount { get; set; }

        /// <summary>
        /// Gets or sets the page size for news archive
        /// </summary>
        public int NewsArchivePageSize { get; set; }

        /// <summary>
        /// Enable the news RSS feed link in users browser address bar
        /// </summary>
        public bool ShowHeaderRssUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether news comments must be approved
        /// </summary>
        public bool NewsCommentsMustBeApproved { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether news comments will be filtered per node
        /// </summary>
        public bool ShowNewsCommentsPerNode { get; set; }
    }
}