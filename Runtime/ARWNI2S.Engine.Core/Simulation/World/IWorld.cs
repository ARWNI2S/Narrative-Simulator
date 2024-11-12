using ARWNI2S.Engine.Simulation.Simulable;
using ARWNI2S.Engine.Simulation.World.Composition;

namespace ARWNI2S.Engine.Simulation.World
{
    public interface IWorld : ISimulable
    {
        WorldTreeEventHandler WorldTraceHandler { get; }

        IList<ISimulable> AdditionalSimulables { get; }

        WorldTreeRoot WorldRoot { get; }
    }
}
