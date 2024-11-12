using ARWNI2S.Engine.Network.Connection;
using ARWNI2S.Engine.Network.Session;
using ARWNI2S.Infrastructure.Logging;

namespace ARWNI2S.Engine.Network.Relayer
{
    public interface IRelayServer : IServer, IConnectionRegister, ILoggerAccessor, ISessionEventHost
    {
    }
}
