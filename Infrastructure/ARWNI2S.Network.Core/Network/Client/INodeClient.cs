using SuperSocket.ProtoBase;
using System.Net;

namespace ARWNI2S.Engine.Network.Client
{
    public interface INodeClient<TReceivePackage, TSendPackage> : INodeClient<TReceivePackage>
        where TReceivePackage : class
    {
        ValueTask SendAsync(TSendPackage package);
    }


    public interface INodeClient<TReceivePackage>
        where TReceivePackage : class
    {
        IConnector Proxy { get; set; }

        ValueTask<bool> ConnectAsync(EndPoint remoteEndPoint, CancellationToken cancellationToken = default);

        ValueTask<TReceivePackage> ReceiveAsync();

        IPEndPoint LocalEndPoint { get; set; }

        SecurityOptions Security { get; set; }

        void StartReceive();

        ValueTask SendAsync(ReadOnlyMemory<byte> data);

        ValueTask SendAsync<TSendPackage>(IPackageEncoder<TSendPackage> packageEncoder, TSendPackage package);

        event EventHandler Closed;

        event PackageHandler<TReceivePackage> PackageHandler;

        ValueTask CloseAsync();
    }
}