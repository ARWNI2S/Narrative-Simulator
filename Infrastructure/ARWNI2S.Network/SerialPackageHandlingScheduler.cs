using ARWNI2S.Engine.Network.Session;

namespace ARWNI2S.Engine.Network
{
    public class SerialPackageHandlingScheduler<TPackageInfo> : PackageHandlingSchedulerBase<TPackageInfo>
    {
        public override async ValueTask HandlePackage(INodeSession session, TPackageInfo package, CancellationToken cancellationToken)
        {
            await HandlePackageInternal(session, package, cancellationToken);
        }
    }
}