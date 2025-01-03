using ARWNI2S.Engine.Data.Events;
using ARWNI2S.Framework.Users.Entities;

namespace ARWNI2S.Framework.Users.Caching
{
    /// <summary>
    /// Represents a user address mapping cache event consumer
    /// </summary>
    public partial class UserAddressMappingCacheEventConsumer : CacheEventConsumer<UserAddressMapping>
    {
        /// <summary>
        /// Clear cache by entity event type
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(UserAddressMapping entity, EntityEventType entityEventType)
        {
            await RemoveAsync(UserServicesDefaults.UserAddressesCacheKey, entity.UserId);

            if (entityEventType == EntityEventType.Delete)
                await RemoveAsync(UserServicesDefaults.UserAddressCacheKey, entity.UserId, entity.AddressId);

            await base.ClearCacheAsync(entity, entityEventType);
        }
    }
}