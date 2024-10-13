using ARWNI2S.Node.Services.Caching;
using ARWNI2S.Portal.Services.Entities.News;

namespace ARWNI2S.Portal.Services.News.Caching
{
    /// <summary>
    /// Represents a news item cache event consumer
    /// </summary>
    public partial class NewsItemCacheEventConsumer : CacheEventConsumer<NewsItem>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(NewsItem entity, EntityEventType entityEventType)
        {
            if (entityEventType == EntityEventType.Delete)
                await RemoveByPrefixAsync(NewsServicesDefaults.NewsCommentsNumberPrefix, entity);

            await base.ClearCacheAsync(entity, entityEventType);
        }
    }
}