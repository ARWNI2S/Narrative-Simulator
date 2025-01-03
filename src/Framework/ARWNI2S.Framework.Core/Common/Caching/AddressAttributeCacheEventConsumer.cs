using ARWNI2S.Engine.Data.Events;
using ARWNI2S.Framework.Attributes;
using ARWNI2S.Framework.Common.Entities;

namespace ARWNI2S.Framework.Common.Caching
{
    /// <summary>
    /// Represents a address attribute cache event consumer
    /// </summary>
    public partial class AddressAttributeCacheEventConsumer : CacheEventConsumer<AddressAttribute>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(AddressAttribute entity)
        {
            await RemoveAsync(AttributeDefaults.AttributeValuesByAttributeCacheKey, nameof(AddressAttribute), entity);
        }
    }
}