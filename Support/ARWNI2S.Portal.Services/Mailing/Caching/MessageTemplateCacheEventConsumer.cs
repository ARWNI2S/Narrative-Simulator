using ARWNI2S.Node.Services.Caching;
using ARWNI2S.Portal.Services.Entities.Mailing;

namespace ARWNI2S.Portal.Services.Mailing.Caching
{
    /// <summary>
    /// Represents a message template cache event consumer
    /// </summary>
    public partial class MessageTemplateCacheEventConsumer : CacheEventConsumer<MessageTemplate>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(MessageTemplate entity)
        {
            await RemoveByPrefixAsync(MessageServicesDefaults.MessageTemplatesByNamePrefix, entity.Name);
        }
    }
}
