using ARWNI2S.Engine.Simulation.Runtime.Update;

namespace ARWNI2S.Engine.Simulation
{
    public abstract class SimulableBase : ISimulable
    {
        private UpdateFunction _updateFunction;

        public virtual Guid UUID { get; protected set; }

        UpdateFunction ISimulable.UpdateFunction => _updateFunction;
    }
}
