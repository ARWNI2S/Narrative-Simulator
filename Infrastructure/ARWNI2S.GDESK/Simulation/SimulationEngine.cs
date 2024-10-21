
namespace ARWNI2S.Engine.Simulation
{
    public struct SimulationStartResult
    {
        public bool Success { get; internal set; }
    }

    internal abstract class SimulationBase
    {
        internal virtual void Initialize(CancellationToken cancellationToken)
        {

        }

        internal virtual void PostInitialize(CancellationToken cancellationToken)
        {

        }

        internal virtual void PreInitialize(CancellationToken cancellationToken)
        {

        }

        public SimulationStartResult Start(CancellationToken stoppingToken)
        {
            return new SimulationStartResult { Success = true };
        }
    }
}