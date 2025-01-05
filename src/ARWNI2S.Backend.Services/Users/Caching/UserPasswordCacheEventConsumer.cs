using ARWNI2S.Backend.Services.Users.Entities;
using ARWNI2S.Engine.Data.Events;

namespace ARWNI2S.Backend.Services.Users.Caching
{
    /// <summary>
    /// Represents a user password cache event consumer
    /// </summary>
    public partial class UserPasswordCacheEventConsumer : CacheEventConsumer<UserPassword>
    {
    }
}