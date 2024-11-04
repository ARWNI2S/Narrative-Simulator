
using ARWNI2S.Engine.Configuration;

namespace ARWNI2S.Engine.Simulation.Runtime
{
    public interface ISimulableRuntime
    {
        /// <summary>
        /// Initializes the simulation update cycle runtime
        /// </summary>
        void InitializeRuntime(GDESKConfig configuration);

        void Start(CancellationToken? token = null);

        public void Stop();
    }
}
