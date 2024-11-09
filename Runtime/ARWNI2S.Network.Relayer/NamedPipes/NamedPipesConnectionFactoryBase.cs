using ARWNI2S.Engine.Network.Configuration.Options;
using ARWNI2S.Engine.Network.Connection;
using ARWNI2S.Infrastructure.Network.Connection;
using Microsoft.Extensions.Logging;

namespace ARWNI2S.Engine.Network.NamedPipes
{
    public abstract class NamedPipesConnectionFactoryBase : IConnectionFactory
    {
        protected ListenOptions ListenOptions { get; }

        protected ConnectionOptions ConnectionOptions { get; }

        protected ILogger Logger { get; }

        protected IEnumerable<IConnectionStreamInitializer> ConnectionStreamInitializers { get; }

        public NamedPipesConnectionFactoryBase(
            ListenOptions listenOptions,
            ConnectionOptions connectionOptions)
        {
            ListenOptions = listenOptions;
            ConnectionOptions = connectionOptions;
            Logger = connectionOptions.Logger;
        }

        public abstract Task<IConnection> CreateConnection(object connection, CancellationToken cancellationToken);
    }
}