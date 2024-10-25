using ARWNI2S.Engine.Infrastructure;
using ARWNI2S.Engine.Simulation;
using ARWNI2S.Engine.Simulation.Kernel;
using ARWNI2S.Engine.Simulation.Time;
using Microsoft.Extensions.Logging;

namespace ARWNI2S.Node.Core.Infrastructure
{
    internal class NarratorEngine : SimulationBase, ISimulation, ILifecycleParticipant<ISiloLifecycle>
    {
        public NarratorEngine(Dispatcher dispatcher, FrameTaskScheduler frameTaskScheduler, ISimulationClock clock,
            ILogger<NarratorEngine> logger) : base(dispatcher, frameTaskScheduler, clock, logger) { }

        public void Participate(ISiloLifecycle lifecycle)
        {
            lifecycle.Subscribe(
                nameof(NarratorEngine),
                ServiceLifecycleStage.Last,
                OnStartAsync,
                OnStopAsync);
        }

        // This method will be invoked when the silo is ready to serve requests
        protected Task OnStartAsync(CancellationToken cancellationToken)
        {
            StartSimulation(cancellationToken);
            return Task.CompletedTask;
        }

        protected Task OnStopAsync(CancellationToken token)
        {
            StopSimulation();
            // You can trigger any other custom logic here
            return Task.CompletedTask;
        }

    }
}
