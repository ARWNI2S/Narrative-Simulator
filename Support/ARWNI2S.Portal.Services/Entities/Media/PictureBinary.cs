using ARWNI2S.Node.Data.Entities;

namespace ARWNI2S.Portal.Services.Entities.Media
{
    /// <summary>
    /// Represents a picture binary data
    /// </summary>
    public partial class PictureBinary : BaseDataEntity
    {
        /// <summary>
        /// Gets or sets the picture binary
        /// </summary>
        public byte[] BinaryData { get; set; }

        /// <summary>
        /// Gets or sets the picture identifier
        /// </summary>
        public int PictureId { get; set; }
    }
}
