namespace ARWNI2S.Engine.Entities.Assets
{
    /// <summary>
    /// Helper class for making assets hashes from DB
    /// </summary>
    public partial class AssetHashItem : IComparable, IComparable<AssetHashItem>
    {
        /// <summary>
        /// Gets or sets the asset ID
        /// </summary>
        public int AssetId { get; set; }

        /// <summary>
        /// Gets or sets the asset hash
        /// </summary>
        public byte[] Hash { get; set; }

        /// <summary>
        /// Compares this instance to a specified and returns an indication
        /// </summary>
        /// <param name="obj">An object to compare with this instance</param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            return CompareTo(obj as AssetHashItem);
        }

        /// <summary>
        /// Compares this instance to a specified and returns an indication
        /// </summary>
        /// <param name="other">An object to compare with this instance</param>
        /// <returns></returns>
        public int CompareTo(AssetHashItem other)
        {
            return other == null ? -1 : AssetId.CompareTo(other.AssetId);
        }
    }

}
