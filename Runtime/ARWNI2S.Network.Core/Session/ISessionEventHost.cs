using ARWNI2S.Infrastructure.Network.Connection;

namespace ARWNI2S.Engine.Network.Session
{
    public interface ISessionEventHost
    {
        ValueTask HandleSessionConnectedEvent(INodeSession session);

        ValueTask HandleSessionClosedEvent(INodeSession session, CloseReason reason);
    }
}