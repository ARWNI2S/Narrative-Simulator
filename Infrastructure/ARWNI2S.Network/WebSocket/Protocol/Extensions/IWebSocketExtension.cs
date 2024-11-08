namespace ARWNI2S.Engine.Network.WebSocket.Protocol.Extensions
{
    /// <summary>
    /// WebSocket Extensions
    /// https://tools.ietf.org/html/rfc6455#section-9
    /// </summary>
    public interface IWebSocketExtension
    {
        string Name { get; }

        void Encode(WebSocketPackage package);

        void Decode(WebSocketPackage package);
    }
}
