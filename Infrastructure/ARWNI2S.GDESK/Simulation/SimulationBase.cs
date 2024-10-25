using ARWNI2S.Engine.Infrastructure;
using ARWNI2S.Engine.Simulation.Kernel;
using ARWNI2S.Engine.Simulation.LOD;
using ARWNI2S.Engine.Simulation.Time;
using ARWNI2S.Infrastructure.Logging;
using ARWNI2S.Infrastructure.Timing;
using Microsoft.Extensions.Logging;

namespace ARWNI2S.Engine.Simulation
{
    public abstract class SimulationBase : ISimulation, ISimulable
    {
        private class Actor : ISimulable
        {
            private static int IDs = 0;

            private readonly int _id;
            private readonly Random _random = new(DateTime.UtcNow.Millisecond);
            private readonly Dispatcher _dispatcher;
            private readonly ILogger _logger;
            private SimulationBase _simulation;

            public int ID => _id;

            public Actor(Dispatcher dispatcher, ILogger logger)
            {
                _id = IDs;
                _dispatcher = dispatcher;
                _logger = logger;
                IDs++;
            }

            public void HandleEvent(Event @event)
            {
                if (@event == null) return;
                if (@event.Sender == this)
                {
                    var d = _random.NextDouble();

                    if (d <= .05)
                    {
                        var next = new Event { Time = TimeSpan.FromMilliseconds(0), Sender = this, Receiver = _simulation, Data = "KILL_ME" };
                        _dispatcher.EnqueueEvent(next);
                    }
                    if (d > .05)
                    {
                        var next = new Event { Time = TimeSpan.FromMilliseconds(@event.TimeMs + 1000), Sender = this, Receiver = this };
                        _dispatcher.EnqueueEvent(next);
                    }
                    if (d > .95 && _simulation != null)
                    {
                        var next = new Event { Time = TimeSpan.FromMilliseconds(@event.TimeMs + 1000), Sender = this, Receiver = _simulation };
                        _dispatcher.EnqueueEvent(next);
                    }

                }
                else if (@event.Sender is SimulationBase)
                {
                    if (_simulation == null)
                        _simulation = (SimulationBase)@event.Sender;
                    var next = new Event { Time = TimeSpan.FromMilliseconds(@event.TimeMs + 1000), Sender = this, Receiver = this };
                    _dispatcher.EnqueueEvent(next);
                }
            }
        }

        private List<Actor> actors = [];

        public struct Statistics
        {
            public double StartTimeMs;

            public double FrameStartTimeMs;

            public double WorkEndTimeMs;
            public double WorkElapsedTimeMs;

            public double FrameFreeTimeMs;

            public double FrameEndTimeMs;
            public double FrameElapsedTimeMs;
            internal int FramesPerSecond;
            internal int LastFrame;
        }

        private Statistics stats;
        private readonly HiResTimer _timer;

        private readonly Dispatcher _dispatcher;
        private readonly FrameTaskScheduler _frameTaskScheduler;

        private readonly Thread _workerThread;

        private readonly ISimulationClock _clock;
        private readonly ILogger _logger;

        protected CancellationTokenSource mainCancelSource;
        protected CancellationTokenSource innerCancelSource;
        private CancellationToken dispatcherCancelToken;
        private bool _reached;

        public ISimulationClock Clock => _clock;
        public ILogger Logger => _logger;

        internal SimulationBase(Dispatcher dispatcher, FrameTaskScheduler frameTaskScheduler, ISimulationClock clock, ILogger logger)
        {
            _dispatcher = dispatcher;
            _frameTaskScheduler = frameTaskScheduler;

            _clock = clock;
            _logger = logger;

            _timer = new HiResTimer();

            // Create the dedicated thread
            _workerThread = new Thread((_) => ThreadLoop(mainCancelSource.Token))
            {
                Name = GetType().Name + "_MainThread",
                Priority = ThreadPriority.AboveNormal,
                IsBackground = true // Mark thread as background so it won't block app shutdown
            };

        }

        // Method to start the loop on a dedicated thread
        protected virtual void StartSimulation(CancellationToken? cancellationToken = null)
        {
            mainCancelSource = cancellationToken.HasValue ? new CancellationTokenSource() :
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken.Value);
            innerCancelSource = new CancellationTokenSource();
            dispatcherCancelToken = innerCancelSource.Token;

            _dispatcher.EnqueueEvent(new Event { Time = TimeSpan.FromMilliseconds(0), Sender = this, Receiver = this });

