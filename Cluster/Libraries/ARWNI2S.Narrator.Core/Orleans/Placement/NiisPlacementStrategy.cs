using Orleans.Placement;

namespace ARWNI2S.Engine.Orleans.Placement
{
    [Serializable]
    public sealed class NiisPlacementStrategy : PlacementStrategy
    {
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class NiisPlacementStrategyAttribute : PlacementAttribute
    {
        public NiisPlacementStrategyAttribute() :
            base(new NiisPlacementStrategy())
        {
        }
    }
}
