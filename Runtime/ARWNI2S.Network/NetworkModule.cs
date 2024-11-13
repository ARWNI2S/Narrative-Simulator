using ARWNI2S.Infrastructure.Engine;
using ARWNI2S.Infrastructure.Engine.Builder;
using ARWNI2S.Infrastructure.Lifecycle;
using Microsoft.Extensions.DependencyInjection;

namespace ARWNI2S.Engine.Network
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
