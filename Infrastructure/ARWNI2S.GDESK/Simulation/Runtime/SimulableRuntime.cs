using ARWNI2S.Engine.Simulation.Runtime.Update;
using ARWNI2S.Engine.Simulation.Time;

namespace ARWNI2S.Engine.Simulation.Runtime
{
    internal class SimulableRuntime : ISimulableRuntime, IDisposable
    {
        private readonly UpdateProcessor _updateProcessor;
        private readonly ISimulationClock _clock;

        protected CancellationTokenSource cancelSource;

        public ISimulationClock Clock { get { return _clock; } }

        public SimulableRuntime(UpdateProcessor updateProcessor, ISimulationClock clock)
        {
            _updateProcessor = updateProcessor;

            _clock = clock;
        }

        // Method to start the loop on a dedicated thread
        public void Start(CancellationToken? cancellationToken = null)
        {
            cancelSource = cancellationToken.HasValue ? new CancellationTokenSource() :
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken.Value);

            // Start the dedicated thread
            _updateProcessor.Start(cancelSource.Token);
        }

        // Method to stop the loop gracefully
        public void Stop()
        {
            if (_updateProcessor == null)
                return;

            _updateProcessor.Stop();

            // Signal cancellation
            cancelSource.Cancel();
        }

        #region IDisposable

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: eliminar el estado administrado (objetos administrados)
                }

                // TODO: liberar los recursos no administrados (objetos no administrados) y reemplazar el finalizador
                // TODO: establecer los campos grandes como NULL
                disposedValue = true;
            }
        }

        // // TODO: reemplazar el finalizador solo si "Dispose(bool disposing)" tiene código para liberar los recursos no administrados
        // ~EntityRuntime()
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

        #endregion

    }
}
