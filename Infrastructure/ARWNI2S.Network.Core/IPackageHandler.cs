using ARWNI2S.Engine.Network.Session;

namespace ARWNI2S.Engine.Network
{
    public interface IPackageHandler<TReceivePackageInfo>
    {
        ValueTask Handle(INodeSession session, TReceivePackageInfo package, CancellationToken cancellationToken);
    }
}