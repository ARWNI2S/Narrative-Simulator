using ARWNI2S.Engine.Network.Connection;
using ARWNI2S.Infrastructure.Network.Connection;

namespace ARWNI2S.Engine.Network.Udp
{
    public class UdpConnectionFactory : IConnectionFactory
    {
        public Task<IConnection> CreateConnection(object connection, CancellationToken cancellationToken)
        {
            var connectionInfo = (UdpConnectionInfo)connection;

            return Task.FromResult<IConnection>(new UdpPipeConnection(connectionInfo.Socket, connectionInfo.ConnectionOptions, connectionInfo.RemoteEndPoint, connectionInfo.SessionIdentifier));
        }
    }
}