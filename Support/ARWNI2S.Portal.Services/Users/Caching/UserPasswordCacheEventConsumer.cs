using ARWNI2S.Node.Services.Caching;
using ARWNI2S.Portal.Services.Entities.Users;

namespace ARWNI2S.Portal.Services.Users.Caching
{
    /// <summary>
    /// Represents a user password cache event consumer
    /// </summary>
    public partial class UserPasswordCacheEventConsumer : CacheEventConsumer<UserPassword>
    {
    }
}