using ARWNI2S.Infrastructure.Memory;

namespace ARWNI2S.Engine.Simulation.Kernel
{
    internal class EventPool : ObjectPool<Event>
    {
        protected override Event CreateNewPoolObject()
        {
            return new Event();
        }
    }
}
