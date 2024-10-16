using ARWNI2S.Node.Services.Caching;
using ARWNI2S.Portal.Services.Entities.Common;

namespace ARWNI2S.Portal.Services.Common.Caching
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
            await RemoveAsync(CommonServicesDefaults.AddressAttributeValuesByAttributeCacheKey, entity);
        }
    }
}
