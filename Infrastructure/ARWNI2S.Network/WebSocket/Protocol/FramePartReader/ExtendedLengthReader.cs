using ARWNI2S.Engine.Network.Protocol;
using ARWNI2S.Infrastructure.Network.Protocol;
using System.Buffers;

namespace ARWNI2S.Engine.Network.WebSocket.Protocol.FramePartReader
{
    class ExtendedLengthReader : PackagePartReader
    {
        public override bool Process(WebSocketPackage package, object filterContext, ref SequenceReader<byte> reader, out IPackagePartReader<WebSocketPackage> nextPartReader, out bool needMoreData)
        {
            int required;

            if (package.PayloadLength == 126)
                required = 2;
            else
                required = 8;

            if (reader.Remaining < required)
            {
                nextPartReader = null;
                needMoreData = true;
                return false;
            }

            needMoreData = false;

            if (required == 2)
            {
                reader.TryReadBigEndian(out ushort len);
                package.PayloadLength = len;
            }
            else // required == 8 (long)
            {
                reader.TryReadBigEndian(out long len);
                package.PayloadLength = len;
            }

            if (package.HasMask)
                nextPartReader = MaskKeyReader;
            else
                nextPartReader = PayloadDataReader;

            return false;
        }
    }
}
