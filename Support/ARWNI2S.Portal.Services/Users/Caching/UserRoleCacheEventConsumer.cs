using ARWNI2S.Node.Core.Entities.Users;
using ARWNI2S.Node.Services.Caching;
using ARWNI2S.Node.Services.Users;

namespace ARWNI2S.Portal.Services.Users.Caching
{
    /// <summary>
    /// Represents a user role cache event consumer
    /// </summary>
    public partial class UserRoleCacheEventConsumer : CacheEventConsumer<UserRole>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(UserRole entity)
        {
            await RemoveByPrefixAsync(UserServicesDefaults.UserRolesBySystemNamePrefix);
            await RemoveByPrefixAsync(UserServicesDefaults.UserUserRolesPrefix);
        }
    }
}
