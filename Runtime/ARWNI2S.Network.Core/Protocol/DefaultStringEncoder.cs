using ARWNI2S.Infrastructure.Network.Protocol;
using System.Buffers;
using System.Text;

namespace ARWNI2S.Engine.Network.Protocol
{
    public class DefaultStringEncoder : IPackageEncoder<string>
    {
        private Encoding _encoding;

        public DefaultStringEncoder()
            : this(new UTF8Encoding(false))
        {

        }

        public DefaultStringEncoder(Encoding encoding)
        {
            _encoding = encoding;
        }

        public int Encode(IBufferWriter<byte> writer, string pack)
        {
            return writer.Write(pack, _encoding);
        }
    }
}