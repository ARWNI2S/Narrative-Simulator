using ARWNI2S.Node.Services.Caching;
using ARWNI2S.Node.Services.Common;
using ARWNI2S.Portal.Services.Entities.Common;

namespace ARWNI2S.Portal.Services.Common.Caching
{
    /// <summary>
    /// Represents a address attribute value cache event consumer
    /// </summary>
    public partial class AddressAttributeValueCacheEventConsumer : CacheEventConsumer<AddressAttributeValue>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(AddressAttributeValue entity)
        {
            await RemoveAsync(CommonServicesDefaults.AddressAttributeValuesByAttributeCacheKey, entity.AddressAttributeId);
        }
    }
}
