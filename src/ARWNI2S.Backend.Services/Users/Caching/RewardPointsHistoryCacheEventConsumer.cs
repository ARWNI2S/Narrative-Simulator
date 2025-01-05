using ARWNI2S.Backend.Services.Users.Entities;
using ARWNI2S.Engine.Data.Events;

namespace ARWNI2S.Backend.Services.Users.Caching
{
    /// <summary>
    /// Represents a reward point history cache event consumer
    /// </summary>
    public partial class RewardPointsHistoryCacheEventConsumer : CacheEventConsumer<RewardPointsHistory>
    {
    }
}