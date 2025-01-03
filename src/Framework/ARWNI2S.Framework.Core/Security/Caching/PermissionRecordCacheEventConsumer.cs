using ARWNI2S.Engine.Data.Events;
using ARWNI2S.Framework.Users.Security;
using ARWNI2S.Framework.Users.Security.Entities;

namespace ARWNI2S.Framework.Users.Security.Caching
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
            await RemoveByPrefixAsync(UserSecurityDefaults.PermissionAllowedPrefix, entity.SystemName);
        }
    }
}