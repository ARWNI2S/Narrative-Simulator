using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ARWNI2S.Engine.Hosting
{
    internal class SimulationHostedService : BackgroundService
    {
        private readonly ILogger<SimulationHostedService> _logger;

        #region Ctor

        public SimulationHostedService(ILogger<SimulationHostedService> logger)
        {
            _logger = logger;
        }

        #endregion

        #region IHostedService overrides

        /// <inheritdoc />
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            /* critical */

            /* Simulation initialization before start */

            /* critical */

            return base.StartAsync(cancellationToken);

            /* non critical */

            /* After start do nothing */

            /* non critical */
        }

        /// <inheritdoc />
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            /* critical */

            /* Simulation graceful ending before stop */

            return base.StopAsync(cancellationToken);

            /* Simulation cleanup after stop */

            /* critical */
        }

        #endregion

        #region Ctor

        /// <inheritdoc />
        public override void Dispose()
        {
            /* critical */

            base.Dispose();

            /* critical */
        }

        #endregion

        #region Methods

        /// <inheritdoc />
        public override Task? ExecuteTask => base.ExecuteTask;

        /// <summary>
        /// Simulation Loop Entry Point.
        /// </summary>
        /// <param name="stoppingToken">Triggered when <see cref="IHostedService.StopAsync(CancellationToken)"/> is called.</param>
        /// <returns>A <see cref="Task"/> that represents the long running operations.</returns>
        /// <remarks>See <see href="https://docs.microsoft.com/dotnet/core/extensions/workers">Worker Services in .NET</see> for implementation guidelines.</remarks>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            /* Simulation Loop Entry Point */
            /* critical */
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                /* critical */
                await Task.Delay(1000, stoppingToken);
                /* critical */
            }
            /* critical */
        }

        #endregion

        #region object

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            /* critical */

            if (obj is null) return false;
            if (obj is not SimulationHostedService) return false;

            if (ReferenceEquals(this, obj)) return true;

            return base.GetHashCode().Equals(obj.GetHashCode());

            /* critical */
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            // TODO : Implement equallity
            /* critical */

            return base.GetHashCode();

            /* critical */
        }

        /// <inheritdoc />
        public override string ToString()
        {
            // TODO : Implement quick stats string generation.
            /* critical */

            return nameof(SimulationHostedService);

            /* critical */
        }

        #endregion
    }
}
