using SuperSocket.ProtoBase;

namespace ARWNI2S.Engine.Network.Pipeline
{
    public class PipelineFilterFactory : PipelineFilterFactoryBase<Protocol.NI2SProtoPacket>
    {
        public PipelineFilterFactory(IServiceProvider serviceProvider)
            : base(serviceProvider) { }

        protected override IPipelineFilter<Protocol.NI2SProtoPacket> CreateCore(object client)
        {
            return new PipelineFilterRoot();
        }
    }
}
