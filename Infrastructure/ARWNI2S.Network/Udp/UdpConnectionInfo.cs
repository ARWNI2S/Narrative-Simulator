using ARWNI2S.Engine.Network.Connection;
using System.Net;
using System.Net.Sockets;

namespace ARWNI2S.Engine.Network.Udp
{
    internal struct UdpConnectionInfo
    {
        public Socket Socket { get; set; }

        public ConnectionOptions ConnectionOptions { get; set; }

        public string SessionIdentifier { get; set; }

        public IPEndPoint RemoteEndPoint { get; set; }
    }
}