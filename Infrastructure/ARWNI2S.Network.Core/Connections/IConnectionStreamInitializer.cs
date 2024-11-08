using System.Net.Sockets;

namespace ARWNI2S.Engine.Network.Connections
{
    public interface IConnectionStreamInitializer
    {
        void Setup(ListenOptions listenOptions);

        Task<Stream> InitializeAsync(Socket socket, Stream stream, CancellationToken cancellationToken);
    }
}