using ARWNI2S.Infrastructure.Network.Connection;

namespace ARWNI2S.Engine.Network.Connection
{
    public delegate ValueTask NewConnectionAcceptHandler(ListenOptions listenOptions, IConnection connection);

    public interface IConnectionListener
    {
        ListenOptions Options { get; }

        bool Start();

        event NewConnectionAcceptHandler NewConnectionAccept;

        Task StopAsync();

        bool IsRunning { get; }

        IConnectionFactory ConnectionFactory { get; }
    }
}