using SuperSocket.ProtoBase;
using System.Buffers;

namespace ARWNI2S.Engine.Network.Protocol
{
    public abstract class NI2SPacketFilter : IPipelineFilter<NI2SProtoPacket>
    {
        protected int packetBodyLength;
        protected NI2SProtoPacket currentPacket;

        public virtual NI2SPacketDecoder Decoder { get; set; }
        public virtual NI2SPacketFilter NextFilter { get; }
        public virtual object Context { get; set; }

        protected NI2SPacketFilter()
            : this(null, null) { }

        protected NI2SPacketFilter(NI2SPacketFilter next)
            : this(null, next) { }

        protected NI2SPacketFilter(NI2SPacketDecoder decoder, NI2SPacketFilter next)
        {
            Decoder = decoder;
            NextFilter = next;
        }

        public abstract NI2SProtoPacket Filter(ref SequenceReader<byte> reader);

        public virtual void Reset()
        {
            packetBodyLength = 0;
            currentPacket = null;
        }

        IPackageDecoder<NI2SProtoPacket> IPipelineFilter<NI2SProtoPacket>.Decoder { get => Decoder; set => Decoder = value as NI2SPacketDecoder; }

        IPipelineFilter<NI2SProtoPacket> IPipelineFilter<NI2SProtoPacket>.NextFilter => NextFilter;

    }
}
