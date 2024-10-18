using ARWNI2S.Node.Core.Entities;

namespace ARWNI2S.Portal.Services.Entities.Media
{
    /// <summary>
    /// Represents a video
    /// </summary>
    public partial class Video : BaseEntity
    {
        /// <summary>
        /// Gets or sets the URL of video
        /// </summary>
        public string VideoUrl { get; set; }
    }
}
