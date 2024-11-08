using ARWNI2S.Engine.Network.Connection;
using System.Text;

namespace ARWNI2S.Engine.Network
{
    public class ServerOptions : ConnectionOptions
    {
        public string Name { get; set; }

        public List<ListenOptions> Listeners { get; set; }

        public Encoding DefaultTextEncoding { get; set; }

        public int ClearIdleSessionInterval { get; set; } = 120;

        public int IdleSessionTimeOut { get; set; } = 300;

        /// <summary>
        /// In seconds.
        /// </summary>
        public int PackageHandlingTimeOut { get; set; } = 30;

        public bool EnableProxyProtocol { get; set; }
    }
}