using ARWNI2S.Node.Services.Caching;
using ARWNI2S.Portal.Services.Entities.Blogs;

namespace ARWNI2S.Portal.Services.Blogs.Caching
{
    /// <summary>
    /// Represents a blog comment cache event consumer
    /// </summary>
    public partial class BlogCommentCacheEventConsumer : CacheEventConsumer<BlogComment>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(BlogComment entity)
        {
            await RemoveByPrefixAsync(BlogServiceDefaults.BlogCommentsNumberPrefix, entity.BlogPostId);
        }
    }
}