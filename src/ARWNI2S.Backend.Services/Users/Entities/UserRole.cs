using ARWNI2S.Engine.Data.Entities;

namespace ARWNI2S.Backend.Services.Users.Entities
{
    /// <summary>
    /// Represents a user role
    /// </summary>
    public partial class UserRole : DataEntity
    {
        /// <summary>
        /// Gets or sets the user role name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user role is marked as free shipping
        /// </summary>
        public bool FreeShipping { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user role is marked as tax exempt
        /// </summary>
        public bool TaxExempt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user role is active
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user role is system
        /// </summary>
        public bool IsSystemRole { get; set; }

        /// <summary>
        /// Gets or sets the user role system name
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the users must change passwords after a specified time
        /// </summary>
        public bool EnablePasswordLifetime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the users of this role have other tax display type chosen instead of the default one
        /// </summary>
        public bool OverrideTaxDisplayType { get; set; }

        /// <summary>
        /// Gets or sets identifier of the default tax display type (used only with "OverrideTaxDisplayType" enabled)
        /// </summary>
        public int DefaultTaxDisplayTypeId { get; set; }

        /// <summary>
        /// Gets or sets a product identifier that is required by this user role. 
        /// A user is added to this user role once a specified product is purchased.
        /// </summary>
        public int PurchasedWithProductId { get; set; }
    }
}