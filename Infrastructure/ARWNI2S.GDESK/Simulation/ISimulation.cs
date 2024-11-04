using ARWNI2S.Engine.Simulation.Runtime;
using ARWNI2S.Engine.Simulation.Time;
using ARWNI2S.Engine.Simulation.World;

namespace ARWNI2S.Engine.Simulation
{
    public interface ISimulation : IDisposable
    {
        /// <summary>
        /// Gets the global simulation world
        /// </summary>
        IWorld World { get; }

        /// <summary>
        /// Gets the global simulation clock
        /// </summary>
        ISimulationClock Clock { get; }

        /// <summary>
        /// Gets the local node entity runtime
        /// </summary>
        ISimulableRuntime Runtime { get; }

        /// <summary>
        /// Initializes the simulation
        /// </summary>
        void InitializeSimulation();

        /// <summary>
        /// Method to start and run the simulation.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        void StartSimulation(CancellationToken? cancellationToken = null);

        /// <summary>
        /// Method to stop the simulation gracefully
        /// </summary>
        void StopSimulation();


    }
}
