using System.Net;

namespace ARWNI2S.Engine.Network.Udp
{
    public interface IUdpSessionIdentifierProvider
    {
        string GetSessionIdentifier(IPEndPoint remoteEndPoint, ArraySegment<byte> data);
    }
}