using ARWNI2S.Engine.Network.Session;

namespace ARWNI2S.Engine.Network.Middleware
{
    public interface IMiddleware
    {
        int Order { get; }

        void Start(IServer server);

        void Shutdown(IServer server);

        ValueTask<bool> RegisterSession(INodeSession session);

        ValueTask<bool> UnRegisterSession(INodeSession session);
    }
}