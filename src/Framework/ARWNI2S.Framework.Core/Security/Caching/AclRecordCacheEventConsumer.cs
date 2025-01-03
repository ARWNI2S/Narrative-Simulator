using ARWNI2S.Engine.Data.Events;
using ARWNI2S.Framework.Users.Security;
using ARWNI2S.Framework.Users.Security.Entities;

namespace ARWNI2S.Framework.Users.Security.Caching
{
    /// <summary>
    /// Represents a ACL record cache event consumer
    /// </summary>
    public partial class AclRecordCacheEventConsumer : CacheEventConsumer<AclRecord>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(AclRecord entity)
        {
            await RemoveAsync(UserSecurityDefaults.AclRecordCacheKey, entity.EntityId, entity.EntityName);
            await RemoveAsync(UserSecurityDefaults.EntityAclRecordExistsCacheKey, entity.EntityName);
        }
    }
}