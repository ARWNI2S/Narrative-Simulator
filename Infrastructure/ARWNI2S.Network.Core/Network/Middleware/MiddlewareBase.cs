using ARWNI2S.Engine.Network.Session;

namespace ARWNI2S.Engine.Network.Middleware
{
    public abstract class MiddlewareBase : IMiddleware
    {
        public int Order { get; protected set; } = 0;

        public virtual void Start(IServer server)
        {

        }

        public virtual void Shutdown(IServer server)
        {

        }

        public virtual ValueTask<bool> RegisterSession(IAppSession session)
        {
            return new ValueTask<bool>(true);
        }

        public virtual ValueTask<bool> UnRegisterSession(IAppSession session)
        {
            return new ValueTask<bool>(true);
        }
    }
}