namespace ARWNI2S.Engine.Network.Kestrel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public static class KestrelPipeConnectionExtensions
{
    public static ISuperSocketHostBuilder UseKestrelPipeConnection(this ISuperSocketHostBuilder hostBuilder, SocketConnectionFactoryOptions options = null)
    {
        hostBuilder.ConfigureServices((ctx, services) =>
            {
                services.AddSingleton(serviceProvider => new SocketConnectionContextFactory(options ?? new SocketConnectionFactoryOptions(), serviceProvider.GetService<ILoggerFactory>().CreateLogger<SocketConnectionContextFactory>()));
                services.AddSingleton<IConnectionFactoryBuilder, KestrelPipeConnectionFactoryBuilder>();
            });

        return hostBuilder;
    }
}
