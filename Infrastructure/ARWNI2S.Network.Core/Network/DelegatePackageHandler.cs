using ARWNI2S.Engine.Network.Session;

namespace ARWNI2S.Engine.Network
{
    public class DelegatePackageHandler<TReceivePackageInfo> : IPackageHandler<TReceivePackageInfo>
    {
        Func<IAppSession, TReceivePackageInfo, CancellationToken, ValueTask> _func;

        public DelegatePackageHandler(Func<IAppSession, TReceivePackageInfo, ValueTask> func)
        {
            _func = (session, package, cancellationToken) => func(session, package);
        }

        public DelegatePackageHandler(Func<IAppSession, TReceivePackageInfo, CancellationToken, ValueTask> func)
        {
            _func = func;
        }

        public async ValueTask Handle(IAppSession session, TReceivePackageInfo package, CancellationToken cancellationToken)
        {
            await _func(session, package, cancellationToken);
        }
    }
}