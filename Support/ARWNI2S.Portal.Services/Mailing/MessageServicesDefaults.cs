using ARWNI2S.Node.Core.Caching;
using ARWNI2S.Portal.Services.Entities.Mailing;

namespace ARWNI2S.Portal.Services.Mailing
{
    /// <summary>
    /// Represents default values related to messages services
    /// </summary>
    public static partial class MessageServicesDefaults
    {
        /// <summary>
        /// Gets a key for notifications list from TempDataDictionary
        /// </summary>
        public static string NotificationListKey => "NotificationList";

        #region Caching defaults

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : server ID
        /// {1} : is active?
        /// </remarks>
        public static CacheKey MessageTemplatesAllCacheKey => new("ni2s.messagetemplate.all.{0}-{1}", EntityCacheDefaults<MessageTemplate>.AllPrefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : template name
        /// {1} : server ID
        /// </remarks>
        public static CacheKey MessageTemplatesByNameCacheKey => new("ni2s.messagetemplate.byname.{0}-{1}", MessageTemplatesByNamePrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : template name
        /// </remarks>
        public static string MessageTemplatesByNamePrefix => "ni2s.messagetemplate.byname.{0}";

        #endregion
    }
}