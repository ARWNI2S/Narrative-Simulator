using ARWNI2S.Engine.Network.Session;
using ARWNI2S.Infrastructure.Network.Connection;

namespace ARWNI2S.Engine.Network
{
    public class SessionHandlers
    {
        public Func<INodeSession, ValueTask> Connected { get; set; }

        public Func<INodeSession, CloseEventArgs, ValueTask> Closed { get; set; }
    }
}