namespace ARWNI2S.Engine.Network.Connections
{
    public interface IConnectionStreamInitializersFactory
    {
        IEnumerable<IConnectionStreamInitializer> Create(ListenOptions listenOptions);
    }
}