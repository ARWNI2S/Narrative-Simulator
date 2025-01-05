using ARWNI2S.Engine.Data.Entities;

namespace ARWNI2S.Backend.Services.Users.Entities
{
    /// <summary>
    /// Represents a user-address mapping class
    /// </summary>
    public partial class UserAddressMapping : DataEntity
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