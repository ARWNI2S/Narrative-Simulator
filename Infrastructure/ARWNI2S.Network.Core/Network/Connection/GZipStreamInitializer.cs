using System.IO.Compression;
using System.Net.Sockets;
using ARWNI2S.Infrastructure.Network.Connection;

namespace ARWNI2S.Engine.Network.Connection
{
    public class GZipStreamInitializer : IConnectionStreamInitializer
    {
        public CompressionLevel CompressionLevel { get; private set; }

        public Task<Stream> InitializeAsync(Socket socket, Stream stream, CancellationToken cancellationToken)
        {
            var connectionStream = new ReadWriteDelegateStream(
                stream,
                new GZipStream(stream, CompressionMode.Decompress),
                new GZipStream(stream, CompressionLevel));

            return Task.FromResult<Stream>(connectionStream);
        }

        public void Setup(ListenOptions listenOptions)
        {
            CompressionLevel = CompressionLevel.Optimal;
        }
    }
}