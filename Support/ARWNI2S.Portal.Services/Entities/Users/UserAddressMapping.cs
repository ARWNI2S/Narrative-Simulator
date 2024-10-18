using ARWNI2S.Node.Core.Entities;

namespace ARWNI2S.Portal.Services.Entities.Users
{
    /// <summary>
    /// Represents a user-address mapping class
    /// </summary>
    public partial class UserAddressMapping : BaseEntity
    {
        /// <summary>
        /// Gets or sets the user identifier
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the address identifier
        /// </summary>
        public int AddressId { get; set; }
    }
}