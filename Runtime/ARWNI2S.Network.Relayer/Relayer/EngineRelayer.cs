using ARWNI2S.Engine.Network.Configuration.Options;
using ARWNI2S.Engine.Network.Session;
using ARWNI2S.Infrastructure.Lifecycle;
using ARWNI2S.Infrastructure.Network.Connection;
using Microsoft.Extensions.Logging;

namespace ARWNI2S.Engine.Network.Relayer
{
    internal class EngineRelayer : IRelayServer, ILifecycleParticipant<IEngineLifecycle>
    {
        public void Participate(IEngineLifecycle lifecycle)
        {
            lifecycle.Subscribe(
                nameof(EngineRelayer),
                ServiceLifecycleStage.RuntimeInitialize,
                StartAsync,
                StopAsync);
        }
















        public string Name => throw new NotImplementedException();

        public ServerOptions Options => throw new NotImplementedException();

        public object DataContext { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int SessionCount => throw new NotImplementedException();

        public IServiceProvider ServiceProvider => throw new NotImplementedException();

        public ServerState State => throw new NotImplementedException();

        public ILogger Logger => throw new NotImplementedException();

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }

        public ValueTask HandleSessionClosedEvent(INodeSession session, CloseReason reason)
        {
            throw new NotImplementedException();
        }

        public ValueTask HandleSessionConnectedEvent(INodeSession session)
        {
            throw new NotImplementedException();
        }

        public Task RegisterConnection(object connection)
        {
            throw new NotImplementedException();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
