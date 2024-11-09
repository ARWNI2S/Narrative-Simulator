namespace ARWNI2S.Engine.Network.WebSocket.Protocol
{
    public interface IWebSocketFrameHeader
    {
        bool FIN { get; }

        bool RSV1 { get; }

        bool RSV2 { get; }

        bool RSV3 { get; }
    }
}
