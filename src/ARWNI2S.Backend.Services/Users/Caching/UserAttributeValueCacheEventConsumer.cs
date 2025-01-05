using ARWNI2S.Backend.Services.Users.Entities;
using ARWNI2S.Engine.Data.Events;
using ARWNI2S.Framework.Attributes;

namespace ARWNI2S.Backend.Services.Users.Caching
{
    /// <summary>
    /// Represents a user attribute value cache event consumer
    /// </summary>
    public partial class UserAttributeValueCacheEventConsumer : CacheEventConsumer<UserAttributeValue>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(UserAttributeValue entity)
        {
            await RemoveAsync(AttributeDefaults.AttributeValuesByAttributeCacheKey, nameof(UserAttribute), entity.AttributeId);
        }
    }
}