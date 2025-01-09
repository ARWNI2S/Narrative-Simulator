using ARWNI2S.Engine.Core;
using ARWNI2S.Engine.Data;
using Orleans;

namespace ARWNI2S.Runtime.Narrator
{
    internal class ActorGrain<TState> : Grain<TState>, INiisGrain
        where TState : DataEntity
    {
        public override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            return base.OnActivateAsync(cancellationToken);
        }

        public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
        {
            return base.OnDeactivateAsync(reason, cancellationToken);
        }

        protected override Task ReadStateAsync()
        {
            return base.ReadStateAsync();
        }
        protected override Task ClearStateAsync()
        {
            return base.ClearStateAsync();
        }
        protected override Task WriteStateAsync()
        {
            return base.WriteStateAsync();
        }
    }
}
