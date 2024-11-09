using System.IO.Pipelines;
using ARWNI2S.Engine.Network.Configuration.Options;
using ARWNI2S.Engine.Network.Connection.Pipes;
using ARWNI2S.Infrastructure.Network.Connection;
using ARWNI2S.Infrastructure.Network.Connection.Pipes;

namespace ARWNI2S.Engine.Network.Connection
{
    public abstract class VirtualConnection : PipeConnection, IVirtualConnection
    {
        public VirtualConnection(ConnectionOptions options)
            : base(options)
        {

        }

        internal override Task FillPipeAsync(PipeWriter writer, ISupplyController supplyController, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async ValueTask<FlushResult> WritePipeDataAsync(Memory<byte> memory, CancellationToken cancellationToken)
        {
            return await Input.Writer.WriteAsync(memory, cancellationToken).ConfigureAwait(false);
        }
    }
}