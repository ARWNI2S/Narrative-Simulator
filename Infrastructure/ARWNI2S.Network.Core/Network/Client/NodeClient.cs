using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SuperSocket.Connection;
using SuperSocket.ProtoBase;
using System.Buffers;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Security.Authentication;

namespace ARWNI2S.Engine.Network.Client
{
    public class NodeClient<TPackage, TSendPackage> : NodeClient<TPackage>, INodeClient<TPackage, TSendPackage>
        where TPackage : class
    {
        private IPackageEncoder<TSendPackage> _packageEncoder;

        protected NodeClient(IPackageEncoder<TSendPackage> packageEncoder)
            : base()
        {
            _packageEncoder = packageEncoder;
        }

        public NodeClient(IPipelineFilter<TPackage> pipelineFilter, IPackageEncoder<TSendPackage> packageEncoder, ILogger logger = null)
            : this(pipelineFilter, packageEncoder, new ConnectionOptions { Logger = logger })
        {

        }

        public NodeClient(IPipelineFilter<TPackage> pipelineFilter, IPackageEncoder<TSendPackage> packageEncoder, ConnectionOptions options)
            : base(pipelineFilter, options)
        {
            _packageEncoder = packageEncoder;
        }

        public virtual async ValueTask SendAsync(TSendPackage package)
        {
            await SendAsync(_packageEncoder, package);
        }

        public new INodeClient<TPackage, TSendPackage> AsClient()
        {
            return this;
        }
    }

