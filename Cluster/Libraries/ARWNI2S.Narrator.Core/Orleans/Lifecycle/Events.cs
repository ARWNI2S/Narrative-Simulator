using ARWNI2S.Node.Core.Events;

namespace ARWNI2S.Engine.Orleans.Lifecycle
{
    internal abstract class OrleansSiloLifecycleEvent
    {
    }


    internal abstract class OrleansSiloLifecycleStartEvent : OrleansSiloLifecycleEvent
    {
    }

    internal abstract class OrleansSiloLifecycleStopEvent : OrleansSiloLifecycleEvent
    {
    }


    internal sealed class OrleansFirstSiloLicecycleStartEvent : OrleansSiloLifecycleStartEvent
    {
    }

    internal sealed class OrleansRuntimeInitializeLicecycleStartEvent : OrleansSiloLifecycleStartEvent
    {
    }

    internal sealed class OrleansRuntimeServicesLicecycleStartEvent : OrleansSiloLifecycleStartEvent
    {
    }

    internal sealed class OrleansRuntimeStorageServicesLicecycleStartEvent : OrleansSiloLifecycleStartEvent
    {
    }

    internal sealed class OrleansRuntimeGrainServicesLicecycleStartEvent : OrleansSiloLifecycleStartEvent
    {
    }

    internal sealed class OrleansAfterRuntimeGrainServicesLicecycleStartEvent : OrleansSiloLifecycleStartEvent
    {
    }

    internal sealed class OrleansApplicationServicesLicecycleStartEvent : OrleansSiloLifecycleStartEvent
    {
    }

    internal sealed class OrleansSiloBecomeActiveLicecycleStartEvent : OrleansSiloLifecycleStartEvent
    {
    }

    internal sealed class OrleansSiloActiveLicecycleStartEvent : OrleansSiloLifecycleStartEvent
    {
    }

    internal sealed class OrleansLastSiloLicecycleStartEvent : OrleansSiloLifecycleStartEvent
    {
    }


    internal sealed class OrleansLastSiloLicecycleStopEvent : OrleansSiloLifecycleStopEvent
    {
    }

    internal sealed class OrleansSiloActiveLicecycleStopEvent : OrleansSiloLifecycleStopEvent
    {
    }

    internal sealed class OrleansApplicationServicesLicecycleStopEvent : OrleansSiloLifecycleStopEvent
    {
    }

    internal sealed class OrleansSiloBecomeActiveLicecycleStopEvent : OrleansSiloLifecycleStopEvent
    {
    }

    internal sealed class OrleansAfterRuntimeGrainServicesLicecycleStopEvent : OrleansSiloLifecycleStopEvent
    {
    }

    internal sealed class OrleansRuntimeGrainServicesLicecycleStopEvent : OrleansSiloLifecycleStopEvent
    {
    }

    internal sealed class OrleansRuntimeStorageServicesLicecycleStopEvent : OrleansSiloLifecycleStopEvent
    {
    }

    internal sealed class OrleansRuntimeServicesLicecycleStopEvent : OrleansSiloLifecycleStopEvent
    {
    }

    internal sealed class OrleansRuntimeInitializeLicecycleStopEvent : OrleansSiloLifecycleStopEvent
    {
    }

    internal sealed class OrleansFirstSiloLicecycleStopEvent : OrleansSiloLifecycleStopEvent
    {
    }
}
