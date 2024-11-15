namespace ARWNI2S.Engine.Entities.Assets
{
    internal class TextureAsset : BaseAsset
    {
        /// <summary>
        /// Gets or sets the picture file type
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// Gets or sets the texture compression type
        /// </summary>
        public int CompressionType { get; set; }

        /// <summary>
        /// Gets or sets the texture type
        /// </summary>
        public int TextureType { get; set; }

    }
}
