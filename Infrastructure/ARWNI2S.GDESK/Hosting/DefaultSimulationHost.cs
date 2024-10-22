using ARWNI2S.Engine.Simulation;
using Microsoft.Extensions.Logging;

namespace ARWNI2S.Engine.Hosting
{
    internal class DefaultSimulationHost : SimulationHostedServiceBase<DefaultSimulation>
    {
        public DefaultSimulationHost(DefaultSimulation simulation) : base(simulation)
        {
        }
    }
}
