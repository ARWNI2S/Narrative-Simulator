using ARWNI2S.Infrastructure.Network.Protocol;

namespace ARWNI2S.Engine.Network.Proxy
{
    public class ProxyPipelineFilterFactory<TPackageInfo> : IPipelineFilterFactory<TPackageInfo>
    {
        private readonly IPipelineFilterFactory<TPackageInfo> _pipelineFilterFactory;

        public ProxyPipelineFilterFactory(IPipelineFilterFactory<TPackageInfo> pipelineFilterFactory)
        {
            _pipelineFilterFactory = pipelineFilterFactory;
        }

        public IPipelineFilter<TPackageInfo> Create(object client)
        {
            return new ProxyPipelineFilter<TPackageInfo>(_pipelineFilterFactory.Create(client));
        }
    }
}