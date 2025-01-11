using ARWNI2S.Engine.Data.Events;

namespace ARWNI2S.Framework.Localization.Caching
{
    /// <summary>
    /// Represents a locale string resource cache event consumer
    /// </summary>
    public partial class LocaleStringResourceCacheEventConsumer : CacheEventConsumer<LocaleStringResource>
    {
        /// <summary>
        /// Clear cache by entity event type
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(LocaleStringResource entity)
        {
            await RemoveAsync(LocalizationDefaults.LocaleStringResourcesAllPublicCacheKey, entity.LanguageId);
            await RemoveAsync(LocalizationDefaults.LocaleStringResourcesAllAdminCacheKey, entity.LanguageId);
            await RemoveAsync(LocalizationDefaults.LocaleStringResourcesAllCacheKey, entity.LanguageId);
            await RemoveByPrefixAsync(LocalizationDefaults.LocaleStringResourcesByNamePrefix, entity.LanguageId);
        }
    }
}