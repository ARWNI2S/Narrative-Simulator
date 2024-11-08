using ARWNI2S.Engine.Network.Connection;
using Microsoft.Extensions.Logging;

namespace ARWNI2S.Engine.Network.Connections
{
    public interface IConnectionListenerFactory
    {
        IConnectionListener CreateConnectionListener(ListenOptions options, ConnectionOptions connectionOptions, ILoggerFactory loggerFactory);
    }
}