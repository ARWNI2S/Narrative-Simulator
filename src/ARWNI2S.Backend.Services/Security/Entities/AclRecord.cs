using ARWNI2S.Engine.Data.Entities;

namespace ARWNI2S.Backend.Services.Security.Entities
{
    /// <summary>
    /// Represents an ACL record
    /// </summary>
    public partial class AclRecord : DataEntity
    {
        /// <summary>
        /// Gets or sets the entity identifier
        /// </summary>
        public int EntityId { get; set; }

        /// <summary>
        /// Gets or sets the entity name
        /// </summary>
        public string EntityName { get; set; }

        /// <summary>
        /// Gets or sets the user role identifier
        /// </summary>
        public int UserRoleId { get; set; }
    }
}