using ARWNI2S.Node.Core.Caching;
using ARWNI2S.Portal.Services.Entities.Topics;

namespace ARWNI2S.Portal.Services.Topics
{
    /// <summary>
    /// Represents default values related to topic services
    /// </summary>
    public static partial class TopicServicesDefaults
    {
        #region Caching defaults

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : server ID
        /// {1} : show hidden?
        /// {2} : include in top menu?
        /// </remarks>
        public static CacheKey TopicsAllCacheKey => new("ni2s.topic.all.{0}-{1}-{2}", EntityCacheDefaults<Topic>.AllPrefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : server ID
        /// {1} : show hidden?
        /// {2} : include in top menu?
        /// {3} : user role IDs hash
        /// </remarks>
        public static CacheKey TopicsAllWithACLCacheKey => new("ni2s.topic.all.withacl.{0}-{1}-{2}-{3}", EntityCacheDefaults<Topic>.AllPrefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : topic system name
        /// {1} : server id
        /// {2} : user roles Ids hash
        /// </remarks>
        public static CacheKey TopicBySystemNameCacheKey => new("ni2s.topic.bysystemname.{0}-{1}-{2}", TopicBySystemNamePrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : topic system name
        /// </remarks>
        public static string TopicBySystemNamePrefix => "ni2s.topic.bysystemname.{0}";

        #endregion
    }
}