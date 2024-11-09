using ARWNI2S.Engine.Network.Session;

namespace ARWNI2S.Engine.Network
{
    public class DelegatePackageHandler<TReceivePackageInfo> : IPackageHandler<TReceivePackageInfo>
    {
        Func<INodeSession, TReceivePackageInfo, CancellationToken, ValueTask> _func;

        public DelegatePackageHandler(Func<INodeSession, TReceivePackageInfo, ValueTask> func)
        {
            _func = (session, package, cancellationToken) => func(session, package);
        }

        public DelegatePackageHandler(Func<INodeSession, TReceivePackageInfo, CancellationToken, ValueTask> func)
        {
            _func = func;
        }

        public async ValueTask Handle(INodeSession session, TReceivePackageInfo package, CancellationToken cancellationToken)
        {
            await _func(session, package, cancellationToken);
        }
    }
}