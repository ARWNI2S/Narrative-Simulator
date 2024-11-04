using ARWNI2S.Engine.Simulation.Simulable.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ARWNI2S.Engine.Simulation
{
    internal class SimulationContext
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            Context = new SimulationContext(serviceProvider);
        }

        public static SimulationContext Context { get; private set; }

        private IServiceProvider _globalServiceProvider;
        private ISimulation _simulation;

        private SimulationContext(IServiceProvider serviceProvider)
        {
            _globalServiceProvider = serviceProvider;
        }

        public ISimulation Simulation => _simulation ??= _globalServiceProvider.GetRequiredService<ISimulation>();

        internal ISimulableObjectBuilder GetContextBuilder()
        {
            return null;
        }

    }
}
