using ARWNI2S.Infrastructure.Network.Protocol;
using System.Buffers;

namespace ARWNI2S.Engine.Network.WebSocket.Protocol.FramePartReader
{
    class MaskKeyReader : PackagePartReader
    {
        public override bool Process(WebSocketPackage package, object filterContext, ref SequenceReader<byte> reader, out IPackagePartReader<WebSocketPackage> nextPartReader, out bool needMoreData)
        {
            int required = 4;

            if (reader.Remaining < required)
            {
                nextPartReader = null;
                needMoreData = true;
                return false;
            }

            needMoreData = false;

            package.MaskKey = reader.Sequence.Slice(reader.Consumed, 4).ToArray();
            reader.Advance(4);

            if (TryInitIfEmptyMessage(package))
            {
                nextPartReader = null;
                return true;
            }

            nextPartReader = PayloadDataReader;
            return false;
        }
    }
}
