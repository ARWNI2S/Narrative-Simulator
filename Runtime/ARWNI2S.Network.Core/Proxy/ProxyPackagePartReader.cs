using ARWNI2S.Infrastructure.Network.Protocol;
using System.Buffers;

namespace ARWNI2S.Engine.Network.Proxy
{
    abstract class ProxyPackagePartReader<TPackageInfo> : IPackagePartReader<TPackageInfo>
    {
        static ProxyPackagePartReader()
        {
            ProxyProtocolSwitch = new ProxySwitchPartReader<TPackageInfo>();
            ProxyProtocolV1Reader = new ProxyV1PartReader<TPackageInfo>();
            ProxyProtocolV2Reader = new ProxyV2PartReader<TPackageInfo>();
        }

        public abstract bool Process(TPackageInfo package, object filterContext, ref SequenceReader<byte> reader, out IPackagePartReader<TPackageInfo> nextPartReader, out bool needMoreData);

        internal static IPackagePartReader<TPackageInfo> ProxyProtocolSwitch { get; }

        internal static IPackagePartReader<TPackageInfo> ProxyProtocolV1Reader { get; }

        internal static IPackagePartReader<TPackageInfo> ProxyProtocolV2Reader { get; }
    }
}