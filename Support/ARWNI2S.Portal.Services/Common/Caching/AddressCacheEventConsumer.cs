using ARWNI2S.Node.Services.Caching;
using ARWNI2S.Node.Services.Users;
using ARWNI2S.Portal.Services.Entities.Common;

namespace ARWNI2S.Portal.Services.Common.Caching
{
    /// <summary>
    /// Represents a address cache event consumer
    /// </summary>
    public partial class AddressCacheEventConsumer : CacheEventConsumer<Address>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(Address entity)
        {
            await RemoveByPrefixAsync(UserServicesDefaults.UserAddressesPrefix);
        }
    }
}
