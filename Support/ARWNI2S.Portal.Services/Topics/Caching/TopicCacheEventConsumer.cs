using ARWNI2S.Node.Services.Caching;
using ARWNI2S.Portal.Services.Entities.Topics;

namespace ARWNI2S.Portal.Services.Topics.Caching
{
    /// <summary>
    /// Represents a topic cache event consumer
    /// </summary>
    public partial class TopicCacheEventConsumer : CacheEventConsumer<Topic>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(Topic entity)
        {
            await RemoveByPrefixAsync(TopicServicesDefaults.TopicBySystemNamePrefix, entity.SystemName);
        }
    }
}
