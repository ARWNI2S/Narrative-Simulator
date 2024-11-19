using ARWNI2S.Engine.Core.Dispatching;
using ARWNI2S.Infrastructure.Memory;

namespace ARWNI2S.Engine.Memory
{
    internal class EventPool : ObjectPool<SimEvent>
    {
        protected override SimEvent CreateNewPoolObject() => new();
    }
}
