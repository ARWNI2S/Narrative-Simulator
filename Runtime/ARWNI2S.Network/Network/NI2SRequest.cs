namespace ARWNI2S.Engine.Network.Network
{
    public class HeaderNames
    {
        public static readonly string Host;
    }

    public class NI2SRequest
    {
        public readonly Dictionary<string, string> Headers;

        public bool IsSecured { get; internal set; }
        public object PathBase { get; internal set; }
        public string Path { get; internal set; }
    }
}