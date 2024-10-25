using ARWNI2S.Infrastructure.Timing;

namespace ARWNI2S.Engine.Simulation.Time
{
    public class SimulationClock : ISimulationClock
    {
        private readonly HiResTimer _hiResTimer;
        private double _startTime;
        private double _stopTime;
        //private double _lastElapsedTime;

        public SimulationClock()
        {
            _hiResTimer = new HiResTimer();
            Reset(TimeSpan.FromMilliseconds(0));
        }

        public virtual async Task<TimeSpan> GetCurrentTimeAsync()
        {
            return await Task.FromResult(TimeSpan.FromMilliseconds(_hiResTimer.GetTime()));
        }

        public virtual void Pause()
        {
            if (!_hiResTimer.IsStopped)
                _hiResTimer.Stop();

            _stopTime = _hiResTimer.GetTime();
        }

        public virtual void Reset(TimeSpan startTime)
        {
            _startTime = startTime.TotalMilliseconds;
            _hiResTimer.Reset();
        }

        public virtual void Start()
        {
            if (_hiResTimer.IsStopped)
                _hiResTimer.Start();

            _startTime = _hiResTimer.GetTime();
        }

        public virtual void Synchronize(TimeSpan time)
        {
            if (!_hiResTimer.IsStopped)
                _hiResTimer.Stop();

        }
    }
}
