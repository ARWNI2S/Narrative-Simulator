using ARWNI2S.Backend.Services.Security.Entities;
using ARWNI2S.Engine.Data.Events;

namespace ARWNI2S.Backend.Services.Security.Caching
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
            await RemoveAsync(SecurityDefaults.AclRecordCacheKey, entity.EntityId, entity.EntityName);
            await RemoveAsync(SecurityDefaults.EntityAclRecordExistsCacheKey, entity.EntityName);
        }
    }
}