using System.Net;

namespace ARWNI2S.Engine.Network.NamedPipes
{
    class NamedPipesSessionIdentifierProvider : INamedPipesSessionIdentifierProvider
    {
        public string GetSessionIdentifier(IPEndPoint remoteEndPoint, ArraySegment<byte> data)
        {
            return remoteEndPoint.Address.ToString() + ":" + remoteEndPoint.Port;
        }
    }
}