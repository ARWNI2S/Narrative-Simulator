using ARWNI2S.Node.Services.Caching;
using ARWNI2S.Portal.Services.Entities.Security;

namespace ARWNI2S.Portal.Services.Security.Caching
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
            await RemoveByPrefixAsync(SecurityServicesDefaults.PermissionAllowedPrefix, entity.SystemName);
        }
    }
}
