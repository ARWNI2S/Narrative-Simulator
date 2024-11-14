using Orleans.Runtime.Placement;

namespace ARWNI2S.Engine.Orleans.Placement
{
    internal class NiisPlacementDirector : IPlacementDirector
    {
        public Task<SiloAddress> OnAddActivation(PlacementStrategy strategy, PlacementTarget target, IPlacementContext context)
        {
            var silos = context.GetCompatibleSilos(target).OrderBy(s => s).ToArray();
            int silo = GetNearestPlacementSiloOrFallback(target.GrainIdentity.GetGuidKey(), silos.Length);

            return Task.FromResult(silos[silo]);
        }

        private int GetNearestPlacementSiloOrFallback(Guid guid, int length)
        {
            return 0;
        }
    }
}
