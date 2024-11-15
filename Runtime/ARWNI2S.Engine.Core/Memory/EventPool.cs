using ARWNI2S.Infrastructure.Engine;
using ARWNI2S.Infrastructure.Memory;

namespace ARWNI2S.Engine.Memory
{
    internal class EventPool : ObjectPool<IEvent>
    {
        protected override IEvent CreateNewPoolObject()
        {
            throw new NotImplementedException();
        }
    }
}
