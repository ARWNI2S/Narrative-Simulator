using ARWNI2S.Backend.Services.Users.Entities;
using ARWNI2S.Engine.Data.Events;

namespace ARWNI2S.Backend.Services.Users.Caching
{
    /// <summary>
    /// Represents an external authentication record cache event consumer
    /// </summary>
    public partial class ExternalAuthenticationRecordCacheEventConsumer : CacheEventConsumer<ExternalAuthenticationRecord>
    {
    }
}