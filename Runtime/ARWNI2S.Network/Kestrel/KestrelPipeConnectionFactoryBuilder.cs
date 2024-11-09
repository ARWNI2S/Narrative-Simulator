using System.Net.Sockets;

namespace ARWNI2S.Engine.Network.Kestrel;

public class KestrelPipeConnectionFactoryBuilder : IConnectionFactoryBuilder
{
    private readonly SocketConnectionContextFactory _socketConnectionContextFactory;

    private readonly Action<Socket> _socketOptionsSetter;

    public KestrelPipeConnectionFactoryBuilder(SocketConnectionContextFactory socketConnectionContextFactory, SocketOptionsSetter socketOptionsSetter)
    {
        _socketConnectionContextFactory = socketConnectionContextFactory;
        _socketOptionsSetter = socketOptionsSetter.Setter;
    }

    public IConnectionFactory Build(ListenOptions listenOptions, ConnectionOptions connectionOptions)
    {
        return new KestrelPipeConnectionFactory(_socketConnectionContextFactory, listenOptions, connectionOptions, _socketOptionsSetter);
    }
}