    public class NodeClient<TReceivePackage> : INodeClient<TReceivePackage>
        where TReceivePackage : class
    {
        private IPipelineFilter<TReceivePackage> _pipelineFilter;

        protected IConnection Connection { get; private set; }

        public IConnector Proxy { get; set; }

        protected ILogger Logger { get; set; }

        protected ConnectionOptions Options { get; private set; }

        IAsyncEnumerator<TReceivePackage> _packageStream;

        public event PackageHandler<TReceivePackage> PackageHandler;

        public IPEndPoint LocalEndPoint { get; set; }

        public SecurityOptions Security { get; set; }

        public CompressionLevel CompressionLevel { get; set; } = CompressionLevel.NoCompression;

        protected NodeClient()
        {

        }

        public NodeClient(IPipelineFilter<TReceivePackage> pipelineFilter)
            : this(pipelineFilter, NullLogger.Instance)
        {

        }

        public NodeClient(IPipelineFilter<TReceivePackage> pipelineFilter, ILogger logger)
            : this(pipelineFilter, new ConnectionOptions { Logger = logger })
        {

        }

        public NodeClient(IPipelineFilter<TReceivePackage> pipelineFilter, ConnectionOptions options)
        {
            ArgumentNullException.ThrowIfNull(pipelineFilter);
            ArgumentNullException.ThrowIfNull(options);

            _pipelineFilter = pipelineFilter;
            Options = options;
            Logger = options.Logger;
        }

        public virtual INodeClient<TReceivePackage> AsClient()
        {
            return this;
        }

        protected virtual IConnector GetConnector()
        {
            var connectors = new List<IConnector>
            {
                new SocketConnector(LocalEndPoint)
            };

            var security = Security;

            if (security != null)
            {
                if (security.EnabledSslProtocols != SslProtocols.None)
                    connectors.Add(new SslStreamConnector(security));
            }

            if (Proxy is IConnector proxy)
            {
                connectors.Add(proxy);
            }

            if (CompressionLevel != CompressionLevel.NoCompression)
            {
                connectors.Add(new GZipConnector(CompressionLevel));
            }

            return BuildConnectors(connectors);
        }

        protected IConnector BuildConnectors(IEnumerable<IConnector> connectors)
        {
            var prevConnector = default(ConnectorBase);

            foreach (var connector in connectors)
            {
                if (prevConnector != null)
                {
                    prevConnector.NextConnector = connector;
                }

                prevConnector = connector as ConnectorBase;
            }

            return connectors.First();
        }

        ValueTask<bool> INodeClient<TReceivePackage>.ConnectAsync(EndPoint remoteEndPoint, CancellationToken cancellationToken)
        {
            return ConnectAsync(remoteEndPoint, cancellationToken);
        }

        protected virtual async ValueTask<bool> ConnectAsync(EndPoint remoteEndPoint, CancellationToken cancellationToken)
        {
            var connector = GetConnector();
            var state = await connector.ConnectAsync(remoteEndPoint, null, cancellationToken);

            if (state.Cancelled || cancellationToken.IsCancellationRequested)
            {
                OnError($"The connection to {remoteEndPoint} was cancelled.", state.Exception);
                return false;
            }

            if (!state.Result)
            {
                OnError($"Failed to connect to {remoteEndPoint}", state.Exception);
                return false;
            }

            var socket = state.Socket;

            if (socket == null)
                throw new Exception("Socket is null.");

            SetupConnection(state.CreateConnection(Options));
            return true;
        }

        public void AsUdp(IPEndPoint remoteEndPoint, ArrayPool<byte> bufferPool = null, int bufferSize = 4096)
        {
            var localEndPoint = LocalEndPoint;

            if (localEndPoint == null)
            {
                localEndPoint = new IPEndPoint(remoteEndPoint.AddressFamily == AddressFamily.InterNetworkV6 ? IPAddress.IPv6Any : IPAddress.Any, 0);
            }

            var socket = new Socket(remoteEndPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

            // bind the local endpoint
            socket.Bind(localEndPoint);

            var connection = new UdpPipeConnection(socket, Options, remoteEndPoint);

            SetupConnection(connection);

            UdpReceive(socket, connection, bufferPool, bufferSize);
        }

        private async void UdpReceive(Socket socket, UdpPipeConnection connection, ArrayPool<byte> bufferPool, int bufferSize)
        {
            if (bufferPool == null)
                bufferPool = ArrayPool<byte>.Shared;

            while (true)
            {
                var buffer = bufferPool.Rent(bufferSize);

                try
                {
                    var result = await socket
                        .ReceiveFromAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), SocketFlags.None, connection.RemoteEndPoint)
                        .ConfigureAwait(false);

                    await connection.WritePipeDataAsync(new ArraySegment<byte>(buffer, 0, result.ReceivedBytes).AsMemory(), CancellationToken.None);
                }
                catch (NullReferenceException)
                {
                    break;
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
                catch (Exception e)
                {
                    OnError($"Failed to receive UDP data.", e);
                }
                finally
                {
                    bufferPool.Return(buffer);
                }
            }
        }

        protected virtual void SetupConnection(IConnection connection)
        {
            connection.Closed += OnConnectionClosed;
            _packageStream = connection.GetPackageStream(_pipelineFilter);
            Connection = connection;
        }

        ValueTask<TReceivePackage> INodeClient<TReceivePackage>.ReceiveAsync()
        {
            return ReceiveAsync();
        }

        /// <summary>
        /// Try to receive one package
        /// </summary>
        /// <returns></returns>
        protected virtual async ValueTask<TReceivePackage> ReceiveAsync()
        {
            var p = await _packageStream.ReceiveAsync();

            if (p != null)
                return p;

            OnClosed(Connection, EventArgs.Empty);
            return null;
        }

        void INodeClient<TReceivePackage>.StartReceive()
        {
            StartReceive();
        }

        /// <summary>
        /// Start receive packages and handle the packages by event handler
        /// </summary>
        protected virtual void StartReceive()
        {
            StartReceiveAsync();
        }

        private async void StartReceiveAsync()
        {
            var enumerator = _packageStream;

            while (await enumerator.MoveNextAsync())
            {
                await OnPackageReceived(enumerator.Current);
            }
        }

        protected virtual async ValueTask OnPackageReceived(TReceivePackage package)
        {
            var handler = PackageHandler;

            try
            {
                await handler.Invoke(this, package);
            }
            catch (Exception e)
            {
                OnError("Unhandled exception happened in PackageHandler.", e);
            }
        }

        private void OnConnectionClosed(object sender, EventArgs e)
        {
            Connection.Closed -= OnConnectionClosed;
            OnClosed(this, e);
        }

        protected virtual void OnClosed(object sender, EventArgs e)
        {
            var handler = Closed;

            if (handler != null)
            {
                if (Interlocked.CompareExchange(ref Closed, null, handler) == handler)
                {
                    handler.Invoke(sender, e);
                }
            }
        }

        protected virtual void OnError(string message, Exception exception)
        {
            Logger?.LogError(exception, message);
        }

        protected virtual void OnError(string message)
        {
            Logger?.LogError(message);
        }

        ValueTask INodeClient<TReceivePackage>.SendAsync(ReadOnlyMemory<byte> data)
        {
            return SendAsync(data);
        }

        protected virtual async ValueTask SendAsync(ReadOnlyMemory<byte> data)
        {
            await Connection.SendAsync(data);
        }

        ValueTask INodeClient<TReceivePackage>.SendAsync<TSendPackage>(IPackageEncoder<TSendPackage> packageEncoder, TSendPackage package)
        {
            return SendAsync(packageEncoder, package);
        }

        protected virtual async ValueTask SendAsync<TSendPackage>(IPackageEncoder<TSendPackage> packageEncoder, TSendPackage package)
        {
            await Connection.SendAsync(packageEncoder, package);
        }

        public event EventHandler Closed;

        public virtual async ValueTask CloseAsync()
        {
            await Connection.CloseAsync(CloseReason.LocalClosing);
            OnClosed(this, EventArgs.Empty);
        }
    }
}
