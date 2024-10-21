using ARWNI2S.Engine.Simulation;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace ARWNI2S.Engine.Hosting
{
    internal abstract class SimulationHostedServiceBase : ISimulationHostedService
    {
        private Task? _executeTask;
        private CancellationTokenSource? _stoppingCts;

        private readonly ILogger _logger;

        public virtual SimulationBase Simulation { get; protected set; }

        #region Ctor

        protected SimulationHostedServiceBase(SimulationBase simulation, ILogger logger)
        {
            Simulation = simulation;
            _logger = logger;
        }

        #endregion

        #region IHostedService

        /// <summary>
        /// Triggered when the application host is ready to start the service.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous Start operation.</returns>
        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                /* critical */
                Simulation.PreInitialize(cancellationToken);

                /* Simulation initialization before start */
                Simulation.Initialize(cancellationToken);

                /* critical */
                Simulation.PostInitialize(cancellationToken);
            }
            catch (Exception ex)
            {
                if (!HandleInitializationException(ex))
                    throw;
            }

            // Create linked token to allow cancelling executing task from provided token
            _stoppingCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            // Store the task we're executing
            _executeTask = ExecuteAsync(_stoppingCts.Token);

            // If the task is completed then return it, this will bubble cancellation and failure to the caller
            if (_executeTask.IsCompleted)
            {
                return _executeTask;
            }

            // Otherwise it's running
            return Task.CompletedTask;
        }

        private bool HandleInitializationException(Exception ex)
        {
            // TODO: ERROR CONTROL
            // By now we consider always critical breaks.
            return false;
        }

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous Stop operation.</returns>
        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            // Stop called without start
            if (_executeTask == null)
            {
                return;
            }

            /* critical */

            /* Simulation graceful ending before stop */

            try
            {
                // Signal cancellation to the executing method
                _stoppingCts!.Cancel();
            }
            finally
            {
                // Wait until the task completes or the stop token triggers
                var tcs = new TaskCompletionSource<object>();
                using CancellationTokenRegistration registration = cancellationToken.Register(s => ((TaskCompletionSource<object>)s!).SetCanceled(), tcs);
                // Do not await the _executeTask because cancelling it will throw an OperationCanceledException which we are explicitly ignoring
                await Task.WhenAny(_executeTask, tcs.Task).ConfigureAwait(false);

                /* Simulation cleanup after stop */

                /* critical */
            }
        }

        #endregion

        /// <summary>
        /// Simulation Loop Entry Point.
        /// </summary>
        /// <param name="stoppingToken">Triggered when <see cref="IHostedService.StopAsync(CancellationToken)"/> is called.</param>
        /// <returns>A <see cref="Task"/> that represents the long running operations.</returns>
        /// <remarks>See <see href="https://docs.microsoft.com/dotnet/core/extensions/workers">Worker Services in .NET</see> for implementation guidelines.</remarks>
        protected virtual async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            /* Simulation Loop Entry Point */
            if (Simulation.Start(stoppingToken).Success)
            {
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

        }

        /// <inheritdoc />
        public virtual void Dispose()
        {
            /* critical */

            _stoppingCts?.Cancel();

            /* critical */
        }

        /// <inheritdoc />
        public override string ToString()
        {
            // TODO : Implement quick stats string generation.
            /* critical */

            return $"[{GetType()}] : TODO: Implement quick stats string generation.";

            /* critical */
        }
    }

    internal abstract class SimulationHostedServiceBase<TSimulation> : ISimulationHostedService<TSimulation>, ISimulationHostedService where TSimulation : SimulationBase
    {
        public TSimulation Simulation => throw new NotImplementedException();

        Simulation.SimulationBase ISimulationHostedService.Simulation => throw new NotImplementedException();

        public Task StartAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }






        //    private Task? _executeTask;
        //    private CancellationTokenSource? _stoppingCts;

        //    private readonly TSimulation Simulation;
        //    private readonly ILogger<SimulationHostedService<TSimulation>> _logger;

        //    #region Ctor

        //    public SimulationHostedService(TSimulation simulation,
        //        ILogger<SimulationHostedService<TSimulation>> logger)
        //    {
        //        Simulation = simulation;
        //        _logger = logger;
        //    }

        //    #endregion

        //    #region IHostedService

        //    /// <summary>
        //    /// Triggered when the application host is ready to start the service.
        //    /// </summary>
        //    /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        //    /// <returns>A <see cref="Task"/> that represents the asynchronous Start operation.</returns>
        //    public Task StartAsync(CancellationToken cancellationToken)
        //    {
        //        /* critical */

        //        /* Simulation initialization before start */

        //        /* critical */

        //        // Create linked token to allow cancelling executing task from provided token
        //        _stoppingCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        //        // Store the task we're executing
        //        _executeTask = ExecuteAsync(_stoppingCts.Token);

        //        // If the task is completed then return it, this will bubble cancellation and failure to the caller
        //        if (_executeTask.IsCompleted)
        //        {
        //            return _executeTask;
        //        }

        //        // Otherwise it's running
        //        return Task.CompletedTask;
        //    }

        //    /// <summary>
        //    /// Triggered when the application host is performing a graceful shutdown.
        //    /// </summary>
        //    /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        //    /// <returns>A <see cref="Task"/> that represents the asynchronous Stop operation.</returns>
        //    public async Task StopAsync(CancellationToken cancellationToken)
        //    {
        //        // Stop called without start
        //        if (_executeTask == null)
        //        {
        //            return;
        //        }

        //        /* critical */

        //        /* Simulation graceful ending before stop */

        //        try
        //        {
        //            // Signal cancellation to the executing method
        //            _stoppingCts!.Cancel();
        //        }
        //        finally
        //        {
        //            // Wait until the task completes or the stop token triggers
        //            var tcs = new TaskCompletionSource<object>();
        //            using CancellationTokenRegistration registration = cancellationToken.Register(s => ((TaskCompletionSource<object>)s!).SetCanceled(), tcs);
        //            // Do not await the _executeTask because cancelling it will throw an OperationCanceledException which we are explicitly ignoring
        //            await Task.WhenAny(_executeTask, tcs.Task).ConfigureAwait(false);

        //            /* Simulation cleanup after stop */

        //            /* critical */
        //        }
        //    }

        //    #endregion

        //    /// <summary>
        //    /// Simulation Loop Entry Point.
        //    /// </summary>
        //    /// <param name="stoppingToken">Triggered when <see cref="IHostedService.StopAsync(CancellationToken)"/> is called.</param>
        //    /// <returns>A <see cref="Task"/> that represents the long running operations.</returns>
        //    /// <remarks>See <see href="https://docs.microsoft.com/dotnet/core/extensions/workers">Worker Services in .NET</see> for implementation guidelines.</remarks>
        //    protected async Task ExecuteAsync(CancellationToken stoppingToken)
        //    {
        //        /* Simulation Loop Entry Point */
        //        if (Simulation.Start(stoppingToken).Success)
        //        {
        //            /* critical */

        //            while (!stoppingToken.IsCancellationRequested)
        //            {
        //                if (_logger.IsEnabled(LogLevel.Information))
        //                {
        //                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        //                }

        //                /* critical */

        //                await Task.Delay(1000, stoppingToken);

        //                /* critical */
        //            }

        //            /* critical */
        //        }

        //    }

        //    /// <inheritdoc />
        //    public virtual void Dispose()
        //    {
        //        /* critical */

        //        _stoppingCts?.Cancel();

        //        /* critical */
        //    }

        //    /// <inheritdoc />
        //    public override string ToString()
        //    {
        //        // TODO : Implement quick stats string generation.
        //        /* critical */

        //        return nameof(SimulationHostedService);

        //        /* critical */
        //    }

        //    async Task ISimulationHostedService<DefaultSimulation>.ExecuteAsync(CancellationToken stoppingToken) => await ExecuteAsync(stoppingToken);
    }
}
