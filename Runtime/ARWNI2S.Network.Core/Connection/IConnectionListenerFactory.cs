using ARWNI2S.Engine.Network.Configuration.Options;
using Microsoft.Extensions.Logging;

namespace ARWNI2S.Engine.Network.Connection
{
    public interface IConnectionListenerFactory
    {
        IConnectionListener CreateConnectionListener(ListenOptions options, ConnectionOptions connectionOptions, ILoggerFactory loggerFactory);
    }
}