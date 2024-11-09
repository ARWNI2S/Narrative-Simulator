using ARWNI2S.Infrastructure.Network.Protocol;

namespace ARWNI2S.Engine.Network.Protocol.Factory
{
    public class DelegatePipelineFilterFactory<TPackageInfo> : PipelineFilterFactoryBase<TPackageInfo>
    {
        private readonly Func<object, IPipelineFilter<TPackageInfo>> _factory;

        public DelegatePipelineFilterFactory(IServiceProvider serviceProvider, Func<object, IPipelineFilter<TPackageInfo>> factory)
            : base(serviceProvider)
        {
            _factory = factory;
        }

        protected override IPipelineFilter<TPackageInfo> CreateCore(object client)
        {
            return _factory(client);
        }
    }
}