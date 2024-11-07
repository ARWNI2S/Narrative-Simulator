using ARWNI2S.Infrastructure.Network.Connection;

namespace ARWNI2S.Engine.Network.Session
{
    public interface ISessionEventHost
    {
        ValueTask HandleSessionConnectedEvent(IAppSession session);

        ValueTask HandleSessionClosedEvent(IAppSession session, CloseReason reason);
    }
}