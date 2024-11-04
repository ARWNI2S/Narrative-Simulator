using Microsoft.Extensions.DependencyInjection;

namespace ARWNI2S.Engine.Simulation.Builder
{
    public class SimulationBuilder : ISimulationBuilder
    {
        private readonly IServiceCollection _services = new ServiceCollection();

        public IServiceCollection Services => _services;

        public IServiceProvider BuildProvider()
        {
            return _services.BuildServiceProvider();
        }
    }
}
