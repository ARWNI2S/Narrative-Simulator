using ARWNI2S.Engine.Network.Configuration.Options;
using ARWNI2S.Engine.Network.Connection;
using ARWNI2S.Infrastructure.Network.Connection;

namespace ARWNI2S.Engine.Network.NamedPipes
{
    public class NamedPipesConnectionFactoryBuilder : IConnectionFactoryBuilder
    {
        public IConnectionFactory Build(ListenOptions listenOptions, ConnectionOptions connectionOptions)
        {
            return new NamedPipesConnectionFactory(listenOptions, connectionOptions);
        }
    }
}