using ARWNI2S.Engine.Network.Connection;
using ARWNI2S.Engine.Network.Connections;
using ARWNI2S.Infrastructure.Network.Connection;

namespace ARWNI2S.Engine.Network.Udp
{
    public class UdpConnectionFactoryBuilder : IConnectionFactoryBuilder
    {
        public IConnectionFactory Build(ListenOptions listenOptions, ConnectionOptions connectionOptions)
        {
            return new UdpConnectionFactory();
        }
    }
}