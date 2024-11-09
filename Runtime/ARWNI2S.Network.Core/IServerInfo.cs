using ARWNI2S.Engine.Network.Configuration.Options;

namespace ARWNI2S.Engine.Network
{
    public interface IServerInfo
    {
        string Name { get; }

        ServerOptions Options { get; }

        object DataContext { get; set; }

        int SessionCount { get; }

        IServiceProvider ServiceProvider { get; }

        ServerState State { get; }
    }
}