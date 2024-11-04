using ARWNI2S.Engine.Configuration;
using ARWNI2S.Engine.Simulation;
using ARWNI2S.Engine.Simulation.Kernel;
using ARWNI2S.Engine.Simulation.Runtime;
using ARWNI2S.Engine.Simulation.Time;
using ARWNI2S.Engine.Simulation.World;
using Microsoft.Extensions.Logging;

namespace ARWNI2S.Node.Core.Infrastructure
{
    internal class NarratorEngine : SimulationBase, ISimulation, ILifecycleParticipant<ISiloLifecycle>
    {
        public NarratorEngine(GDESKConfig configuration, Dispatcher dispatcher, ISimulableRuntime simulableRuntime, ISimulationClock clock,
            ILogger<NarratorEngine> logger) : base(configuration, dispatcher, simulableRuntime, clock, logger) { }

        public void Participate(ISiloLifecycle lifecycle)
        {
            lifecycle.Subscribe(
                nameof(NarratorEngine),
                ServiceLifecycleStage.Last,
                OnStartAsync,
                OnStopAsync);
        }

        // This method will be invoked when the silo is ready to serve requests
        protected async Task OnStartAsync(CancellationToken cancellationToken)
        {
            await InitializeSimulationAsync(cancellationToken);

            StartSimulation(cancellationToken);
        }

        protected Task OnStopAsync(CancellationToken token)
        {
            StopSimulation();
            // You can trigger any other custom logic here
            return Task.CompletedTask;
        }

        private async Task InitializeSimulationAsync(CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                InitializeSimulation();
            }, cancellationToken);
        }

        /// <summary>
        /// Narrator engine initialization.
        /// </summary>
        /// <returns>true if the initialization was successful, otherwise false.</returns>
        protected override bool Initialize()
        {
            return base.Initialize();
        }

        protected override IWorld GetWorld()
        {
            throw new NotImplementedException();
        }
    }
}
