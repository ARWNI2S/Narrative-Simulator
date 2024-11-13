using ARWNI2S.Infrastructure.Engine;
using ARWNI2S.Infrastructure.Engine.Builder;
using ARWNI2S.Infrastructure.Lifecycle;
using Microsoft.Extensions.DependencyInjection;
using ILifecycleSubject = ARWNI2S.Infrastructure.Lifecycle.ILifecycleSubject;
using ServiceLifecycleStage = ARWNI2S.Infrastructure.Lifecycle.ServiceLifecycleStage;

namespace ARWNI2S.Engine.Modules
{
    internal class EngineModuleBase : IEngineModule
    {
        public IFeatureCollection Features { get; }

        public EngineModuleBase()
        {

        }

        public void Configure(IEngineBuilder engineBuilder)
        {

        }

        public void ConfigureServices(IServiceCollection services)
        {

        }

        public void Participate(ILifecycleSubject lifecycle)
        {
            lifecycle.Subscribe(GetType().Name, ServiceLifecycleStage.RuntimeInitialize, ct => OnStartAsync(ct), ct => OnStopAsync(ct));
        }

        private Task OnStartAsync(CancellationToken token)
        {
            return Task.CompletedTask;
        }

        private Task OnStopAsync(CancellationToken token)
        {
            return Task.CompletedTask;
        }

    }
}
