using ARWNI2S.Engine.Simulation.Time;
using ARWNI2S.Infrastructure.Timing;
using System.Diagnostics;

namespace ARWNI2S.Engine.Simulation.Runtime
{
    internal class GameRuntime : IGameRuntime, IDisposable
    {
        private readonly ISimulationClock _clock;

        protected CancellationTokenSource cancelSource;

        public ISimulationClock Clock { get { return _clock; } }

        public GameRuntime(ISimulationClock clock)
        {
            _clock = clock;

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
            cancelSource.Cancel();

            lock (_lock)
            {
                Monitor.Pulse(_lock); // Despierta el hilo si está esperando
            }

            // Wait for the thread to complete
            _workerThread.Join();
        }

        private readonly object _lock = new();

        private readonly HiResTimer _timer = new();
        private readonly Thread _workerThread;

        private void ThreadLoop(CancellationToken token)
        {
            _timer.Start();

            while (token.IsCancellationRequested)
            {
                var frameStartMs = _timer.GetTimeMs();
            }
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









        private long fpsStartTime;
        private long fpsFrameCount;

        public virtual void LimitFrameRate(int fps)
        {
            long freq = Stopwatch.Frequency;
            long frame = Stopwatch.GetTimestamp();
            while ((frame - fpsStartTime) * fps < freq * fpsFrameCount)
            {
                int sleepTime = (int)((fpsStartTime * fps + freq * fpsFrameCount - frame * fps) * 1000 / (freq * fps));
                if (sleepTime > 0) Thread.Sleep(sleepTime);
                frame = Stopwatch.GetTimestamp();
            }
            if (++fpsFrameCount > fps)
            {
                fpsFrameCount = 0;
                fpsStartTime = frame;
            }
        }

    }
}
