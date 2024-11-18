using ARWNI2S.Infrastructure.Engine;
using ARWNI2S.Infrastructure.Engine.Builder;
using ARWNI2S.Infrastructure.Lifecycle;
using Microsoft.Extensions.DependencyInjection;

#pragma warning disable IDE0130 // El espacio de nombres no coincide con la estructura de carpetas
namespace ARWNI2S.Engine
#pragma warning restore IDE0130 // El espacio de nombres no coincide con la estructura de carpetas
{
    public sealed class NetworkModule : IEngineModule
    {
        public IFeatureCollection Features { get; }

        public IServiceProvider Services { get; }

        public NetworkModule()
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
            lifecycle.Subscribe(nameof(NetworkModule), ServiceLifecycleStage.RuntimeInitialize, ct => OnStartAsync(ct), ct => OnStopAsync(ct));
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
