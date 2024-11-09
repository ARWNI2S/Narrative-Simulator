using ARWNI2S.Engine.Network.Session;
using ARWNI2S.Infrastructure.Extensions;

namespace ARWNI2S.Engine.Network
{
    public class ConcurrentPackageHandlingScheduler<TPackageInfo> : PackageHandlingSchedulerBase<TPackageInfo>
    {
        public override ValueTask HandlePackage(INodeSession session, TPackageInfo package, CancellationToken cancellationToken)
        {
            HandlePackageInternal(session, package, cancellationToken).DoNotAwait();
            return new ValueTask();
        }
    }
}