using ARWNI2S.Engine.Simulation.Runtime.Update;

namespace ARWNI2S.Engine.Simulation
{
    public interface ISimulable
    {
        Guid UUID { get; }

        internal UpdateFunction UpdateFunction { get; }
    }
}
