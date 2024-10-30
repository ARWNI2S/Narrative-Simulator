using ARWNI2S.Engine.Simulation.Entities;
using ARWNI2S.Runtime.Simulation.Actors.Grains;

namespace ARWNI2S.Node.Core.Actors.Grains
{
    internal class NI2SActorGrain : Grain, INI2SActorGrain, IGameObject
    {
        public Guid UUID => GrainContext.GrainId.GetGuidKey();

        public override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            return base.OnActivateAsync(cancellationToken);
        }

        public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
        {
            return base.OnDeactivateAsync(reason, cancellationToken);
        }

        // Not here yet.
        void IGameObject.InitializeGameObject(IGameObjectBuilder objectBuilder) => throw new NotImplementedException();
    }
}
