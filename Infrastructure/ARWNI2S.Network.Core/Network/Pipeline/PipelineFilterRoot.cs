using ARWNI2S.Engine.Network.Protocol;
using System.Buffers;

namespace ARWNI2S.Engine.Network.Pipeline
{
    public sealed class PipelineFilterRoot : NI2SPacketFilter
    {
        public override NI2SProtoPacket Filter(ref SequenceReader<byte> reader)
        {
            throw new NotImplementedException();
        }
    }
}