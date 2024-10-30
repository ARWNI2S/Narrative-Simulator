using ARWNI2S.Engine.Configuration;
using ARWNI2S.Infrastructure.Memory;

namespace ARWNI2S.Engine.Simulation.Kernel
{
    internal sealed class EventPool : ObjectPool<Event>
    {
        private Dispatcher _dispatcher;

        public EventPool(Dispatcher dispatcher, GDESKConfig config)
            : base(config.EventPoolMaxSize, config.EventPoolWarmupSize)
        {
            
        }

        protected override Event CreateNewPoolObject()
        {
            return new Event(_dispatcher, this);
        }
    }
}
