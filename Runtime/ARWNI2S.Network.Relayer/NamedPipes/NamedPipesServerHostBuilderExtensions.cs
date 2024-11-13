using ARWNI2S.Engine.Network.Connection;
using ARWNI2S.Engine.Network.Middleware;
using ARWNI2S.Engine.Network.Session;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;


namespace ARWNI2S.Engine.Network.NamedPipes
{
    public static class NamedPipesServerHostBuilderExtensions
    {
        public static INodeServerHostBuilder UseNamedPipes(this INodeServerHostBuilder hostBuilder)
        {
            return (hostBuilder.ConfigureServices((context, services) =>
            {
                services.AddSingleton<IConnectionListenerFactory, NamedPipesConnectionListenerFactory>();
                services.AddSingleton<IConnectionFactoryBuilder, NamedPipesConnectionFactoryBuilder>();
            }) as INodeServerHostBuilder)
            .ConfigureSupplementServices((context, services) =>
            {
                if (!services.Any(s => s.ServiceType == typeof(INamedPipesSessionIdentifierProvider)))
                {
                    services.AddSingleton<INamedPipesSessionIdentifierProvider, NamedPipesSessionIdentifierProvider>();
                }

                if (!services.Any(s => s.ServiceType == typeof(IAsyncSessionContainer)))
                {
                    services.TryAddEnumerable(ServiceDescriptor.Singleton<IMiddleware, InProcSessionContainerMiddleware>(s => s.GetRequiredService<InProcSessionContainerMiddleware>()));
                    services.AddSingleton<InProcSessionContainerMiddleware>();
                    services.AddSingleton<ISessionContainer>((s) => s.GetRequiredService<InProcSessionContainerMiddleware>());
                    services.AddSingleton((s) => s.GetRequiredService<ISessionContainer>().ToAsyncSessionContainer());
                }
            });
        }

        public static INodeServerHostBuilder<TReceivePackage> UseNamedPipes<TReceivePackage>(this INodeServerHostBuilder<TReceivePackage> hostBuilder)
        {
            return (hostBuilder as INodeServerHostBuilder).UseNamedPipes() as INodeServerHostBuilder<TReceivePackage>;
        }
    }
}