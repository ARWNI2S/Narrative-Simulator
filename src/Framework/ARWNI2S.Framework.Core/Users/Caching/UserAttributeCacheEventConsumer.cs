using ARWNI2S.Engine.Data.Events;
using ARWNI2S.Framework.Attributes;
using ARWNI2S.Framework.Users.Entities;

namespace ARWNI2S.Framework.Users.Caching
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
            await RemoveAsync(AttributeDefaults.AttributeValuesByAttributeCacheKey, nameof(UserAttribute), entity);
        }
    }
}