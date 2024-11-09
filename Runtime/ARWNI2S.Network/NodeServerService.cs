using ARWNI2S.Engine.Network.Configuration.Options;
using ARWNI2S.Engine.Network.Connection;
using ARWNI2S.Engine.Network.Middleware;
using ARWNI2S.Engine.Network.Proxy;
using ARWNI2S.Engine.Network.Session;
using ARWNI2S.Infrastructure;
using ARWNI2S.Infrastructure.Extensions;
using ARWNI2S.Infrastructure.Lifecycle;
using ARWNI2S.Infrastructure.Logging;
using ARWNI2S.Infrastructure.Network.Connection;
using ARWNI2S.Infrastructure.Network.Protocol;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ARWNI2S.Engine.Network
{
    public class NodeServerService<TReceivePackageInfo> : INodeServerHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public IServiceProvider ServiceProvider
        {
            get { return _serviceProvider; }
        }

        public ServerOptions Options { get; }
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;
        private readonly NodeLifecycle _nodeServerLifecycle;

        internal protected ILogger Logger
        {
            get { return _logger; }
        }

        ILogger ILoggerAccessor.Logger
        {
            get { return _logger; }
        }

        private IPipelineFilterFactory<TReceivePackageInfo> _pipelineFilterFactory;
        private IConnectionListenerFactory _connectionListenerFactory;
        private List<IConnectionListener> _connectionListeners;
        private IPackageHandlingScheduler<TReceivePackageInfo> _packageHandlingScheduler;
        private IPackageHandlingContextAccessor<TReceivePackageInfo> _packageHandlingContextAccessor;

        public string Name { get; }

        private int _sessionCount;

        public int SessionCount => _sessionCount;

        private ISessionFactory _sessionFactory;

        private IMiddleware[] _middlewares;

        protected IMiddleware[] Middlewares
        {
            get { return _middlewares; }
        }

        private ServerState _state = ServerState.None;

        public ServerState State
        {
            get { return _state; }
        }

        public object DataContext { get; set; }

        private SessionHandlers _sessionHandlers;

        public NodeServerService(IServiceProvider serviceProvider, IOptions<ServerOptions> serverOptions)
        {
            ArgumentNullException.ThrowIfNull(serviceProvider);

            ArgumentNullException.ThrowIfNull(serverOptions);

            Name = serverOptions.Value.Name;
            Options = serverOptions.Value;
            _serviceProvider = serviceProvider;
            _pipelineFilterFactory = GetPipelineFilterFactory();
            _loggerFactory = _serviceProvider.GetService<ILoggerFactory>();
            _logger = _loggerFactory.CreateLogger("NodeServerService");
            _connectionListenerFactory = _serviceProvider.GetService<IConnectionListenerFactory>();
            _sessionHandlers = _serviceProvider.GetService<SessionHandlers>();
            _sessionFactory = _serviceProvider.GetService<ISessionFactory>();
            _packageHandlingContextAccessor = _serviceProvider.GetService<IPackageHandlingContextAccessor<TReceivePackageInfo>>();

            InitializeMiddlewares();

            var packageHandler = _serviceProvider.GetService<IPackageHandler<TReceivePackageInfo>>()
                ?? _middlewares.OfType<IPackageHandler<TReceivePackageInfo>>().FirstOrDefault();

            if (packageHandler == null)
            {
                Logger.LogWarning("The PackageHandler cannot be found.");
            }
            else
            {
                var errorHandler = _serviceProvider.GetService<Func<INodeSession, PackageHandlingException<TReceivePackageInfo>, ValueTask<bool>>>()
                    ?? OnSessionErrorAsync;

                _packageHandlingScheduler = _serviceProvider.GetService<IPackageHandlingScheduler<TReceivePackageInfo>>()
                    ?? new SerialPackageHandlingScheduler<TReceivePackageInfo>();
                _packageHandlingScheduler.Initialize(packageHandler, errorHandler);
            }

            _nodeServerLifecycle = new NodeLifecycle(_logger);
            // register all lifecycle participants
            IEnumerable<ILifecycleParticipant<INodeLifecycle>> lifecycleParticipants = _serviceProvider.GetServices<ILifecycleParticipant<INodeLifecycle>>();
            foreach (var participant in lifecycleParticipants)
            {
                participant?.Participate(_nodeServerLifecycle);
            }

        }

        protected virtual IPipelineFilterFactory<TReceivePackageInfo> GetPipelineFilterFactory()
        {
            var filterFactory = _serviceProvider.GetRequiredService<IPipelineFilterFactory<TReceivePackageInfo>>();

            if (Options.EnableProxyProtocol)
                filterFactory = new ProxyPipelineFilterFactory<TReceivePackageInfo>(filterFactory);

            return filterFactory;
        }

        private bool AddConnectionListener(ListenOptions listenOptions, ServerOptions serverOptions)
        {
            var listener = _connectionListenerFactory.CreateConnectionListener(listenOptions, serverOptions, _loggerFactory);
            listener.NewConnectionAccept += OnNewConnectionAccept;

            if (!listener.Start())
            {
                _logger.LogError($"Failed to listen {listener}.");
                return false;
            }

            _logger.LogInformation($"The listener [{listener}] has been started.");
            _connectionListeners.Add(listener);
            return true;
        }

        private Task<bool> StartListenAsync(CancellationToken cancellationToken)
        {
            _connectionListeners = [];

            var serverOptions = Options;

            if (serverOptions.Listeners != null && serverOptions.Listeners.Any())
            {
                foreach (var l in serverOptions.Listeners)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    if (!AddConnectionListener(l, serverOptions))
                    {
                        continue;
                    }
                }
            }
            else
            {
                _logger.LogWarning("No listener was defined, so this server only can accept connections from the ActiveConnect.");

                if (!AddConnectionListener(null, serverOptions))
                {
                    _logger.LogError($"Failed to add the channel creator.");
                    return Task.FromResult(false);
                }
            }

            return Task.FromResult(_connectionListeners.Any());
        }

        protected virtual ValueTask OnNewConnectionAccept(ListenOptions listenOptions, IConnection connection)
        {
            return AcceptNewConnection(connection);
        }

        private ValueTask AcceptNewConnection(IConnection connection)
        {
            var session = _sessionFactory.Create() as NodeSession;
            return HandleSession(session, connection);
        }

        async Task IConnectionRegister.RegisterConnection(object connectionSource)
        {
            var connectionListener = _connectionListeners.FirstOrDefault();
            using var cts = CancellationTokenSourcePool.Shared.Rent(connectionListener.Options.ConnectionAcceptTimeOut);
            var connection = await connectionListener.ConnectionFactory.CreateConnection(connectionSource, cts.Token);
            await AcceptNewConnection(connection);
        }

        protected virtual object CreatePipelineContext(INodeSession session)
        {
            return session;
        }

        #region Middlewares

        private void InitializeMiddlewares()
        {
            _middlewares = _serviceProvider.GetServices<IMiddleware>()
                .OrderBy(m => m.Order)
                .ToArray();
        }

        private void ShutdownMiddlewares()
        {
            foreach (var m in _middlewares)
            {
                try
                {
                    m.Shutdown(this);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"The exception was thrown from the middleware {m.GetType().Name} when it is being shutdown.");
                }
            }
        }

        private async ValueTask<bool> RegisterSessionInMiddlewares(INodeSession session)
        {
            var middlewares = _middlewares;

            if (middlewares != null && middlewares.Length > 0)
            {
                for (var i = 0; i < middlewares.Length; i++)
                {
                    var middleware = middlewares[i];

                    if (!await middleware.RegisterSession(session))
                    {
                        _logger.LogWarning($"A session from {session.RemoteEndPoint} was rejected by the middleware {middleware.GetType().Name}.");
                        return false;
                    }
                }
            }

            return true;
        }

        private async ValueTask UnRegisterSessionFromMiddlewares(INodeSession session)
        {
            var middlewares = _middlewares;

            if (middlewares != null && middlewares.Length > 0)
            {
                for (var i = 0; i < middlewares.Length; i++)
                {
                    var middleware = middlewares[i];

                    try
                    {
                        if (!await middleware.UnRegisterSession(session))
                        {
                            _logger.LogWarning($"The session from {session.RemoteEndPoint} was failed to be unregistered from the middleware {middleware.GetType().Name}.");
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, $"An unhandled exception occured when the session from {session.RemoteEndPoint} was being unregistered from the middleware {nameof(RegisterSessionInMiddlewares)}.");
                    }
                }
            }
        }

        #endregion

        private async ValueTask<bool> InitializeSession(INodeSession session, IConnection connection)
        {
            session.Initialize(this, connection);

            var middlewares = _middlewares;

            try
            {
                if (!await RegisterSessionInMiddlewares(session))
                    return false;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"An unhandled exception occured in {nameof(RegisterSessionInMiddlewares)}.");
                return false;
            }

            connection.Closed += (s, e) => OnChannelClosed(session, e);
            return true;
        }


        protected virtual ValueTask OnSessionConnectedAsync(INodeSession session)
        {
            var connectedHandler = _sessionHandlers?.Connected;

            if (connectedHandler != null)
                return connectedHandler.Invoke(session);

            return new ValueTask();
        }

        private void OnChannelClosed(INodeSession session, CloseEventArgs e)
        {
            FireSessionClosedEvent(session as NodeSession, e.Reason).DoNotAwait();
        }

        protected virtual ValueTask OnSessionClosedAsync(INodeSession session, CloseEventArgs e)
        {
            var closedHandler = _sessionHandlers?.Closed;

            if (closedHandler != null)
                return closedHandler.Invoke(session, e);

            return ValueTask.CompletedTask;
        }

        protected virtual async ValueTask FireSessionConnectedEvent(NodeSession session)
        {
            if (session is IHandshakeRequiredSession handshakeSession)
            {
                if (!handshakeSession.Handshaked)
                    return;
            }

            _logger.LogInformation($"A new session connected: {session.SessionID}");

            try
            {
                Interlocked.Increment(ref _sessionCount);
                await session.FireSessionConnectedAsync();
                await OnSessionConnectedAsync(session);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "There is one exception thrown from the event handler of SessionConnected.");
            }
        }

        protected virtual async ValueTask FireSessionClosedEvent(NodeSession session, CloseReason reason)
        {
            if (session is IHandshakeRequiredSession handshakeSession)
            {
                if (!handshakeSession.Handshaked)
                    return;
            }

            await UnRegisterSessionFromMiddlewares(session);

            _logger.LogInformation($"The session disconnected: {session.SessionID} ({reason})");

            try
            {
                Interlocked.Decrement(ref _sessionCount);

                var closeEventArgs = new CloseEventArgs(reason);
                await session.FireSessionClosedAsync(closeEventArgs);
                await OnSessionClosedAsync(session, closeEventArgs);
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "There is one exception thrown from the event of OnSessionClosed.");
            }
        }

        ValueTask ISessionEventHost.HandleSessionConnectedEvent(INodeSession session)
        {
            return FireSessionConnectedEvent((NodeSession)session);
        }

        ValueTask ISessionEventHost.HandleSessionClosedEvent(INodeSession session, CloseReason reason)
        {
            return FireSessionClosedEvent((NodeSession)session, reason);
        }

        private async ValueTask HandleSession(NodeSession session, IConnection connection)
        {
            if (!await InitializeSession(session, connection))
                return;

            try
            {
                var pipelineFilter = _pipelineFilterFactory.Create(connection);
                pipelineFilter.Context = CreatePipelineContext(session);

                var packageStream = connection.RunAsync<TReceivePackageInfo>(pipelineFilter);

                await FireSessionConnectedEvent(session);

                var packageHandlingScheduler = _packageHandlingScheduler;

                using var cancellationTokenSource = GetPackageHandlingCancellationTokenSource(connection.ConnectionToken);

                await foreach (var p in packageStream)
                {
                    if (_packageHandlingContextAccessor != null)
                    {
                        _packageHandlingContextAccessor.PackageHandlingContext = new PackageHandlingContext<INodeSession, TReceivePackageInfo>(session, p);
                    }

                    await packageHandlingScheduler.HandlePackage(session, p, cancellationTokenSource.Token);

                    cancellationTokenSource.TryReset();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to handle the session {session.SessionID}.");
            }
        }

        protected virtual CancellationTokenSource GetPackageHandlingCancellationTokenSource(CancellationToken cancellationToken)
        {
            var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(Options.PackageHandlingTimeOut));
            return cancellationTokenSource;
        }

        protected virtual ValueTask<bool> OnSessionErrorAsync(INodeSession session, PackageHandlingException<TReceivePackageInfo> exception)
        {
            _logger.LogError(exception, $"Session[{session.SessionID}]: session exception.");
            return new ValueTask<bool>(true);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var state = _state;

            if (state != ServerState.None && state != ServerState.Stopped)
            {
                throw new InvalidOperationException($"The server cannot be started right now, because its state is {state}.");
            }

            _state = ServerState.Starting;

            foreach (var m in _middlewares)
            {
                m.Start(this);
            }

            if (!await StartListenAsync(cancellationToken))
            {
                _state = ServerState.Failed;
                _logger.LogError("Failed to start any listener.");
                return;
            }

            _state = ServerState.Started;

            try
            {
                await _nodeServerLifecycle.OnStart(cancellationToken).ConfigureAwait(false);
                await OnStartedAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "There is one exception thrown from the method OnStartedAsync().");
            }
        }

        protected virtual ValueTask OnStartedAsync()
        {
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask OnStopAsync()
        {
            return ValueTask.CompletedTask;
        }

        private async Task StopListener(IConnectionListener listener)
        {
            await listener.StopAsync().ConfigureAwait(false);
            _logger.LogInformation($"The listener [{listener}] has been stopped.");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            var state = _state;

            if (state != ServerState.Started)
            {
                throw new InvalidOperationException($"The server cannot be stopped right now, because its state is {state}.");
            }

            _state = ServerState.Stopping;

            var tasks = _connectionListeners.Where(l => l.IsRunning).Select(l => StopListener(l))
                .Union([Task.Run(ShutdownMiddlewares)]);

            await Task.WhenAll(tasks).ConfigureAwait(false);

            try
            {
                await OnStopAsync();
                await _nodeServerLifecycle.OnStop(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "There is an exception thrown from the method OnStopAsync().");
            }

            _state = ServerState.Stopped;
        }

        async Task<bool> IServer.StartAsync()
        {
            await StartAsync(CancellationToken.None);
            return true;
        }

        async Task IServer.StopAsync()
        {
            await StopAsync(CancellationToken.None);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        ValueTask IAsyncDisposable.DisposeAsync() => DisposeAsync(true);

        protected virtual async ValueTask DisposeAsync(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    try
                    {
                        if (_state == ServerState.Started)
                        {
                            await StopAsync(CancellationToken.None);
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Failed to stop the server");
                    }
                }

                disposedValue = true;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            DisposeAsync(disposing).GetAwaiter().GetResult();
        }

        void IDisposable.Dispose()
        {
            DisposeAsync(true).GetAwaiter().GetResult();
        }

        #endregion
    }
}
