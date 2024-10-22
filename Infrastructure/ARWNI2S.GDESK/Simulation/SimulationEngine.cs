

namespace ARWNI2S.Engine.Simulation
{
    public struct SimulationStartResult
    {
        public static readonly SimulationStartResult Success = new() { Failed = false };
        public static readonly SimulationStartResult Failure = new() { Failed = true };

        public bool Failed { get; internal set; }
    }

    internal abstract class SimulationBase : IDisposable
    {
        private bool disposedValue;

        internal virtual void StopPersist(CancellationToken cancellationToken)
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
            return SimulationStartResult.Success;
        }

        internal void DeInitialize(CancellationToken cancellationToken)
        {
        }

        internal void SaveGame(CancellationToken cancellationToken)
        {
        }

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

        // TODO: reemplazar el finalizador solo si "Dispose(bool disposing)" tiene código para liberar los recursos no administrados
        ~SimulationBase()
        {
            // No cambie este código. Coloque el código de limpieza en el método "Dispose(bool disposing)".
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // No cambie este código. Coloque el código de limpieza en el método "Dispose(bool disposing)".
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}