using ARWNI2S.Engine.Configuration;
using ARWNI2S.Engine.Simulation.Kernel;
using ARWNI2S.Engine.Simulation.Runtime;
using ARWNI2S.Engine.Simulation.Time;
using ARWNI2S.Engine.Simulation.World;
using Microsoft.Extensions.Logging;

namespace ARWNI2S.Engine.Simulation
{
    internal sealed class DefaultSimulation : SimulationBase, ISimulation
    {
        public DefaultSimulation(
            GDESKConfig configuration,
            Dispatcher dispatcher,
            ISimulableRuntime simulableRuntime,
            ISimulationClock clock,
            ILogger<DefaultSimulation> logger)
            : base(configuration,
                  dispatcher,
                  simulableRuntime,
                  clock,
                  logger)
        {
        }

        protected override IWorld GetWorld()
        {
            throw new NotImplementedException();
        }
    }
}
