using ARWNI2S.Backend.Services.Users.Entities;
using ARWNI2S.Engine.Data.Events;

namespace ARWNI2S.Backend.Services.Users.Caching
{
    /// <summary>
    /// Represents a user role cache event consumer
    /// </summary>
    public partial class UserRoleCacheEventConsumer : CacheEventConsumer<UserRole>
    {
        /// <summary>
        /// Clear cache by entity event type
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(UserRole entity, EntityEventType entityEventType)
        {
            switch (entityEventType)
            {
                case EntityEventType.Update:
                    await RemoveByPrefixAsync(UserServicesDefaults.UserRolesBySystemNamePrefix);
                    break;
                case EntityEventType.Delete:
                    await RemoveAsync(UserServicesDefaults.UserRolesBySystemNameCacheKey, entity.SystemName);
                    break;
            }

            if (entityEventType != EntityEventType.Insert)
                await RemoveByPrefixAsync(UserServicesDefaults.UserUserRolesPrefix);

            await base.ClearCacheAsync(entity, entityEventType);
        }
    }
}