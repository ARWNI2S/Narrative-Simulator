using ARWNI2S.Engine.Network.Connections;
using ARWNI2S.Engine.Network.Middleware;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;


namespace ARWNI2S.Engine.Network.Host
{
    public static class HostBuilderExtensions
    {
        public static INodeServerHostBuilder AsNodeServerBuilder(this IHostBuilder hostBuilder)
        {
            return hostBuilder as INodeServerHostBuilder;
        }

        public static INodeServerHostBuilder UseMiddleware<TMiddleware>(this INodeServerHostBuilder builder)
            where TMiddleware : class, IMiddleware
        {
            return builder.ConfigureServices((ctx, services) =>
            {
                services.TryAddEnumerable(ServiceDescriptor.Singleton<IMiddleware, TMiddleware>());
            }).AsNodeServerBuilder();
        }

        public static INodeServerHostBuilder UseMiddleware<TMiddleware>(this INodeServerHostBuilder builder, Func<IServiceProvider, TMiddleware> implementationFactory)
            where TMiddleware : class, IMiddleware
        {
            return builder.ConfigureServices((ctx, services) =>
            {
                services.TryAddEnumerable(ServiceDescriptor.Singleton<IMiddleware, TMiddleware>(implementationFactory));
            }).AsNodeServerBuilder();
        }
        public static INodeServerHostBuilder UseTcpConnectionListenerFactory<TConnectionListenerFactory>(this INodeServerHostBuilder builder)
            where TConnectionListenerFactory : class, IConnectionListenerFactory
        {
            return builder.ConfigureServices((ctx, services) =>
            {
                services.AddSingleton<IConnectionListenerFactory, TConnectionListenerFactory>();
            }).AsNodeServerBuilder();
        }
    }
}
