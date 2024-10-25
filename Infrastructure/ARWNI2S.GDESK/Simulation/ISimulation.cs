using ARWNI2S.Engine.Simulation.Time;
using Microsoft.Extensions.Logging;

namespace ARWNI2S.Engine.Simulation
{
    public interface ISimulation
    {
        ISimulationClock Clock { get; }

        ILogger Logger { get; }


        void StartSimulation(CancellationToken? cancellationToken = null);

        // Method to stop the loop gracefully
        void StopSimulation();

    }
}
