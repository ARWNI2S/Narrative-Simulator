using ARWNI2S.Engine.Simulation.Kernel;
using ARWNI2S.Engine.Simulation.Runtime;
using ARWNI2S.Engine.Simulation.Time;
using Microsoft.Extensions.Logging;

namespace ARWNI2S.Engine.Simulation
{
    public enum SimulationLOD : int
    {
        Disabled = -1,
        LOD_0 = 0,
        LOD_1 = LOD_0 + 1,
        LOD_2 = LOD_0 + 2,
        LOD_3 = LOD_0 + 3,
        LOD_4 = LOD_0 + 4,
        LOD_5 = LOD_0 + 5,
        LOD_6 = LOD_0 + 6,
        LOD_7 = LOD_0 + 7,
        LOD_8 = LOD_0 + 8,
        LOD_9 = LOD_0 + 9,
        LOD_10 = LOD_0 + 10,
        LOD_CUSTOM = LOD_10 + 1,
        LOD_MAX = int.MaxValue,
    }

    public abstract class SimulationBase : ISimulation
    {
        private readonly Dispatcher _dispatcher;
        private readonly ISimulableRuntime _simulableRuntime;
        private readonly ISimulationClock _clock;

        private readonly ILogger _logger;

        private CancellationTokenSource mainCancelSource;
        private bool disposedValue;

        /// <summary>
        /// Gets a value indicating if the simulation is correctly initialized.
        /// </summary>
        /// <remarks>Uninitialized simulation won't run.</remarks>
        public bool IsInitialized { get; protected set; }

        internal SimulationBase(Dispatcher dispatcher, ISimulableRuntime simulableRuntime, ISimulationClock clock, ILogger logger)
        {
            _dispatcher = dispatcher;
            _simulableRuntime = simulableRuntime;

            _clock = clock;
            _logger = logger;
        }

        /// <inheritdoc />
        public ISimulationClock Clock => _clock;

        /// <inheritdoc />
        public ISimulableRuntime Runtime => _simulableRuntime;

        /// <inheritdoc />
        protected void InitializeSimulation() { IsInitialized = Initialize(); }

        /// <inheritdoc />
        protected virtual void StartSimulation(CancellationToken? cancellationToken = null)
        {
            if (!IsInitialized)
                throw new InvalidOperationException($"{GetType().Name} not initialized");

            mainCancelSource = cancellationToken.HasValue ? new CancellationTokenSource() :
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken.Value);

            // Start the dedicated threads
            _simulableRuntime.Start(mainCancelSource.Token);
            _dispatcher.Start(mainCancelSource.Token);
            _logger.LogInformation("Simulation has started.");
        }

        /// <inheritdoc />
        protected virtual void StopSimulation()
        {
            if (_dispatcher != null)
            {
                _logger.LogInformation("Stopping event dispatcher...");
                _dispatcher.Stop();
            }

            // Signal cancellation
            mainCancelSource.Cancel();

            _logger.LogInformation("Simulation has exited.");
        }

        /// <summary>
        /// Default initialization method.
        /// </summary>
        /// <returns>true if the initialization was successful, otherwise false.</returns>
        /// <remarks>Always returns true if not overloaded.</remarks>
        protected virtual bool Initialize() { return true; }

        /// <inheritdoc />
        void ISimulation.InitializeSimulation() => InitializeSimulation();

        /// <inheritdoc />
        void ISimulation.StartSimulation(CancellationToken? cancellationToken) => StartSimulation(cancellationToken);

        /// <inheritdoc />
        void ISimulation.StopSimulation() => StopSimulation();

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _dispatcher.Dispose();
                }

                // TODO: liberar los recursos no administrados (objetos no administrados) y reemplazar el finalizador
                disposedValue = true;
            }
        }

        // // TODO: reemplazar el finalizador solo si "Dispose(bool disposing)" tiene código para liberar los recursos no administrados
        // ~SimulationBase()
        // {
        //     // No cambie este código. Coloque el código de limpieza en el método "Dispose(bool disposing)".
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // No cambie este código. Coloque el código de limpieza en el método "Dispose(bool disposing)".
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

    }
}
