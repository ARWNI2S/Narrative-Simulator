using System.Net.Sockets;
using ARWNI2S.Engine.Network.Configuration.Options;

namespace ARWNI2S.Engine.Network.Connection
{
    public interface IConnectionStreamInitializer
    {
        void Setup(ListenOptions listenOptions);

        Task<Stream> InitializeAsync(Socket socket, Stream stream, CancellationToken cancellationToken);
    }
}