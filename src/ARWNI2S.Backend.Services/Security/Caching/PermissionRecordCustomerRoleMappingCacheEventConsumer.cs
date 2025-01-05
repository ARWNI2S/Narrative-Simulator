using ARWNI2S.Backend.Services.Security.Entities;
using ARWNI2S.Engine.Data.Events;

namespace ARWNI2S.Backend.Services.Security.Caching
{
    /// <summary>
    /// Represents a permission record-user role mapping cache event consumer
    /// </summary>
    public partial class PermissionRecordUserRoleMappingCacheEventConsumer : CacheEventConsumer<PermissionRecordUserRoleMapping>
    {
    }
}