using ARWNI2S.Engine.Network.Session;

namespace ARWNI2S.Engine.Network
{
    public interface IPackageHandlingScheduler<TPackageInfo>
    {
        void Initialize(IPackageHandler<TPackageInfo> packageHandler, Func<INodeSession, PackageHandlingException<TPackageInfo>, ValueTask<bool>> errorHandler);

        ValueTask HandlePackage(INodeSession session, TPackageInfo package, CancellationToken cancellationToken);
    }
}