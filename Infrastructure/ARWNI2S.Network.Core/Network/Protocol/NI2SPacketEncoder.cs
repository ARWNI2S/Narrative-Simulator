using SuperSocket.ProtoBase;
using System.Buffers;

namespace ARWNI2S.Engine.Network.Protocol
{
    public class NI2SPacketEncoder : IPackageEncoder<NI2SProtoPacket>
    {
        public int Encode(IBufferWriter<byte> writer, NI2SProtoPacket pack)
        {
            throw new NotImplementedException();
        }
    }
}
