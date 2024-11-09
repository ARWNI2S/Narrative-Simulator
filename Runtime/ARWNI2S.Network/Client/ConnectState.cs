using ARWNI2S.Engine.Network.Configuration.Options;
using ARWNI2S.Engine.Network.Connection;
using ARWNI2S.Engine.Network.Connection.Pipes;
using ARWNI2S.Infrastructure.Network.Connection;
using System.Net.Sockets;

namespace ARWNI2S.Engine.Network.Client
{
    public class ConnectState
    {
        public ConnectState()
        {

        }

        private ConnectState(bool cancelled)
        {
            Cancelled = cancelled;
        }

        public bool Result { get; set; }

        public bool Cancelled { get; private set; }

        public Exception Exception { get; set; }

        public Socket Socket { get; set; }

        public Stream Stream { get; set; }

        public static readonly ConnectState CancelledState = new ConnectState(false);

        public IConnection CreateConnection(ConnectionOptions connectionOptions)
        {
            var stream = Stream;
            var socket = Socket;

            if (stream != null)
            {
                return new StreamPipeConnection(stream, socket.RemoteEndPoint, socket.LocalEndPoint, connectionOptions);
            }
            else
            {
                return new TcpPipeConnection(socket, connectionOptions);
            }
        }
    }
}