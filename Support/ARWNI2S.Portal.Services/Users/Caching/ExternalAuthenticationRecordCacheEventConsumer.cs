using ARWNI2S.Node.Core.Entities.Users;
using ARWNI2S.Node.Services.Caching;

namespace ARWNI2S.Portal.Services.Users.Caching
{
    /// <summary>
    /// Represents an external authentication record cache event consumer
    /// </summary>
    public partial class ExternalAuthenticationRecordCacheEventConsumer : CacheEventConsumer<ExternalAuthenticationRecord>
    {
    }
}
