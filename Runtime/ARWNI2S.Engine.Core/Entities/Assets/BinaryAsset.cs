using ARWNI2S.Node.Core.Entities;

namespace ARWNI2S.Engine.Entities.Assets
{
    /// <summary>
    /// Represents a asset binary data
    /// </summary>
    public partial class BinaryAsset : DataEntity
    {
        /// <summary>
        /// Gets or sets the asset binary data
        /// </summary>
        public byte[] BinaryData { get; set; }

        /// <summary>
        /// Gets or sets the asset identifier
        /// </summary>
        public int AssetId { get; set; }
    }
}
