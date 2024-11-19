using ARWNI2S.Engine.Core.Threading;
using ARWNI2S.Engine.Memory;
using ARWNI2S.Infrastructure;
using ARWNI2S.Infrastructure.Engine;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace ARWNI2S.Engine.Core.Dispatching
{
    internal sealed class Dispatcher : IDispatcher
    {
        private readonly FrameProcessor _frameProcessor;
        private readonly EventPool _eventPool;
        private readonly ILogger _logger;

        private readonly object _lock = new();

        private readonly Thread _workerThread;
        private readonly ManualResetEventSlim _startedEvent;
        private readonly ConcurrentQueue<IEvent> _eventQueue = new();

        private CancellationTokenSource cancelSource;

        public Thread WorkerThread => _workerThread;

        public Dispatcher(
            FrameProcessor frameProcessor,
            EventPool eventPool,

            ILogger logger)
        {
            _frameProcessor = frameProcessor;
            _eventPool = eventPool;
            _logger = logger;

            _startedEvent = new ManualResetEventSlim(false);
            _workerThread = new Thread(_ => RunWorkerThread(cancelSource.Token))
            {
                IsBackground = true,
                Name = $"{nameof(Dispatcher)}.{nameof(WorkerThread)}",
            };
        }

        private void RunWorkerThread(CancellationToken token)
        {
            try
            {
                _startedEvent.Set();

                while (!token.IsCancellationRequested)
                {
                    DispatcherLoop();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DispatcherThread Exception: {ex.Message}");
            }
        }

        private void DispatcherLoop()
        {
            lock (_lock)
            {
                while (_eventQueue.IsEmpty)
                {
                    // Esperar hasta que haya eventos disponibles
                    Monitor.Wait(_lock);
                }

                // Procesar eventos
                while (_eventQueue.TryDequeue(out var @event))
                {
                    try
                    {
                        // Procesar el evento
                        Console.WriteLine($"Processing event: {@event}");
                    }
                    catch (NI2SException niisEx)
                    {
                        HandleException(niisEx);
                    }
                    catch (Exception ex)
                    {
                        HandleUnkownException(ex);
                    }
                }
            }
        }

        public void ScheduleEvent(IEvent @event)
        {
            lock (_lock)
            {
                _eventQueue.Enqueue(@event);
                // Notificar al hilo worker que hay un nuevo evento
                Monitor.Pulse(_lock);
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            cancelSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _workerThread.Start();
            await Task.Run(() => _startedEvent.Wait(cancellationToken));
            Console.WriteLine("Dispatcher started successfully.");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                cancelSource.Cancel();
                await Task.Run(() =>
                {
                    if (!_workerThread.Join(5000))
                    {
                        Console.WriteLine("Dispatcher thread did not stop gracefully. Aborting...");
                        _workerThread.Interrupt();
                    }
                }, cancellationToken);

                Console.WriteLine("Dispatcher stopped successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"StopAsync Exception: {ex.Message}");
            }
        }

        private void HandleException(NI2SException niisEx)
        {
            Console.WriteLine($"Handled NI2SException: {niisEx.Message}");
        }

        private void HandleUnkownException(Exception ex)
        {
            Console.WriteLine($"Handled Unknown Exception: {ex.Message}");
        }
    }
}
