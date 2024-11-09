using ARWNI2S.Infrastructure.Network.Connection;
using ARWNI2S.Infrastructure.Network.Protocol;
using ARWNI2S.Infrastructure.Network.Proxy;
using System.IO.Pipelines;
using System.Net;
using Reason = ARWNI2S.Infrastructure.Network.Connection.CloseReason;

namespace ARWNI2S.Engine.Network.Connection
{
    public abstract class ConnectionBase : IConnection
    {
        public abstract IAsyncEnumerable<TPackageInfo> RunAsync<TPackageInfo>(IPipelineFilter<TPackageInfo> pipelineFilter);

        public abstract ValueTask SendAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default);

        public abstract ValueTask SendAsync<TPackage>(IPackageEncoder<TPackage> packageEncoder, TPackage package, CancellationToken cancellationToken = default);

        public abstract ValueTask SendAsync(Action<PipeWriter> write, CancellationToken cancellationToken = default);

        public bool IsClosed { get; private set; }

        public EndPoint RemoteEndPoint { get; protected set; }

        public EndPoint LocalEndPoint { get; protected set; }

        public CloseReason? CloseReason { get; protected set; }

        public DateTimeOffset LastActiveTime { get; protected set; } = DateTimeOffset.Now;

        public CancellationToken ConnectionToken { get; protected set; }

        public ProxyInfo ProxyInfo { get; protected set; }

        protected virtual void OnClosed()
        {
            IsClosed = true;

            var closed = Closed;

            if (closed == null)
                return;

            if (Interlocked.CompareExchange(ref Closed, null, closed) != closed)
                return;

            var closeReason = CloseReason ?? Reason.Unknown;

            closed.Invoke(this, new CloseEventArgs(closeReason));
        }

        public event EventHandler<CloseEventArgs> Closed;

        public abstract ValueTask CloseAsync(CloseReason closeReason);

        public abstract ValueTask DetachAsync();
    }
}
