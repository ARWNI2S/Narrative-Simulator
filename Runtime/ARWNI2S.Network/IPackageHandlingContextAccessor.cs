using ARWNI2S.Engine.Network.Session;

namespace ARWNI2S.Engine.Network
{
    public interface IPackageHandlingContextAccessor<TPackageInfo>
    {
        PackageHandlingContext<INodeSession, TPackageInfo> PackageHandlingContext { get; set; }
    }
}
