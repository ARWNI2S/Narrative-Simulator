using ARWNI2S.Node.Services.Caching;
using ARWNI2S.Portal.Services.Entities.Blogs;

namespace ARWNI2S.Portal.Services.Blogs.Caching
{
    /// <summary>
    /// Represents a blog post cache event consumer
    /// </summary>
    public partial class BlogPostCacheEventConsumer : CacheEventConsumer<BlogPost>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(BlogPost entity)
        {
            await RemoveByPrefixAsync(BlogServiceDefaults.BlogTagsPrefix);
        }
    }
}