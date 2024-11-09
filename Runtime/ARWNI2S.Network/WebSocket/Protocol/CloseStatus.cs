namespace ARWNI2S.Engine.Network.WebSocket.Protocol
{
    public class CloseStatus
    {
        public CloseReason Reason { get; set; }

        public string ReasonText { get; set; }

        public bool RemoteInitiated { get; set; }
    }
}
