using ARWNI2S.Engine.Network.Session;
using ARWNI2S.Infrastructure.Network;

namespace ARWNI2S.Engine.Network
{
    public interface IPackageHandlingScheduler<TPackageInfo>
    {
        void Initialize(IPackageHandler<TPackageInfo> packageHandler, Func<IAppSession, PackageHandlingException<TPackageInfo>, ValueTask<bool>> errorHandler);

        ValueTask HandlePackage(IAppSession session, TPackageInfo package, CancellationToken cancellationToken);
    }
}