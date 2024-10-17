using System.Diagnostics;

namespace ARWNI2S.SceneNode.Framework.GameEngine
{
    internal class GameServerHostProcess(string executablePath, string args = "-log -server", int monitorResolution = 3000)
    {
        public struct ProcessStats
        {
            public double CpuUsage { get; set; }
            public long MemoryUsage { get; set; }
            public TimeSpan TotalProcessorTime { get; set; }
        }

        private Process _gameServerProcess;
        private Timer _monitorTimer; // TODO: USE INFRASTRUCTURED?

        public event Action OnStart;
        public event Action OnExit;
        public event Action<ProcessStats> OnStatsUpdate;

        public void Start()
        {
            _gameServerProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = executablePath,
                    Arguments = args,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                },
                EnableRaisingEvents = true
            };

            _gameServerProcess.Exited += Process_Exited;

            _gameServerProcess.Start();

            OnStart?.Invoke();

            StartMonitoring();
        }

        public void Stop()
        {
            if (_gameServerProcess != null && !_gameServerProcess.HasExited)
            {
                _gameServerProcess.Kill();
            }
            StopMonitoring();
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            StopMonitoring();
            OnExit?.Invoke();
        }

        private void StartMonitoring()
        {
            _monitorTimer ??= new Timer(MonitorProcess, null, Timeout.Infinite, monitorResolution);

            _monitorTimer.Change(0, monitorResolution);
        }

        private void StopMonitoring()
        {
            _monitorTimer?.Change(Timeout.Infinite, monitorResolution);
        }

        private void MonitorProcess(object state)
        {
            if (_gameServerProcess != null && !_gameServerProcess.HasExited)
            {
                var stats = new ProcessStats
                {
                    CpuUsage = GetCpuUsage(),
                    MemoryUsage = _gameServerProcess.WorkingSet64,
                    TotalProcessorTime = _gameServerProcess.TotalProcessorTime
                };

                OnStatsUpdate?.Invoke(stats);
            }
        }

        private double GetCpuUsage()
        {
            try
            {
                return (_gameServerProcess.TotalProcessorTime.TotalMilliseconds / Environment.ProcessorCount) / (DateTime.Now - _gameServerProcess.StartTime).TotalMilliseconds * 100;
            }
            catch
            {
                return 0;
            }
        }

    }
}
