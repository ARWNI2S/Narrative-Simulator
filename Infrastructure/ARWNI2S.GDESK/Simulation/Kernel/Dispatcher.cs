using ARWNI2S.Engine.Simulation.Time;
using ARWNI2S.Infrastructure.Timing;

namespace ARWNI2S.Engine.Simulation.Kernel
{
    internal sealed class Dispatcher : IDisposable
    {
        #region Statistics

        private DateTime startTime;

        #endregion

        private readonly ISimulationClock _clock;
        private readonly double _resolution;

        protected CancellationTokenSource cancelSource;
        private bool disposedValue;

        public ISimulationClock Clock { get { return _clock; } }

        public Dispatcher(ISimulationClock clock)
        {
            _clock = clock;
            _resolution = _clock.GetResolution();

            // Create the dedicated thread
            _workerThread = new Thread((_) => ThreadLoop(cancelSource.Token))
            {
                Name = GetType().Name + "_MainThread",
                Priority = ThreadPriority.AboveNormal,
                IsBackground = true // Mark thread as background so it won't block app shutdown
            };
        }

        // Method to start the loop on a dedicated thread
        public void Start(CancellationToken? cancellationToken = null)
        {
            cancelSource = cancellationToken.HasValue ? new CancellationTokenSource() :
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken.Value);

            // Start the dedicated thread
            _workerThread.Start(cancelSource.Token);
        }

        // Method to stop the loop gracefully
        public void Stop()
        {
            if (_workerThread == null)
                return;

            // Signal cancellation
            if (cancelSource != null)
                cancelSource.Cancel();

            lock (_lock)
            {
                Monitor.Pulse(_lock); // Despierta el hilo si está esperando
            }

            // Wait for the thread to complete
            _workerThread.Join();
        }

        #region Event Queue

        private readonly PriorityQueue<Event, double> _eventQueue = new();

        public void ScheduleEvent(Event @event)
        {
            lock (_lock)
            {
                if (@event == null) return;
                _eventQueue.Enqueue(@event, _timer.GetTimeMs() + (@event.Time / _resolution));
                Monitor.Pulse(_lock); // Despierta el hilo si está esperando
            }
        }

        #endregion

        #region Dispacher Loop

        private readonly object _lock = new();

        private readonly HiResTimer _timer = new();
        private readonly Thread _workerThread;

        private void ThreadLoop(CancellationToken cancellationToken)
        {
            startTime = DateTime.UtcNow;
            _timer.Start();
            _clock.Start();

            var currentTime = _timer.GetTimeMs();
            var lastTime = currentTime;

            while (!cancellationToken.IsCancellationRequested)
            {
                var elapsedTimeMs = lastTime - currentTime;

                lock (_lock)
                {
                    if (_eventQueue.TryPeek(out Event next, out double timestamp))
                    {
                        if (timestamp <= _timer.GetTimeMs())
                        {
                            _eventQueue.Dequeue();
                            _clock.Advance(elapsedTimeMs);
                            next.Execute();
                            next.Dispose();
                        }
                        else
                            Monitor.Wait(_lock, (int)(timestamp - _timer.GetTimeMs()));
                    }
                    else
                        Monitor.Wait(_lock); // Espera si no hay eventos
                }

                lastTime = currentTime;
                currentTime = _timer.GetTimeMs();
            }

        }

        #endregion

        #region IDisposable implementation

        private void Dispose(bool disposing)
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

        public void Dispose()
        {
            // No cambie este código. Coloque el código de limpieza en el método "Dispose(bool disposing)".
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
