using System.Net;
using System.Net.Sockets;

namespace ARWNI2S.Engine.Network.Client
{
    public class SocketConnector : ConnectorBase
    {
        public IPEndPoint LocalEndPoint { get; private set; }

        public SocketConnector()
            : base() { }

        public SocketConnector(IPEndPoint localEndPoint)
            : base() { LocalEndPoint = localEndPoint; }

        protected override async ValueTask<ConnectState> ConnectAsync(EndPoint remoteEndPoint, ConnectState state, CancellationToken cancellationToken)
        {
            var addressFamily = remoteEndPoint.AddressFamily;

            if (addressFamily == AddressFamily.Unspecified)
                addressFamily = AddressFamily.InterNetworkV6;

            var socket = new Socket(addressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                var localEndPoint = LocalEndPoint;

                if (localEndPoint != null)
                {
                    socket.ExclusiveAddressUse = false;
                    socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
                    socket.Bind(localEndPoint);
                }
                await socket.ConnectAsync(remoteEndPoint, cancellationToken);
            }
            catch (Exception e)
            {
                return new ConnectState
                {
                    Result = false,
                    Exception = e
                };
            }

            return new ConnectState
            {
                Result = true,
                Socket = socket
            };
        }
    }
}