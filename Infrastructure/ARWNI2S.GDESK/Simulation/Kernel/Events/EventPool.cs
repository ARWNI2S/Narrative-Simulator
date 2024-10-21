using ARWNI2S.Infrastructure.Memory;

namespace ARWNI2S.Engine.Simulation.Kernel.Events
{
    internal class EventPool : ObjectPool<SimulationEvent>, IEventPool
    {
        protected override SimulationEvent CreateNewPoolObject()
        {
            return new SimulationEvent();
        }

        protected override void ReleaseObject(SimulationEvent obj)
        {
            base.ReleaseObject(obj);
        }
    }
}
