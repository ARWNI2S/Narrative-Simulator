using System.Net.Sockets;

namespace ARWNI2S.Engine.Network.Connections
{
    public class SocketOptionsSetter
    {
        public Action<Socket> Setter { get; private set; }

        public SocketOptionsSetter(Action<Socket> setter)
        {
            Setter = setter;
        }
    }
}