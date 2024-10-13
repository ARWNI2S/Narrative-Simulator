using ARWNI2S.Infrastructure.Configuration;

namespace ARWNI2S.Portal.Services.Entities
{
    /// <summary>
    /// Node information settings
    /// </summary>
    public partial class PortalInfoSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether node is offline
        /// </summary>
        public bool NodeOffline { get; set; }

        /// <summary>
        /// Gets or sets a picture identifier of the logo. If 0, then the default one will be used
        /// </summary>
        public int LogoPictureId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether we should display warnings about the new EU cookie law
        /// </summary>
        public bool DisplayEuCookieLawWarning { get; set; }

        /// <summary>
        /// Gets or sets a value of Facebook page URL 
        /// </summary>
        public string FacebookLink { get; set; }

        /// <summary>
        /// Gets or sets a value of Discord invite link
        /// </summary>
        public string DiscordInviteLink { get; set; }

        /// <summary>
        /// Gets or sets a value of the Discord server 
        /// </summary>
        public string DiscordServerId { get; set; }

        /// <summary>
        /// Gets or sets a value of Twitter page URL 
        /// </summary>
        public string TwitterLink { get; set; }

        /// <summary>
        /// Gets or sets a value of YouTube channel URL 
        /// </summary>
        public string YouTubeLink { get; set; }

        /// <summary>
        /// Gets or sets a value of Instagram account URL 
        /// </summary>
        public string InstagramLink { get; set; }
    }
}
