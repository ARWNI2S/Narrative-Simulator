﻿using ARWNI2S.Node.Core.Caching;
using ARWNI2S.Node.Services.Caching;
using ARWNI2S.Portal.Services.Entities.Directory;

namespace ARWNI2S.Portal.Services.Globalization.Caching
{
    /// <summary>
    /// Represents a state province cache event consumer
    /// </summary>
    public partial class StateProvinceCacheEventConsumer : CacheEventConsumer<StateProvince>
    {
        /// <summary>
        /// Clear cache by entity event type
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(StateProvince entity, EntityEventType entityEventType)
        {
            await RemoveByPrefixAsync(EntityCacheDefaults<StateProvince>.Prefix);
        }
    }
}
