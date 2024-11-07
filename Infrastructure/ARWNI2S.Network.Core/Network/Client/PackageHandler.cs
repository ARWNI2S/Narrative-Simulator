namespace ARWNI2S.Engine.Network.Client
{
    public delegate ValueTask PackageHandler<TReceivePackage>(NodeClient<TReceivePackage> sender, TReceivePackage package)
        where TReceivePackage : class;
}