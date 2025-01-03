using ARWNI2S.Engine.Data.Events;
using ARWNI2S.Framework.Users.Entities;

namespace ARWNI2S.Framework.Users.Caching
{
    /// <summary>
    /// Represents a user password cache event consumer
    /// </summary>
    public partial class UserPasswordCacheEventConsumer : CacheEventConsumer<UserPassword>
    {
    }
}