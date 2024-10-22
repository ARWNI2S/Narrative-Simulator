using ARWNI2S.Engine.Simulation.Kernel.Events;

namespace ARWNI2S.Engine.Simulation.Kernel
{
    internal interface IDispatcher
    {
        void PushEvent(Event @event);
    }
}