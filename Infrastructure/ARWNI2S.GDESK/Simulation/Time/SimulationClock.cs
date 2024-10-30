

namespace ARWNI2S.Engine.Simulation.Time
{
    public class SimulationClock : ISimulationClock
    {
        private ulong _time;
        private double _pendingTime;
        private double _resolution;
        private bool _isStopped;

        public ulong GetTimeUnits() => _time;

        public void Advance(double deltaTimeMs)
        {
            if (!_isStopped)
            {
                double delta = (deltaTimeMs * _resolution);
                ulong time = (ulong)delta;
                _pendingTime += delta - time;
                if (_pendingTime > 1)
                {
                    _pendingTime--;
                    time++;
                }
                _time += time;
            }
        }

        public double GetResolution() => _resolution;

        public void Pause() { _isStopped = true; }

        public void Start() { _isStopped = false; }

        public Task<TimeSpan> GetTimeAsync() => Task.FromResult(TimeSpan.FromMilliseconds(GetTimeUnits()));
    }
}
