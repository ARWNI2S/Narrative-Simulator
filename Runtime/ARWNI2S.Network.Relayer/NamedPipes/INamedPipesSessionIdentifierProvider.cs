using System.Net;

namespace ARWNI2S.Engine.Network.NamedPipes
{
    public interface INamedPipesSessionIdentifierProvider
    {
        string GetSessionIdentifier(IPEndPoint remoteEndPoint, ArraySegment<byte> data);
    }
}