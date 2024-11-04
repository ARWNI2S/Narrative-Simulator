using ARWNI2S.Engine.Configuration;
using ARWNI2S.Engine.Simulation.Time;
using ARWNI2S.Infrastructure.Timing;

namespace ARWNI2S.Engine.Simulation.Runtime.Update
{
    internal class UpdateProcessor : IDisposable
    {
        private readonly ISimulableRuntime _runtime;
        private readonly ISimulationClock _clock;
        private readonly UpdateFunctionRing _updateRing;

        private readonly Thread _updateThread;
        private readonly Thread _workerThread;

        #region Statistics

        private double cyclesPerSecond = 0;

        #endregion

        public ISimulationClock Clock { get { return _clock; } }

        public UpdateProcessor(GDESKConfig config, ISimulableRuntime runtime, UpdateFunctionRing updateRing, ISimulationClock clock)
        {
            _runtime = runtime;
            _updateRing = updateRing;

            _clock = clock;

            // Create the dedicated threads
            _updateThread = new Thread((_) => UpdateLoop(cancelSource.Token))
            {
                Name = GetType().Name + "_UpdateThread",
                Priority = ThreadPriority.Highest,
                IsBackground = true // Mark thread as background so it won't block app shutdown
            };
            _workerThread = new Thread((_) => WorkerLoop(cancelSource.Token))
            {
                Name = GetType().Name + "_WorkerLoop",
                Priority = ThreadPriority.AboveNormal,
                IsBackground = true // Mark thread as background so it won't block app shutdown
            };
        }

        internal void Initialize(UpdateFrameRoot frameRoot)
        {
            _updateRing.Initialize(frameRoot);
        }

        // Method to start the loop on a dedicated thread
        public void Start(CancellationToken? cancellationToken = null)
        {
            cancelSource = cancellationToken.HasValue ? new CancellationTokenSource() :
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken.Value);

            // Start the dedicated thread
            _workerThread.Start(cancelSource.Token);
            _updateThread.Start(cancelSource.Token);
        }

        // Method to stop the loop gracefully
        public void Stop()
        {
            if (_updateThread == null)
                return;

            // Signal cancellation
            cancelSource.Cancel();

            lock (_lock)
            {
                Monitor.Pulse(_lock); // Despierta el hilo si está esperando
            }

            // Wait for the thread to complete
            _updateThread.Join();
        }

        #region Update Function Ring


        private UpdateFunction frameRoot;
        private UpdateFunction cycleRoot;
        private UpdateFunction nextCycleRoot;

        protected virtual UpdateFunction UpdateFrameRoot()
        {
            return _updateRing.GetNextFrameRoot();
        }

        protected virtual UpdateFunction PrepareNextCycle()
        {
            return _updateRing.GetNextCycleRoot();
        }

        #endregion

        #region Threads

        private readonly object _lock = new();
        protected CancellationTokenSource cancelSource;

        #region Update Cycle Loop

        private readonly HiResTimer _timer = new();

        private void UpdateLoop(CancellationToken token)
        {
            const int oneSecondMs = 1000; // 1000 ms

            double lastFrameStartMs = .0;
            double lastCycleStartMs = .0;
            double timeCountMs = .0;
            int cycleCount = 0;

            lock (_awaiter)
            {
                frameRoot = UpdateFrameRoot();
                nextCycleRoot = PrepareNextCycle();
            }

            _timer.Start();

            while (!token.IsCancellationRequested)
            {
                //Frame preparation tasks (high priority do once per frame)
                foreach (var update in _updateRing)
                {
                    update.InternalData?.Task?.RunSynchronously();
                }

                lock (_awaiter)
                {
                    cycleRoot = nextCycleRoot;
                    Monitor.Pulse(_awaiter);
                }

                while (cycleRoot != frameRoot)
                {
                    foreach (var update in _updateRing)
                    {
                        update.InternalData?.Task?.RunSynchronously();
                    }

                    lock (_awaiter)
                    {
                        cycleRoot = nextCycleRoot;
                        Monitor.Pulse(_awaiter);
                    }

                    var cycleElapsedTimeMs = _timer.GetTimeMs() - lastCycleStartMs;
                    lastCycleStartMs += cycleElapsedTimeMs;

                    cycleCount++;
                    timeCountMs += cycleElapsedTimeMs;
                    if (timeCountMs > oneSecondMs)
                    {
                        timeCountMs -= oneSecondMs;


                        cyclesPerSecond = cycleCount;
                        cycleCount = 0;
                        Console.WriteLine($"Cycles/s: {cyclesPerSecond:F2}");
                    }
                }

                var frameElapsedTimeMs = _timer.GetTimeMs() - lastFrameStartMs;
                lastFrameStartMs += frameElapsedTimeMs;
            }
        }

        #endregion

        #region Dynamic Maintenance

        private readonly object _awaiter = new();

        private void WorkerLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                lock (_awaiter)
                {
                    frameRoot = UpdateFrameRoot();
                    nextCycleRoot = PrepareNextCycle();

                    Monitor.Wait(_awaiter);
                }
            }
        }

        #endregion

        #endregion

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
