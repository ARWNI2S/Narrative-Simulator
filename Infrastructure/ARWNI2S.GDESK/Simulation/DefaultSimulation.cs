


using ARWNI2S.Infrastructure.Engine;
using ARWNI2S.Infrastructure.Memory;
using Microsoft.Extensions.Logging;

namespace ARWNI2S.Engine.Simulation
{
    internal class DefaultSimulation : ISimulation
    {
        private readonly ILogger<ISimulation> _logger;
        private readonly IPool<Thread> _threadPool;


        private readonly Thread _mainThread;

        private CancellationToken _stoppingToken;

        public Thread? MainThread => _mainThread;

        public DefaultSimulation(ILogger<ISimulation> logger,
            IPool<Thread> threadPool,
            IEngineContextAccessor nI2SContexAccessor)
        {
            _logger = logger;

            _mainThread = new Thread(new ThreadStart(Simulation_Loop));
            _threadPool = threadPool;
        }

        protected void Simulation_Loop()
        {
            while (!_stoppingToken.IsCancellationRequested)
            {
                /* critical */

                while (!_stoppingToken.IsCancellationRequested)
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogInformation("Simulation_Loop running at: {time}", DateTimeOffset.Now);
                    }

                    /* critical */


                    /* critical */
                }

                /* critical */
            }


        }

        public SimulationStartResult Start(CancellationToken stoppingToken)
        {
            _stoppingToken = stoppingToken;

            try
            {
                _mainThread.Start();
            }
            catch (OutOfMemoryException oOmEx)
            {
                _logger.LogError(oOmEx.Message);
            }
            catch (ThreadStateException tSeX)
            {
                _logger.LogError(tSeX.Message);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {

            }

            return new SimulationStartResult()
            {
                Success = _mainThread.IsAlive
            };
        }
    }
}