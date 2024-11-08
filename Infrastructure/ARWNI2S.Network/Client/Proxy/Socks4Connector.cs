using System.Net;

namespace ARWNI2S.Engine.Network.Client.Proxy
{
    public class Socks4Connector : ConnectorBase
    {
        protected override ValueTask<ConnectState> ConnectAsync(EndPoint remoteEndPoint, ConnectState state, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}