            // Start the dedicated thread
            _workerThread.Start();
            _logger.LogInformation("Worker dedicated thread has started.");
        }

        // Method to stop the loop gracefully
        protected virtual void StopSimulation()
        {
            if (_workerThread == null)
                return;

            // Signal cancellation
            mainCancelSource.Cancel();
            _logger.LogInformation("Stopping working thread...");

            // Wait for the thread to complete
            _workerThread.Join();

            _logger.LogInformation("Worker thread has exited.");
        }

        // The actual loop logic, running on the dedicated thread
        private void ThreadLoop(CancellationToken cancellationToken)
        {
            _timer.Start();
            stats.FrameStartTimeMs = stats.StartTimeMs = _timer.GetTime();
            int frameCount = 0;
            double timeCount = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {

                    _dispatcher.ProcessEvents(dispatcherCancelToken);

                    _frameTaskScheduler.RunFrameTasks();

                    stats.WorkEndTimeMs = _timer.GetTime();
                    stats.WorkElapsedTimeMs = stats.WorkEndTimeMs - stats.FrameStartTimeMs;

                    stats.FrameFreeTimeMs = stats.FrameStartTimeMs + SimulationLOD.LOD_0.Resolution - stats.WorkEndTimeMs;

                    if (stats.FrameFreeTimeMs > 0)
                    {
                        _frameTaskScheduler.PrepareFrameTasks();

                        stats.WorkEndTimeMs = _timer.GetTime();
                        stats.WorkElapsedTimeMs = stats.WorkEndTimeMs - stats.FrameStartTimeMs;

                        stats.FrameFreeTimeMs = stats.FrameStartTimeMs + SimulationLOD.LOD_0.Resolution - stats.WorkEndTimeMs;

                        //if (stats.FrameFreeTimeMs > 0)
                        //    Thread.Sleep(TimeSpan.FromMilliseconds(stats.FrameFreeTimeMs)); // 1-second delay between iterations
                    }
                    stats.FrameEndTimeMs = _timer.GetTime();
                    stats.FrameElapsedTimeMs = stats.FrameEndTimeMs - stats.FrameStartTimeMs;

                    stats.FrameStartTimeMs = stats.FrameEndTimeMs;

                    timeCount += stats.FrameElapsedTimeMs;
                    frameCount++;
                    stats.LastFrame = frameCount;
                    if (timeCount >= 1000)
                    {
                        stats.FramesPerSecond = frameCount;
                        frameCount = 0;
                        timeCount = timeCount - 1000;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("", ex);
                }
            }

        }

        void ISimulation.StartSimulation(CancellationToken? cancellationToken) => StartSimulation(cancellationToken);

        void ISimulation.StopSimulation() => StopSimulation();

        void ISimulable.HandleEvent(Event @event)
        {

            if (@event == null) return;
            if (@event.Sender == this)
            {
                _logger.LogInformation("EVENT[{Id}]\t{time} - SIMULATION STATS\n\tActors:\t{actors}\n\tfps:\t{fps}\n\tframe:\t{frame}",
                    @event.Id, @event.Time, actors.Count(), stats.FramesPerSecond, stats.LastFrame);
                _dispatcher.EnqueueEvent(new Event { Time = TimeSpan.FromMilliseconds(@event.TimeMs + 1000), Sender = this, Receiver = this });

                if (!_reached)
                {
                    if (actors.Count < 1000)
                    {
                        lock (actors)
                        {
                            for (int i = 0; i < 100; i++)
                            {
                                var actor = new Actor(_dispatcher, _logger);
                                actors.Add(actor);
                                _dispatcher.EnqueueEvent(new Event { Time = TimeSpan.FromMilliseconds(@event.TimeMs + 1000 + (i * 10)), Sender = this, Receiver = actor });
                            }
                        }
                    }
                    if (actors.Count >= 10000)
                        _reached = true;
                }

            }
            else if (@event.Sender is Actor actor)
            {
                if (@event.Data == "KILL_ME")
                {
                    lock (actors)
                    {
                        actors.Remove(actor);
                        actor = null;
                    }
                }
                else
                {
                    var newActor = new Actor(_dispatcher, _logger);
                    lock (actors)
                    {
                        actors.Add(newActor);
                    }
                    _dispatcher.EnqueueEvent(new Event { Time = TimeSpan.FromMilliseconds(0), Sender = this, Receiver = newActor });
                }
            }
        }
    }
}
