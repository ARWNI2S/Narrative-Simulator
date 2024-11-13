namespace ARWNI2S.Engine.Tasks
{
    internal class NiisFrame
    {
        public object Protocol { get; internal set; }
        public object Method { get; internal set; }
        public string ContentType { get; internal set; }
        public int? ContentLength { get; internal set; }
        public object Scheme { get; internal set; }
        public object Host { get; internal set; }
        public object PathBase { get; internal set; }
        public object Path { get; internal set; }
        public object QueryString { get; internal set; }
    }
}
