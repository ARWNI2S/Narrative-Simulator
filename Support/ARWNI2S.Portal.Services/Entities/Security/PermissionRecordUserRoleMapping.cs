using ARWNI2S.Node.Data.Entities;

namespace ARWNI2S.Portal.Services.Entities.Security
{
    /// <summary>
    /// Represents a permission record-user role mapping class
    /// </summary>
    public partial class PermissionRecordUserRoleMapping : BaseDataEntity
    {
        /// <summary>
        /// Gets or sets the permission record identifier
        /// </summary>
        public int PermissionRecordId { get; set; }

        /// <summary>
        /// Gets or sets the user role identifier
        /// </summary>
        public int UserRoleId { get; set; }
    }
}