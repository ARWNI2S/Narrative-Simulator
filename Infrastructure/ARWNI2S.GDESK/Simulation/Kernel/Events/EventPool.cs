using ARWNI2S.Infrastructure.Memory;

namespace ARWNI2S.Engine.Simulation.Kernel.Events
{
    internal class EventPool : ObjectPool<Event>, IEventPool
    {
        protected override Event CreateNewPoolObject()
        {
            return new Event();
        }

        protected override void ReleaseObject(Event obj)
        {
            base.ReleaseObject(obj);
        }
    }
}
