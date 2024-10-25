using ARWNI2S.Engine.Simulation.Kernel;

namespace ARWNI2S.Engine.Simulation
{
    internal interface ISimulable
    {
        void HandleEvent(Event next);
    }
}