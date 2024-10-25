using ARWNI2S.Engine.Infrastructure;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ARWNI2S.Engine.Hosting
{
    internal class GDESKHostedService : IHostedService, IDisposable
    {
        private readonly FrameProcessor _frameProcessor;
        private readonly ILogger<GDESKHostedService> _logger;

        public GDESKHostedService(FrameProcessor frameProcessor, ILogger<GDESKHostedService> logger)
        {
            _frameProcessor = frameProcessor;
            _logger = logger;
        }

        private Task _executeTask;
        private CancellationTokenSource _stoppingCts;

        /// <summary>
        /// Gets the Task that executes the background operation.
        /// </summary>
        /// <remarks>
        /// Will return <see langword="null"/> if the background operation hasn't started.
        /// </remarks>
        public virtual Task ExecuteTask => _executeTask;

        //protected virtual async Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    while (!stoppingToken.IsCancellationRequested)
        //    {
        //        if (_logger.IsEnabled(LogLevel.Information))
        //        {
        //            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        //        }
        //        await Task.Delay(1000, stoppingToken);
        //    }
        //}

        /// <summary>
        /// Triggered when the application host is ready to start the service.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous Start operation.</returns>
        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            // Create linked token to allow cancelling executing task from provided token
            _stoppingCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            _logger.Log(LogLevel.Information, $"{nameof(GDESKHostedService)} starting...");

            // Store the task we're executing
            _executeTask = _frameProcessor.RunAsync(_stoppingCts.Token);
            //_executeTask = _frameTaskScheduler.RunAsync(_stoppingCts.Token);

            // If the task is completed then return it, this will bubble cancellation and failure to the caller
            if (_executeTask.IsCompleted)
            {
                return _executeTask;
            }

            // Otherwise it's running
            return Task.CompletedTask;
        }

        //public virtual Task StartAsync(CancellationToken cancellationToken)
        //{
        //    // Create linked token to allow cancelling executing task from provided token
        //    _stoppingCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        //    _logger.Log(LogLevel.Information, $"{nameof(GDESKHostedService)} starting...");

        //    // Ejecutar la tarea usando el FrameTaskScheduler y el ServiceProvider de este contexto
        //    return Task.Factory.StartNew(() =>
        //    {
        //        using (var scope = _serviceProvider.CreateScope())
        //        {
        //            // Store the task we're executing
        //            _executeTask = ExecuteAsync(_stoppingCts.Token);

        //            // If the task is completed then return it, this will bubble cancellation and failure to the caller
        //            if (_executeTask.IsCompleted)
        //            {
        //                return _executeTask;
        //            }

        //            // Otherwise it's running
        //            return Task.CompletedTask;
        //        }
        //    }, CancellationToken.None, TaskCreationOptions.None, _taskScheduler).Unwrap();
        //}

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous Stop operation.</returns>
        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.Log(LogLevel.Information, $"{nameof(GDESKHostedService)} stop!");

            // Stop called without start
            if (_executeTask == null)
            {
                return;
            }

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
            }

        }

        /// <inheritdoc />
        public virtual void Dispose()
        {
            _stoppingCts?.Cancel();
        }
    }
}