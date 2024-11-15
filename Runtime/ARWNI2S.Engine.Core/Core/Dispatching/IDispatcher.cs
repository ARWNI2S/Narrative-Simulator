using ARWNI2S.Infrastructure.Engine;

namespace ARWNI2S.Engine.Core.Dispatching
{
    public interface IDispatcher
    {
        void ScheduleEvent(IEvent @event);
    }
}
