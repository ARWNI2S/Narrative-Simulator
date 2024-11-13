using ARWNI2S.Infrastructure.Engine;
using ARWNI2S.Infrastructure.Engine.Builder;
using ARWNI2S.Infrastructure.Lifecycle;
using Microsoft.Extensions.DependencyInjection;

namespace ARWNI2S.Engine.Network
{
    public class EngineRelayModule : IEngineModule
    {
        public IFeatureCollection Features => throw new NotImplementedException();

        public EngineRelayModule() 
        {

        }

        public void Configure(IEngineBuilder engineBuilder)
        {
            throw new NotImplementedException();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            throw new NotImplementedException();
        }

        public void Participate(ILifecycleSubject lifecycle)
        {
            throw new NotImplementedException();
        }
    }
}
