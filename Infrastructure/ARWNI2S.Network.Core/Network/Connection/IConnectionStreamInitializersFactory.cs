namespace ARWNI2S.Engine.Network.Connection
{
    public interface IConnectionStreamInitializersFactory
    {
        IEnumerable<IConnectionStreamInitializer> Create(ListenOptions listenOptions);
    }
}