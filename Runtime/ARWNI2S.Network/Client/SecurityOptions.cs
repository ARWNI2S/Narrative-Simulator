using System.Net;
using System.Net.Security;

namespace ARWNI2S.Engine.Network.Client
{
    public class SecurityOptions : SslClientAuthenticationOptions
    {
        public NetworkCredential Credential { get; set; }
    }
}