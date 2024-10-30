using ARWNI2S.Node.Core.Entities.Localization;
using ARWNI2S.Node.Core.Entities;

namespace ARWNI2S.Portal.Services.Entities.Common
{
    /// <summary>
    /// Represents an address attribute value
    /// </summary>
    public partial class AddressAttributeValue : DataEntity, ILocalizedEntity
    {
        /// <summary>
        /// Gets or sets the address attribute identifier
        /// </summary>
        public int AddressAttributeId { get; set; }

        /// <summary>
        /// Gets or sets the checkout attribute name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the value is pre-selected
        /// </summary>
        public bool IsPreSelected { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }
    }
}
