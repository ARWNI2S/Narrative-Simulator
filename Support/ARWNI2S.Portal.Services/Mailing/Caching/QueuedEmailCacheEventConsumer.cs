using ARWNI2S.Node.Services.Caching;
using ARWNI2S.Portal.Services.Entities.Mailing;

namespace ARWNI2S.Portal.Services.Mailing.Caching
{
    /// <summary>
    /// Represents an queued email cache event consumer
    /// </summary>
    public partial class QueuedEmailCacheEventConsumer : CacheEventConsumer<QueuedEmail>
    {
    }
}
