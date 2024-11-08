using ARWNI2S.Engine.Network.Session;
using ARWNI2S.Infrastructure.Extensions;
using ARWNI2S.Infrastructure.Network.Connection;

namespace ARWNI2S.Engine.Network
{
    public abstract class PackageHandlingSchedulerBase<TPackageInfo> : IPackageHandlingScheduler<TPackageInfo>
    {
        public IPackageHandler<TPackageInfo> PackageHandler { get; private set; }

        public Func<INodeSession, PackageHandlingException<TPackageInfo>, ValueTask<bool>> ErrorHandler { get; private set; }

        public abstract ValueTask HandlePackage(INodeSession session, TPackageInfo package, CancellationToken cancellationToken);

        public virtual void Initialize(IPackageHandler<TPackageInfo> packageHandler, Func<INodeSession, PackageHandlingException<TPackageInfo>, ValueTask<bool>> errorHandler)
        {
            PackageHandler = packageHandler;
            ErrorHandler = errorHandler;
        }

        protected async ValueTask HandlePackageInternal(INodeSession session, TPackageInfo package, CancellationToken cancellationToken)
        {
            var packageHandler = PackageHandler;
            var errorHandler = ErrorHandler;

            try
            {
                if (packageHandler != null)
                    await packageHandler.Handle(session, package, cancellationToken);
            }
            catch (Exception e)
            {
                var toClose = await errorHandler(session, new PackageHandlingException<TPackageInfo>($"Session {session.SessionID} got an error when handle a package.", package, e));

                if (toClose)
                {
                    session.CloseAsync(CloseReason.ApplicationError).DoNotAwait();
                }
            }
        }
    }
}