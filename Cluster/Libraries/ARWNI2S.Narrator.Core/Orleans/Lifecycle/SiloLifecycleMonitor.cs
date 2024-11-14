using ARWNI2S.Node.Core.Events;

namespace ARWNI2S.Engine.Orleans.Lifecycle
{
    internal class SiloLifecycleMonitor : ILifecycleParticipant<ISiloLifecycle>
    {
        private readonly INodeEventPublisher _nodeEventPublisher;

        public SiloLifecycleMonitor(INodeEventPublisher nodeEventPublisher)
        {
            _nodeEventPublisher = nodeEventPublisher;
        }

        public void Participate(ISiloLifecycle lifecycle)
        {
            // Registrar acciones en el ciclo de vida del silo
            lifecycle.Subscribe(nameof(SiloLifecycleMonitor), ServiceLifecycleStage.First, OnFirstSiloLicecycleStart, OnFirstSiloLicecycleStop);
            lifecycle.Subscribe(nameof(SiloLifecycleMonitor), ServiceLifecycleStage.RuntimeInitialize, OnRuntimeInitializeLicecycleStart, OnRuntimeInitializeLicecycleStop);
            lifecycle.Subscribe(nameof(SiloLifecycleMonitor), ServiceLifecycleStage.RuntimeServices, OnRuntimeServicesLicecycleStart, OnRuntimeServicesLicecycleStop);
            lifecycle.Subscribe(nameof(SiloLifecycleMonitor), ServiceLifecycleStage.RuntimeStorageServices, OnRuntimeStorageServicesLicecycleStart, OnRuntimeStorageServicesLicecycleStop);
            lifecycle.Subscribe(nameof(SiloLifecycleMonitor), ServiceLifecycleStage.RuntimeGrainServices, OnRuntimeGrainServicesLicecycleStart, OnRuntimeGrainServicesLicecycleStop);
            lifecycle.Subscribe(nameof(SiloLifecycleMonitor), ServiceLifecycleStage.AfterRuntimeGrainServices, OnAfterRuntimeGrainServicesLicecycleStart, OnAfterRuntimeGrainServicesLicecycleStop);
            lifecycle.Subscribe(nameof(SiloLifecycleMonitor), ServiceLifecycleStage.ApplicationServices, OnApplicationServicesLicecycleStart, OnApplicationServicesLicecycleStop);
            lifecycle.Subscribe(nameof(SiloLifecycleMonitor), ServiceLifecycleStage.BecomeActive, OnSiloBecomeActiveLicecycleStart, OnSiloBecomeActiveLicecycleStop);
            lifecycle.Subscribe(nameof(SiloLifecycleMonitor), ServiceLifecycleStage.Active, OnSiloActiveLicecycleStart, OnSiloActiveLicecycleStop);
            lifecycle.Subscribe(nameof(SiloLifecycleMonitor), ServiceLifecycleStage.Last, OnLastSiloLicecycleStart, OnLastSiloLicecycleStop);
        }

