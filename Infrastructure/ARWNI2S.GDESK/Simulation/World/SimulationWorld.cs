using ARWNI2S.Engine.Simulation.Runtime.Update;
using ARWNI2S.Engine.Simulation.Simulable;
using ARWNI2S.Engine.Simulation.World.Composition;
using ARWNI2S.Infrastructure;

namespace ARWNI2S.Engine.Simulation.World
{
    public class SimulationWorld : SimulableBase, IWorld
    {
        public override Guid UUID => Constants._UUID_WORLD_ACTOR;
        
        public virtual WorldTreeEventHandler WorldTraceHandler { get; protected internal set; }

        public virtual IList<ISimulable> AdditionalSimulables { get; protected internal set; }

        public virtual WorldTreeRoot WorldRoot { get; protected internal set; }

        public SimulationWorld()
        {






            updateFunction = new UpdateFrameRoot();
        }


    }
}
