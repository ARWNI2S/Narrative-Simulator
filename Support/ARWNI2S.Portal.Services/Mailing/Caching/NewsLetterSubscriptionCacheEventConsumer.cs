using ARWNI2S.Node.Services.Caching;
using ARWNI2S.Portal.Services.Entities.Mailing;

namespace ARWNI2S.Portal.Services.Mailing.Caching
{
    /// <summary>
    /// Represents news letter subscription cache event consumer
    /// </summary>
    public partial class NewsLetterSubscriptionCacheEventConsumer : CacheEventConsumer<NewsLetterSubscription>
    {
    }
}
