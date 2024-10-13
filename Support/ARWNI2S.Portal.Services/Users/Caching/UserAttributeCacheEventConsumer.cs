using ARWNI2S.Node.Services.Caching;
using ARWNI2S.Portal.Services.Entities.Users;

namespace ARWNI2S.Portal.Services.Users.Caching
{
    /// <summary>
    /// Represents a user attribute cache event consumer
    /// </summary>
    public partial class UserAttributeCacheEventConsumer : CacheEventConsumer<UserAttribute>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(UserAttribute entity)
        {
            await RemoveAsync(UserServicesDefaults.UserAttributeValuesByAttributeCacheKey, entity);
        }
    }
}
