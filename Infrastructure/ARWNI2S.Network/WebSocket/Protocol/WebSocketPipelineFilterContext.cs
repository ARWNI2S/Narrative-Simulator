using ARWNI2S.Engine.Network.WebSocket.Protocol.Extensions;

namespace ARWNI2S.Engine.Network.WebSocket.Protocol
{
    public class WebSocketPipelineFilterContext
    {
        public IReadOnlyList<IWebSocketExtension> Extensions { get; set; }
    }
}
