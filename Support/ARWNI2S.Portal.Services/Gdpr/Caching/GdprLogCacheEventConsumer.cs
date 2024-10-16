﻿using ARWNI2S.Node.Services.Caching;
using ARWNI2S.Portal.Services.Entities.Gdpr;

namespace ARWNI2S.Portal.Services.Gdpr.Caching
{
    /// <summary>
    /// Represents a GDPR log cache event consumer
    /// </summary>
    public partial class GdprLogCacheEventConsumer : CacheEventConsumer<GdprLog>
    {
    }
}
