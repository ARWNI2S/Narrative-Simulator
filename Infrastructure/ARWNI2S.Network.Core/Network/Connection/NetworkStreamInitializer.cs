using System.Net.Sockets;

namespace ARWNI2S.Engine.Network.Connection
{
    public class NetworkStreamInitializer : IConnectionStreamInitializer
    {
        public void Setup(ListenOptions listenOptions)
        {
        }

        public Task<Stream> InitializeAsync(Socket socket, Stream stream, CancellationToken cancellationToken)
        {
            return Task.FromResult<Stream>(new NetworkStream(socket, true));
        }
    }
}