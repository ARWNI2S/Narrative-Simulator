using ARWNI2S.Node.Core.Caching;

namespace ARWNI2S.Portal.Services.Blogs
{
    /// <summary>
    /// Represents default values related to blogs services
    /// </summary>
    public static partial class BlogServiceDefaults
    {
        #region Caching defaults

        /// <summary>
        /// Key for number of blog comments
        /// </summary>
        /// <remarks>
        /// {0} : blog post ID
        /// {1} : server ID
        /// {2} : are only approved comments?
        /// </remarks>
        public static CacheKey BlogCommentsNumberCacheKey => new("ni2s.blogcomment.number.{0}-{1}-{2}", BlogCommentsNumberPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : blog post ID
        /// </remarks>
        public static string BlogCommentsNumberPrefix => "ni2s.blogcomment.number.{0}";

        /// <summary>
        /// Key for blog tag list model
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : current server ID
        /// {2} : show hidden?
        /// </remarks>
        public static CacheKey BlogTagsCacheKey => new("ni2s.blogpost.tags.{0}-{1}-{2}", BlogTagsPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string BlogTagsPrefix => "ni2s.blogpost.tags.";

        #endregion
    }
}