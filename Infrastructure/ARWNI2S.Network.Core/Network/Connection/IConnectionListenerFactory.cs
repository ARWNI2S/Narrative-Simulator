using ARWNI2S.Infrastructure.Network.Connection;
using Microsoft.Extensions.Logging;

namespace ARWNI2S.Engine.Network.Connection
{
    public interface IConnectionListenerFactory
    {
        IConnectionListener CreateConnectionListener(ListenOptions options, ConnectionOptions connectionOptions, ILoggerFactory loggerFactory);
    }
}