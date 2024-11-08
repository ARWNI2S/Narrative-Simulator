using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ARWNI2S.Engine.Network.Host
{
    public interface IServerHostBuilderAdapter
    {
        void ConfigureServer(HostBuilderContext context, IServiceCollection hostServices);

        void ConfigureServiceProvider(IServiceProvider hostServiceProvider);
    }
}