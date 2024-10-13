using ARWNI2S.Node.Services.Caching;
using ARWNI2S.Portal.Services.Entities.Seo;

namespace ARWNI2S.Portal.Services.Seo.Caching
{
    /// <summary>
    /// Represents an URL record cache event consumer
    /// </summary>
    public partial class UrlRecordCacheEventConsumer : CacheEventConsumer<UrlRecord>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(UrlRecord entity)
        {
            await RemoveAsync(SeoServicesDefaults.UrlRecordCacheKey, entity.EntityId, entity.EntityName, entity.LanguageId);
            await RemoveAsync(SeoServicesDefaults.UrlRecordBySlugCacheKey, entity.Slug);
            await RemoveAsync(SeoServicesDefaults.UrlRecordEntityIdLookupCacheKey, entity.LanguageId);
            await RemoveAsync(SeoServicesDefaults.UrlRecordSlugLookupCacheKey);
        }
    }
}
