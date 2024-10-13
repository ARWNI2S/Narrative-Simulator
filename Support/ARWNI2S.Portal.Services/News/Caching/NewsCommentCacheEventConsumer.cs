using ARWNI2S.Node.Services.Caching;
using ARWNI2S.Portal.Services.Entities.News;

namespace ARWNI2S.Portal.Services.News.Caching
{
    /// <summary>
    /// Represents a news comment cache event consumer
    /// </summary>
    public partial class NewsCommentCacheEventConsumer : CacheEventConsumer<NewsComment>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(NewsComment entity)
        {
            await RemoveByPrefixAsync(NewsServicesDefaults.NewsCommentsNumberPrefix, entity.NewsItemId);
        }
    }
}