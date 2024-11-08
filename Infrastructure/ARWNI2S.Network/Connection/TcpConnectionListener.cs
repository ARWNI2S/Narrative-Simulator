using ARWNI2S.Engine.Network.Connections;
using ARWNI2S.Infrastructure;
using ARWNI2S.Infrastructure.Extensions;
using ARWNI2S.Infrastructure.Network.Connection;
using Microsoft.Extensions.Logging;
using System.Net.Sockets;

namespace ARWNI2S.Engine.Network.Connection
{
    public class TcpConnectionListener : IConnectionListener
    {
        private Socket _listenSocket;

        private CancellationTokenSource _cancellationTokenSource;
        private TaskCompletionSource<bool> _stopTaskCompletionSource;
        public IConnectionFactory ConnectionFactory { get; }
        public ListenOptions Options { get; }
        private ILogger _logger;

        public TcpConnectionListener(ListenOptions options, IConnectionFactory connectionFactory, ILogger logger)
        {
            Options = options;
            ConnectionFactory = connectionFactory;
            _logger = logger;
        }

        public bool IsRunning { get; private set; }

        public bool Start()
        {
            var options = Options;

            try
            {
                var listenEndpoint = options.ToEndPoint();
                var listenSocket = _listenSocket = new Socket(listenEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                listenSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                listenSocket.LingerState = new LingerOption(false, 0);

                if (options.NoDelay)
                    listenSocket.NoDelay = true;

                listenSocket.Bind(listenEndpoint);
                listenSocket.Listen(options.BackLog);

                IsRunning = true;

                _cancellationTokenSource = new CancellationTokenSource();

                KeepAccept(listenSocket).DoNotAwait();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"The listener[{ToString()}] failed to start.");
                return false;
            }
        }

        private async Task KeepAccept(Socket listenSocket)
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    var client = await listenSocket.AcceptAsync().ConfigureAwait(false);
                    OnNewClientAccept(client);
                }
                catch (Exception e)
                {
                    if (e is ObjectDisposedException || e is NullReferenceException)
                        break;

                    if (e is SocketException se)
                    {
                        var errorCode = se.ErrorCode;

                        //The listen socket was closed
                        if (errorCode == 125 || errorCode == 89 || errorCode == 995 || errorCode == 10004 || errorCode == 10038)
                        {
                            break;
                        }
                    }

                    _logger.LogError(e, $"Listener[{ToString()}] failed to do AcceptAsync");
                    continue;
                }
            }

            _stopTaskCompletionSource.TrySetResult(true);
        }

        public event NewConnectionAcceptHandler NewConnectionAccept;

        private async void OnNewClientAccept(Socket socket)
        {
            var handler = NewConnectionAccept;

            if (handler == null)
                return;

            IConnection connection = null;

            try
            {
                using var cts = CancellationTokenSourcePool.Shared.Rent(Options.ConnectionAcceptTimeOut);
                connection = await ConnectionFactory.CreateConnection(socket, cts.Token);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to create channel for {socket.RemoteEndPoint}.");
                return;
            }

            await handler.Invoke(Options, connection);
        }

        public Task StopAsync()
        {
            var listenSocket = _listenSocket;

            if (listenSocket == null)
                return Task.Delay(0);

            _stopTaskCompletionSource = new TaskCompletionSource<bool>();

            _cancellationTokenSource.Cancel();
            listenSocket.Close();

            return _stopTaskCompletionSource.Task;
        }

        public override string ToString()
        {
            return Options?.ToString();
        }
    }
}