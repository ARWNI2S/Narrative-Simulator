using ARWNI2S.Engine.Network.Configuration.Options;
using ARWNI2S.Engine.Network.Connection;
using ARWNI2S.Engine.Network.Connection.Pipes;
using ARWNI2S.Infrastructure.Network.Connection;
using System.Net.Sockets;

namespace ARWNI2S.Engine.Network.Tcp
{
    public class TcpConnectionFactory : TcpConnectionFactoryBase
    {
        public TcpConnectionFactory(
            ListenOptions listenOptions,
            ConnectionOptions connectionOptions,
            Action<Socket> socketOptionsSetter,
            IConnectionStreamInitializersFactory connectionStreamInitializersFactory)
            : base(listenOptions, connectionOptions, socketOptionsSetter, connectionStreamInitializersFactory)
        {

        }

        public override async Task<IConnection> CreateConnection(object connection, CancellationToken cancellationToken)
        {
            var socket = connection as Socket;

            ApplySocketOptions(socket);

            if (ConnectionStreamInitializers is IEnumerable<IConnectionStreamInitializer> connectionStreamInitializers
                && connectionStreamInitializers.Any())
            {
                var stream = default(Stream);

                foreach (var initializer in connectionStreamInitializers)
                {
                    stream = await initializer.InitializeAsync(socket, stream, cancellationToken);
                }

                return new StreamPipeConnection(stream, socket.RemoteEndPoint, socket.LocalEndPoint, ConnectionOptions);
            }

            return new TcpPipeConnection(socket, ConnectionOptions);
        }
    }
}