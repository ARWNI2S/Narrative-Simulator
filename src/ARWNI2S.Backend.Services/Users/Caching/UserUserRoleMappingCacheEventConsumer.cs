using ARWNI2S.Backend.Services.Users.Entities;
using ARWNI2S.Engine.Data.Events;

namespace ARWNI2S.Backend.Services.Users.Caching
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
            await RemoveAsync(UserServicesDefaults.UserRolesCacheKey, entity.UserId);
        }
    }
}