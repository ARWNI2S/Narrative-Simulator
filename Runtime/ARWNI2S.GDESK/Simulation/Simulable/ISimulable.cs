using ARWNI2S.Engine.Simulation.Runtime.Update;
using ARWNI2S.Engine.Simulation.Simulable.Builder;

namespace ARWNI2S.Engine.Simulation.Simulable
{
    public interface ISimulable
    {
        Guid UUID { get; }
        string Name { get; }
        ISimulable Outer { get; }

        internal UpdateFunction UpdateFunction { get; }

        internal void InitializeInternal(ISimulableObjectBuilder objectBuilder);
    }
}
