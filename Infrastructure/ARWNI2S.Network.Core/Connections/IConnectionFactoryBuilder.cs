using ARWNI2S.Engine.Network.Connection;
using ARWNI2S.Infrastructure.Network.Connection;

namespace ARWNI2S.Engine.Network.Connections
{
    public interface IConnectionFactoryBuilder
    {
        IConnectionFactory Build(ListenOptions listenOptions, ConnectionOptions connectionOptions);
    }
}