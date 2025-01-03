using ARWNI2S.Engine.Data.Events;
using ARWNI2S.Events;
using ARWNI2S.Framework.Users.Entities;

namespace ARWNI2S.Framework.Users.Caching
{
    /// <summary>
    /// Represents a user cache event consumer
    /// </summary>
    public partial class UserCacheEventConsumer : CacheEventConsumer<User>, IEventConsumer<UserPasswordChangedEvent>
    {
        #region Methods

        /// <summary>
        /// Handle password changed event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(UserPasswordChangedEvent eventMessage)
        {
            await RemoveAsync(UserServicesDefaults.UserPasswordLifetimeCacheKey, eventMessage.Password.UserId);
        }

        /// <summary>
        /// Clear cache by entity event type
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(User entity, EntityEventType entityEventType)
        {
            if (entityEventType == EntityEventType.Delete)
            {
                await RemoveAsync(UserServicesDefaults.UserAddressesCacheKey, entity);
                await RemoveByPrefixAsync(UserServicesDefaults.UserAddressesByUserPrefix, entity);
            }

            await base.ClearCacheAsync(entity, entityEventType);
        }

        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(User entity)
        {
            await RemoveAsync(UserServicesDefaults.UserRolesCacheKey, entity);
            //await RemoveByPrefixAsync(NopOrderDefaults.ShoppingCartItemsByUserPrefix, entity);
            await RemoveAsync(UserServicesDefaults.UserByGuidCacheKey, entity.UserGuid);

            if (string.IsNullOrEmpty(entity.SystemName))
                return;

            await RemoveAsync(UserServicesDefaults.UserBySystemNameCacheKey, entity.SystemName);
        }

        #endregion
    }
}