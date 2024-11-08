using System.Buffers;
using ARWNI2S.Engine.Network.Protocol.Filters;
using ARWNI2S.Infrastructure.Network.Protocol;
using ARWNI2S.Infrastructure.Network.Proxy;

namespace ARWNI2S.Engine.Network.Proxy
{
    public class ProxyPipelineFilter<TPackageInfo> : PackagePartsPipelineFilter<TPackageInfo>, IProxyProtocolPipelineFilter
    {
        private readonly IPipelineFilter<TPackageInfo> _applicationPipelineFilter;

        private object _originalFilterContext;

        public ProxyInfo ProxyInfo { get; private set; }

        public ProxyPipelineFilter(IPipelineFilter<TPackageInfo> applicationPipelineFilter)
        {
            _applicationPipelineFilter = applicationPipelineFilter;
        }

        protected override TPackageInfo CreatePackage()
        {
            return default;
        }

        protected override IPackagePartReader<TPackageInfo> GetFirstPartReader()
        {
            return ProxyPackagePartReader<TPackageInfo>.ProxyProtocolSwitch;
        }

        public override void Reset()
        {
            // This method will be called when the proxy package handling finishes
            ProxyInfo.Prepare();
            NextFilter = _applicationPipelineFilter;
            base.Reset();
            Context = _originalFilterContext;
        }

        public override TPackageInfo Filter(ref SequenceReader<byte> reader)
        {
            if (ProxyInfo == null)
            {
                _originalFilterContext = Context;
                Context = ProxyInfo = new ProxyInfo();
            }

            return base.Filter(ref reader);
        }
    }
}