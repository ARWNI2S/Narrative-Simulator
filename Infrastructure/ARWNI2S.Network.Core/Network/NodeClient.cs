using ARWNI2S.Engine.Network.Client;
using ARWNI2S.Engine.Network.Pipeline;
using ARWNI2S.Engine.Network.Protocol;
using Microsoft.Extensions.Logging;
using SuperSocket.Connection;

namespace ARWNI2S.Engine.Network
{
    public class DefaultNodeClient : NodeClient
    {
        public DefaultNodeClient(ILogger logger = null)
            : this(new ConnectionOptions { Logger = logger }) { }

        public DefaultNodeClient(ConnectionOptions options)
            : base(new PipelineFilterRoot(), new NI2SPacketEncoder(), options)
        {
        }

        protected DefaultNodeClient(NI2SPacketEncoder packageEncoder) : base(packageEncoder)
        {
        }
    }

    public class NodeClient : NodeClient<NI2SProtoPacket, NI2SProtoPacket>
    {
        protected NodeClient(NI2SPacketEncoder packageEncoder)
            : base(packageEncoder) { }

        public NodeClient(NI2SPacketFilter pipelineFilter, NI2SPacketEncoder packageEncoder, ILogger logger = null)
            : this(pipelineFilter, packageEncoder, new ConnectionOptions { Logger = logger }) { }

        public NodeClient(NI2SPacketFilter pipelineFilter, NI2SPacketEncoder packageEncoder, ConnectionOptions options)
            : base(pipelineFilter, packageEncoder, options) { }

    }
}
