using System.Net;
using System.Text;
using ARWNI2S.Engine.Network.Connection;
using ARWNI2S.Engine.Network.Protocol;
using ARWNI2S.Engine.Network.Protocol.Filters;
using ARWNI2S.Infrastructure.Network.Connection;

namespace ARWNI2S.Engine.Network.Client.Proxy
{
    public class HttpConnector : ProxyConnectorBase
    {
        private const string _requestTemplate = "CONNECT {0}:{1} HTTP/1.1\r\nHost: {0}:{1}\r\nProxy-Connection: Keep-Alive\r\n";
        private const string _responsePrefix = "HTTP/1.";
        private const char _space = ' ';
        private string _username;
        private string _password;

        public HttpConnector(EndPoint proxyEndPoint)
            : base(proxyEndPoint)
        {

        }

        public HttpConnector(EndPoint proxyEndPoint, string username, string password)
            : this(proxyEndPoint)
        {
            _username = username;
            _password = password;
        }

        protected override async ValueTask<ConnectState> ConnectProxyAsync(EndPoint remoteEndPoint, ConnectState state, CancellationToken cancellationToken)
        {
            var encoding = Encoding.ASCII;
            var request = string.Empty;
            var connection = state.CreateConnection(new ConnectionOptions { ReadAsDemand = true });

            var packStream = connection.GetPackageStream(new LinePipelineFilter(encoding));

            if (remoteEndPoint is DnsEndPoint dnsEndPoint)
            {
                request = string.Format(_requestTemplate, dnsEndPoint.Host, dnsEndPoint.Port);
            }
            else if (remoteEndPoint is IPEndPoint ipEndPoint)
            {
                request = string.Format(_requestTemplate, ipEndPoint.Address, ipEndPoint.Port);
            }
            else
            {
                return new ConnectState
                {
                    Result = false,
                    Exception = new Exception($"The endpint type {remoteEndPoint.GetType().ToString()} is not supported.")
                };
            }

            // send request
            await connection.SendAsync((writer) =>
            {
                writer.Write(request, encoding);

                if (!string.IsNullOrEmpty(_username) || !string.IsNullOrEmpty(_password))
                {
                    writer.Write("Proxy-Authorization: Basic ", encoding);
                    writer.Write(Convert.ToBase64String(encoding.GetBytes($"{_username}:{_password}")), encoding);
                    writer.Write("\r\n\r\n", encoding);
                }
                else
                {
                    writer.Write("\r\n", encoding);
                }
            });

            var p = await packStream.ReceiveAsync();

            if (!HandleResponse(p, out string errorMessage))
            {
                await connection.CloseAsync(CloseReason.ProtocolError);

                return new ConnectState
                {
                    Result = false,
                    Exception = new Exception(errorMessage)
                };
            }

            await connection.DetachAsync();
            return state;
        }

        private bool HandleResponse(TextPackageInfo p, out string message)
        {
            message = string.Empty;

            if (p == null)
                return false;

            var pos = p.Text.IndexOf(_space);

            // validating response
            if (!p.Text.StartsWith(_responsePrefix, StringComparison.OrdinalIgnoreCase) || pos <= 0)
            {
                message = "Invalid response";
                return false;
            }

            if (!int.TryParse(p.Text.AsSpan().Slice(pos + 1, 3), out var statusCode))
            {
                message = "Invalid response";
                return false;
            }

            if (statusCode < 200 || statusCode > 299)
            {
                message = $"Invalid status code {statusCode}";
                return false;
            }

            return true;
        }
    }
}