using ARWNI2S.Infrastructure.Network.Connection;

namespace ARWNI2S.Engine
{
    public interface INetworkContext
    {
        IServiceProvider ContextServices { get; }

        NI2SRequest Request { get; }

        IConnection Connection { get; }

        NI2SResponse Response { get; }

        Dictionary<string, object> Items { get; }
    }
}