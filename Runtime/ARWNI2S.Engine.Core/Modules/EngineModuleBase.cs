using ARWNI2S.Engine.Modules;
using ARWNI2S.Infrastructure.Engine;
using ARWNI2S.Infrastructure.Engine.Builder;
using ARWNI2S.Infrastructure.Lifecycle;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using ILifecycleSubject = ARWNI2S.Infrastructure.Lifecycle.ILifecycleSubject;
using ServiceLifecycleStage = ARWNI2S.Infrastructure.Lifecycle.ServiceLifecycleStage;

#pragma warning disable IDE0130 // El espacio de nombres no coincide con la estructura de carpetas
namespace ARWNI2S.Engine
#pragma warning restore IDE0130 // El espacio de nombres no coincide con la estructura de carpetas
{
    [EngineModule(ServiceLifecycleStage.RuntimeInitialize)]
    public abstract class EngineModuleBase : IEngineModule
    {
        private readonly int _engineLifecycleStage;

        public IFeatureCollection Features { get; }

        public EngineModuleBase()
        {
            _engineLifecycleStage = this.GetType().GetCustomAttribute<EngineModuleAttribute>().Stage;
        }

        public void Configure(IEngineBuilder engineBuilder)
        {

        }

        public void ConfigureServices(IServiceCollection services)
        {

        }

        public void Participate(ILifecycleSubject lifecycle)
        {
            lifecycle.Subscribe(GetType().Name, _engineLifecycleStage, ct => OnStartAsync(ct), ct => OnStopAsync(ct));
        }

        protected abstract void ConfigureEngine(IEngineBuilder engine);
        protected abstract void ConfigureModuleServices(IServiceCollection services);

        protected virtual Task OnStartAsync(CancellationToken token)
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnStopAsync(CancellationToken token)
        {
            return Task.CompletedTask;
        }

    }
}
