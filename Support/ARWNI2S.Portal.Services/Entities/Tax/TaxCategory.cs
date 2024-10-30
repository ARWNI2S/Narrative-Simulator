using ARWNI2S.Node.Core.Entities;

namespace ARWNI2S.Portal.Services.Entities.Tax
{
    /// <summary>
    /// Represents a tax category
    /// </summary>
    public partial class TaxCategory : DataEntity
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }
    }
}
