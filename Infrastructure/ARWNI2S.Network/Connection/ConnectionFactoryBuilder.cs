using ARWNI2S.Engine.Network.Connections;
using ARWNI2S.Infrastructure.Network.Connection;
using System.Net.Sockets;

namespace ARWNI2S.Engine.Network.Connection
{
    public class ConnectionFactoryBuilder : IConnectionFactoryBuilder
    {
        public Action<Socket> SocketOptionsSetter { get; }

        public IConnectionStreamInitializersFactory ConnectionStreamInitializersFactory { get; }

        public ConnectionFactoryBuilder(SocketOptionsSetter socketOptionsSetter, IConnectionStreamInitializersFactory connectionStreamInitializersFactory)
        {
            SocketOptionsSetter = socketOptionsSetter.Setter;
            ConnectionStreamInitializersFactory = connectionStreamInitializersFactory;
        }

        public virtual IConnectionFactory Build(ListenOptions listenOptions, ConnectionOptions connectionOptions)
        {
            return new TcpConnectionFactory(listenOptions, connectionOptions, SocketOptionsSetter, ConnectionStreamInitializersFactory);
        }
    }
}