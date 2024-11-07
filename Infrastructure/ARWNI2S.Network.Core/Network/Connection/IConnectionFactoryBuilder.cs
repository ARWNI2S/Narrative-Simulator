using ARWNI2S.Infrastructure.Network.Connection;

namespace ARWNI2S.Engine.Network.Connection
{
    public interface IConnectionFactoryBuilder
    {
        IConnectionFactory Build(ListenOptions listenOptions, ConnectionOptions connectionOptions);
    }
}