using ARWNI2S.Engine.Simulation.Kernel;
using ARWNI2S.Engine.Simulation.Runtime;
using ARWNI2S.Engine.Simulation.Time;
using Microsoft.Extensions.Logging;

namespace ARWNI2S.Engine.Simulation
{
    internal sealed class DefaultSimulation : SimulationBase, ISimulation
    {
        public DefaultSimulation(
            Dispatcher dispatcher,
            IGameRuntime entityRuntime,
            ISimulationClock clock,
            ILogger<DefaultSimulation> logger)
            : base(dispatcher,
                  entityRuntime,
                  clock,
                  logger)
        {
        }
    }
}
