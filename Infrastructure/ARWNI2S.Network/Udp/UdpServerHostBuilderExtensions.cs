using ARWNI2S.Engine.Network.Connections;
using ARWNI2S.Engine.Network.Host;
using ARWNI2S.Engine.Network.Middleware;
using ARWNI2S.Engine.Network.Session;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;


namespace ARWNI2S.Engine.Network.Udp
{
    public static class UdpServerHostBuilderExtensions
    {
        public static INodeServerHostBuilder UseUdp(this INodeServerHostBuilder hostBuilder)
        {
            return (hostBuilder.ConfigureServices((context, services) =>
            {
                services.AddSingleton<IConnectionListenerFactory, UdpConnectionListenerFactory>();
                services.AddSingleton<IConnectionFactoryBuilder, UdpConnectionFactoryBuilder>();
            }) as INodeServerHostBuilder)
            .ConfigureSupplementServices((context, services) =>
            {
                if (!services.Any(s => s.ServiceType == typeof(IUdpSessionIdentifierProvider)))
                {
                    services.AddSingleton<IUdpSessionIdentifierProvider, IPAddressUdpSessionIdentifierProvider>();
                }

                if (!services.Any(s => s.ServiceType == typeof(IAsyncSessionContainer)))
                {
                    services.TryAddEnumerable(ServiceDescriptor.Singleton<IMiddleware, InProcSessionContainerMiddleware>(s => s.GetRequiredService<InProcSessionContainerMiddleware>()));
                    services.AddSingleton<InProcSessionContainerMiddleware>();
                    services.AddSingleton<ISessionContainer>((s) => s.GetRequiredService<InProcSessionContainerMiddleware>());
                    services.AddSingleton<IAsyncSessionContainer>((s) => s.GetRequiredService<ISessionContainer>().ToAsyncSessionContainer());
                }
            });
        }

        public static INodeServerHostBuilder<TReceivePackage> UseUdp<TReceivePackage>(this INodeServerHostBuilder<TReceivePackage> hostBuilder)
        {
            return (hostBuilder as INodeServerHostBuilder).UseUdp() as INodeServerHostBuilder<TReceivePackage>;
        }
    }
}