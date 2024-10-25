using ARWNI2S.Engine.Simulation;
using ARWNI2S.Infrastructure.Timing;
using Microsoft.Extensions.Logging;

namespace ARWNI2S.Engine.Infrastructure
{
    internal class FrameProcessor
    {
        private readonly Thread _workerThread;

        private readonly HiResTimer _timer;
        private readonly ILogger<FrameProcessor> _logger;
        private readonly FrameTaskScheduler _frameTaskScheduler;
        private ISimulation parentSimulation;
        private CancellationTokenSource cancelSource;

        //private double _lastTime;
        private double lastElapsedTime;
        private int frameCount;
        private int currentFps;
        //private int _desiredFrameRate = 15;
        //private int _minFrameRate = 15;
        //private int _maxFrameRate = 15;

        private double _frameStartTime;

        public FrameProcessor(FrameTaskScheduler frameTaskScheduler, ILogger<FrameProcessor> logger) : this(new HiResTimer(), logger)
        {
            _frameTaskScheduler = frameTaskScheduler;

            // Create the dedicated thread
            _workerThread = new Thread((obj) => ThreadLoop(cancelSource.Token))
            {
                IsBackground = true // Mark thread as background so it won't block app shutdown
            };

        }

        private FrameProcessor(HiResTimer timer, ILogger<FrameProcessor> logger)
        {
            _timer = timer;
            _logger = logger;
        }

        private void ThreadLoop(CancellationToken stoppingToken)
        {
            _timer.Start();
            _frameStartTime = _timer.GetTime();
            var timeCount = _frameStartTime;

            while (!stoppingToken.IsCancellationRequested)
            {

                _frameTaskScheduler.RunFrameTasks();

                var freeTime = _frameStartTime + parentSimulation.CurrentLOD.Resolution - _timer.GetTime();
                if (freeTime > 0)
                    Thread.Sleep(TimeSpan.FromMilliseconds(freeTime <= 16 ? 0 : freeTime - 16)); // 1-second delay between iterations

                lastElapsedTime = _timer.GetTime() - _frameStartTime;

                _frameStartTime += lastElapsedTime;
                frameCount++;

                timeCount += lastElapsedTime;
                if (timeCount >= 1000)
                {
                    currentFps = frameCount;
                    _logger.LogInformation("FPS: {fps} - Avg.Ms: {avgMs}   --  tc: {tc}", currentFps, timeCount / currentFps, timeCount);
                    frameCount = 0;
                    timeCount = timeCount - 1000;
                }

            }
        }

        internal void Start(ISimulation simulation, CancellationToken? cancellationToken)
        {
            if (simulation == null)
                return;

            parentSimulation = simulation;

            cancelSource = cancellationToken.HasValue ? CancellationTokenSource.CreateLinkedTokenSource(cancellationToken.Value) : new CancellationTokenSource();

            _workerThread.Start(cancelSource.Token);
        }
    }
}
