using ARWNI2S.Node.Data.Entities;

namespace ARWNI2S.Portal.Services.Entities.Media
{
    /// <summary>
    /// Represents a video
    /// </summary>
    public partial class Video : BaseDataEntity
    {
        /// <summary>
        /// Gets or sets the URL of video
        /// </summary>
        public string VideoUrl { get; set; }
    }
}
