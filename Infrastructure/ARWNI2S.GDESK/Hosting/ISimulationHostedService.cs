using ARWNI2S.Engine.Simulation;
using Microsoft.Extensions.Hosting;

namespace ARWNI2S.Engine.Hosting
{
    internal interface ISimulationHostedService : IHostedService
    {
        SimulationBase Simulation { get; }

    }

    internal interface ISimulationHostedService<TSimulation> : IHostedService where TSimulation : SimulationBase
    {
        TSimulation Simulation { get; }
    }
}
