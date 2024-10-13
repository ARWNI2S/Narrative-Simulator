using ARWNI2S.Infrastructure.Configuration;

namespace ARWNI2S.Portal.Services.Entities.Media
{
    /// <summary>
    /// Media settings
    /// </summary>
    public partial class MediaSettings : ISettings
    {
        /// <summary>
        /// Picture size of tournament pictures for autocomplete search box
        /// </summary>
        public int AutoCompleteSearchThumbPictureSize { get; set; }

        /// <summary>
        /// Default UI icon picture size
        /// </summary>
        public int DefaultUiIconPictureSize { get; set; }

        /// <summary>
        /// Picture size of image squares on a details page (used with "image squares" attribute type
        /// </summary>
        public int ImageSquarePictureSize { get; set; }

        /// <summary>
        /// Gets or sets the gameplay default image id. If 0, then wwwroot/images/default-image.png will be used
        /// </summary>
        public int GameplayDefaultImageId { get; set; }

        /// <summary>
        /// A value indicating whether picture zoom is enabled
        /// </summary>
        public bool DefaultPictureZoomEnabled { get; set; }

        /// <summary>
        /// A value indicating whether to allow uploading of SVG files in admin area
        /// </summary>
        public bool AllowSVGUploads { get; set; }

        /// <summary>
        /// Maximum allowed picture size. If a larger picture is uploaded, then it'll be resized
        /// </summary>
        public int MaximumImageSize { get; set; }

        /// <summary>
        /// Gets or sets a default quality used for image generation
        /// </summary>
        public int DefaultImageQuality { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether single (/content/images/thumbs/) or multiple (/content/images/thumbs/001/ and /content/images/thumbs/002/) directories will used for picture thumbs
        /// </summary>
        public bool MultipleThumbDirectories { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether we should use fast HASHBYTES (hash sum) database function to compare pictures when importing tournaments
        /// </summary>
        public bool ImportGameplayImagesUsingHash { get; set; }

        /// <summary>
        /// Gets or sets Azure CacheControl header (e.g. "max-age=3600, public")
        /// </summary>
        /// <remarks>
        /// max-age=[seconds]     — specifies the maximum amount of time that a representation will be considered fresh. Similar to Expires, this directive is relative to the time of the request, rather than absolute. [seconds] is the number of seconds from the time of the request you wish the representation to be fresh for.
        /// s-maxage=[seconds]    — similar to max-age, except that it only applies to shared (e.g., proxy) caches.
        /// public                — marks authenticated responses as cacheable; normally, if HTTP authentication is required, responses are automatically private.
        /// private               — allows caches that are specific to one user (e.g., in a browser) to store the response; shared caches (e.g., in a proxy) may not.
        /// no-cache              — forces caches to submit the request to the origin store for validation before releasing a cached copy, every time. This is useful to assure that authentication is respected (in combination with public), or to maintain rigid freshness, without sacrificing all of the benefits of caching.
        /// no-store              — instructs caches not to keep a copy of the representation under any conditions.
        /// must-revalidate       — tells caches that they must obey any freshness information you give them about a representation. HTTP allows caches to serve stale representations under special conditions; by specifying this header, you’re telling the cache that you want it to strictly follow your rules.
        /// proxy-revalidate      — similar to must-revalidate, except that it only applies to proxy caches.
        /// </remarks>
        public string AzureCacheControlHeader { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether need to use absolute pictures path
        /// </summary>
        public bool UseAbsoluteImagePath { get; set; }

        /// <summary>
        /// Gets or sets the value to specify a policy list for embedded content
        /// </summary>
        public string VideoIframeAllow { get; set; }

        /// <summary>
        /// Gets or sets the width of the frame in CSS pixels
        /// </summary>
        public int VideoIframeWidth { get; set; }

        /// <summary>
        /// Gets or sets the height of the frame in CSS pixels
        /// </summary>
        public int VideoIframeHeight { get; set; }

        #region Profile

        /// <summary>
        /// Picture size of user avatars (if enabled)
        /// </summary>
        public int AvatarPictureSize { get; set; }

        /// <summary>
        /// Thumnail picture size of user avatars (if enabled)
        /// </summary>
        public int AvatarThumbPictureSize { get; set; }

        /// <summary>
        /// Picture size of profile user avatar (if enabled)
        /// </summary>
        public int ProfileAvatarPictureSize { get; set; }

        /// <summary>
        /// Picture size of rank badges and medals
        /// </summary>
        public int ProfileMedalBadgePictureSize { get; set; }

        #endregion

        #region Quests

        /// <summary>
        /// Picture size of the main quest picture displayed on the quest details page
        /// </summary>
        public int QuestDetailsPictureSize { get; set; }

        /// <summary>
        /// Picture size of quest pictures
        /// </summary>
        public int QuestThumbPictureSize { get; set; }

        /// <summary>
        /// Picture size of the quest picture thumbs displayed on the quest details page
        /// </summary>
        public int QuestThumbPictureSizeOnQuestDetailsPage { get; set; }

        /// <summary>
        /// Picture size of the associated quest picture
        /// </summary>
        public int AssociatedQuestPictureSize { get; set; }

        #endregion

        #region Tournaments

        /// <summary>
        /// Picture size of the main tournament picture displayed on the tournament details page
        /// </summary>
        public int TournamentDetailsPictureSize { get; set; }

        /// <summary>
        /// Picture size of tournament picture thumbs displayed on pages.
        /// </summary>
        public int TournamentThumbPictureSize { get; set; }

        /// <summary>
        /// Picture size of the tournament picture thumbs displayed on the tournament details page
        /// </summary>
        public int TournamentThumbPictureSizeOnTournamentDetailsPage { get; set; }

        /// <summary>
        /// Picture size of the associated tournament picture
        /// </summary>
        public int AssociatedTournamentPictureSize { get; set; }

        #endregion

        #region Other

        /// <summary>
        /// Picture size of pictures on misc pages
        /// </summary>
        public int DefaultPagePictureSize { get; set; }

        /// <summary>
        /// Default thumbnail picture size on misc pages
        /// </summary>
        public int DefaultThumbPictureSize { get; set; }

        /// <summary>
        /// Picture size of tournament pictures for minishipping cart box
        /// </summary>
        public int MiniatureThumbPictureSize { get; set; }

        /// <summary>
        /// Picture size of partner pictures
        /// </summary>
        public int PartnerThumbPictureSize { get; set; }

        /// <summary>
        /// Picture size of game title pictures
        /// </summary>
        public int GameTitleThumbPictureSize { get; set; }

        #endregion
    }
}