using ARWNI2S.Node.Data.Entities.Users;
using ARWNI2S.Node.Services.Caching;
using ARWNI2S.Node.Services.Users;

namespace ARWNI2S.Portal.Services.Users.Caching
{
    /// <summary>
    /// Represents a user user role mapping cache event consumer
    /// </summary>
    public partial class UserUserRoleMappingCacheEventConsumer : CacheEventConsumer<UserUserRoleMapping>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(UserUserRoleMapping entity)
        {
            await RemoveByPrefixAsync(UserServicesDefaults.UserUserRolesPrefix);
        }
    }
}