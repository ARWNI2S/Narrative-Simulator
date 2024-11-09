using ARWNI2S.Engine.Network.Host;
using Microsoft.Extensions.DependencyInjection;

namespace ARWNI2S.Engine.Network.Session
{
    public static class InProcSessionContainerMiddlewareExtensions
    {
        public static INodeServerHostBuilder UseInProcSessionContainer(this INodeServerHostBuilder builder)
        {
            return builder
                .UseMiddleware(s => s.GetRequiredService<InProcSessionContainerMiddleware>())
                .ConfigureServices((ctx, services) =>
                {
                    services.AddSingleton<InProcSessionContainerMiddleware>();
                    services.AddSingleton<ISessionContainer>((s) => s.GetRequiredService<InProcSessionContainerMiddleware>());
                    services.AddSingleton((s) => s.GetRequiredService<ISessionContainer>().ToAsyncSessionContainer());
                }) as INodeServerHostBuilder;
        }
    }
}
