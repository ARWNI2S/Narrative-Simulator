using ARWNI2S.Engine.Network.Connection;
using ARWNI2S.Engine.Network.Session;
using ARWNI2S.Infrastructure.Logging;
using Microsoft.Extensions.Hosting;

namespace ARWNI2S.Engine.Network
{
    public interface ISuperSocketHostedService : IHostedService, IServer, IConnectionRegister, ILoggerAccessor, ISessionEventHost
    {
    }
}