using ARWNI2S.Backend.Services.Security.Entities;
using ARWNI2S.Engine.Data.Events;

namespace ARWNI2S.Backend.Services.Security.Caching
{
    /// <summary>
    /// Represents a permission record cache event consumer
    /// </summary>
    public partial class PermissionRecordCacheEventConsumer : CacheEventConsumer<PermissionRecord>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(PermissionRecord entity)
        {
            await RemoveByPrefixAsync(SecurityDefaults.PermissionAllowedPrefix, entity.SystemName);
        }
    }
}