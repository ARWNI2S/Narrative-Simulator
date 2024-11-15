using ARWNI2S.Node.Core.Entities;

namespace ARWNI2S.Engine.Entities.Assets
{
    public partial class BaseAsset : DataEntity
    {
        /// <summary>
        /// Gets or sets the asset type
        /// </summary>
        public string AssetType { get; set; }

        /// <summary>
        /// Gets or sets the asset unique name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the asset is new
        /// </summary>
        public bool IsNew { get; set; }

        /// <summary>
        /// Gets or sets the asset virtual path
        /// </summary>
        public string VirtualPath { get; set; }
    }
}
