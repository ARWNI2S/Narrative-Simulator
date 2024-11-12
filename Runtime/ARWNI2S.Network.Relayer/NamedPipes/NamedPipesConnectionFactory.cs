using ARWNI2S.Engine.Network.Configuration.Options;
using ARWNI2S.Infrastructure.Network.Connection;
using System.IO.Pipes;

namespace ARWNI2S.Engine.Network.NamedPipes
{
    public class NamedPipesConnectionFactory : NamedPipesConnectionFactoryBase
    {
        public NamedPipesConnectionFactory(
            ListenOptions listenOptions,
            ConnectionOptions connectionOptions)
            : base(listenOptions, connectionOptions)
        {

        }

        public override async Task<IConnection> CreateConnection(object connection, CancellationToken cancellationToken)
        {
            var pipeStream = connection as NamedPipeServerStream;

            return await Task.FromResult(new NamedPipeConnection(pipeStream, ConnectionOptions));
        }
    }
}