        private Task OnFirstSiloLicecycleStart(CancellationToken token)
        {
            _nodeEventPublisher.PublishAsync(new OrleansFirstSiloLicecycleStartEvent());
            return Task.CompletedTask;
        }
        private Task OnRuntimeInitializeLicecycleStart(CancellationToken token)
        {
            _nodeEventPublisher.PublishAsync(new OrleansRuntimeInitializeLicecycleStartEvent());
            return Task.CompletedTask;
        }
        private Task OnRuntimeServicesLicecycleStart(CancellationToken token)
        {
            _nodeEventPublisher.PublishAsync(new OrleansRuntimeServicesLicecycleStartEvent());
            return Task.CompletedTask;
        }
        private Task OnRuntimeStorageServicesLicecycleStart(CancellationToken token)
        {
            _nodeEventPublisher.PublishAsync(new OrleansRuntimeStorageServicesLicecycleStartEvent());
            return Task.CompletedTask;
        }
        private Task OnRuntimeGrainServicesLicecycleStart(CancellationToken token)
        {
            _nodeEventPublisher.PublishAsync(new OrleansRuntimeGrainServicesLicecycleStartEvent());
            return Task.CompletedTask;
        }
        private Task OnAfterRuntimeGrainServicesLicecycleStart(CancellationToken token)
        {
            _nodeEventPublisher.PublishAsync(new OrleansAfterRuntimeGrainServicesLicecycleStartEvent());
            return Task.CompletedTask;
        }
        private Task OnApplicationServicesLicecycleStart(CancellationToken token)
        {
            _nodeEventPublisher.PublishAsync(new OrleansApplicationServicesLicecycleStartEvent());
            return Task.CompletedTask;
        }
        private Task OnSiloBecomeActiveLicecycleStart(CancellationToken token)
        {
            _nodeEventPublisher.PublishAsync(new OrleansSiloBecomeActiveLicecycleStartEvent());
            return Task.CompletedTask;
        }
        private Task OnSiloActiveLicecycleStart(CancellationToken token)
        {
            _nodeEventPublisher.PublishAsync(new OrleansSiloActiveLicecycleStartEvent());
            return Task.CompletedTask;
        }
        private Task OnLastSiloLicecycleStart(CancellationToken token)
        {
            _nodeEventPublisher.PublishAsync(new OrleansLastSiloLicecycleStartEvent());
            return Task.CompletedTask;
        }

        private Task OnLastSiloLicecycleStop(CancellationToken token)
        {
            _nodeEventPublisher.PublishAsync(new OrleansLastSiloLicecycleStopEvent());
            return Task.CompletedTask;
        }
        private Task OnSiloActiveLicecycleStop(CancellationToken token)
        {
            _nodeEventPublisher.PublishAsync(new OrleansSiloActiveLicecycleStopEvent());
            return Task.CompletedTask;
        }
        private Task OnApplicationServicesLicecycleStop(CancellationToken token)
        {
            _nodeEventPublisher.PublishAsync(new OrleansApplicationServicesLicecycleStopEvent());
            return Task.CompletedTask;
        }
        private Task OnSiloBecomeActiveLicecycleStop(CancellationToken token)
        {
            _nodeEventPublisher.PublishAsync(new OrleansSiloBecomeActiveLicecycleStopEvent());
            return Task.CompletedTask;
        }
        private Task OnAfterRuntimeGrainServicesLicecycleStop(CancellationToken token)
        {
            _nodeEventPublisher.PublishAsync(new OrleansAfterRuntimeGrainServicesLicecycleStopEvent());
            return Task.CompletedTask;
        }
        private Task OnRuntimeGrainServicesLicecycleStop(CancellationToken token)
        {
            _nodeEventPublisher.PublishAsync(new OrleansRuntimeGrainServicesLicecycleStopEvent());
            return Task.CompletedTask;
        }
        private Task OnRuntimeStorageServicesLicecycleStop(CancellationToken token)
        {
            _nodeEventPublisher.PublishAsync(new OrleansRuntimeStorageServicesLicecycleStopEvent());
            return Task.CompletedTask;
        }
        private Task OnRuntimeServicesLicecycleStop(CancellationToken token)
        {
            _nodeEventPublisher.PublishAsync(new OrleansRuntimeServicesLicecycleStopEvent());
            return Task.CompletedTask;
        }
        private Task OnRuntimeInitializeLicecycleStop(CancellationToken token)
        {
            _nodeEventPublisher.PublishAsync(new OrleansRuntimeInitializeLicecycleStopEvent());
            return Task.CompletedTask;
        }
        private Task OnFirstSiloLicecycleStop(CancellationToken token)
        {
            _nodeEventPublisher.PublishAsync(new OrleansFirstSiloLicecycleStopEvent());
            return Task.CompletedTask;
        }
    }
}
