using ARWNI2S.Node.Services.Caching;
using ARWNI2S.Portal.Services.Entities.Media;

namespace ARWNI2S.Portal.Services.Media.Caching
{
    /// <summary>
    /// Represents a picture cache event consumer
    /// </summary>
    public partial class PictureCacheEventConsumer : CacheEventConsumer<Picture>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(Picture entity)
        {
            await RemoveByPrefixAsync(MediaServicesDefaults.ThumbsExistsPrefix);
        }
    }
}
