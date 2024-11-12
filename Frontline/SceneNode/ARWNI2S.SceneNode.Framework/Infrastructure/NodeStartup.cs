using ARWNI2S.Infrastructure;
using ARWNI2S.Infrastructure.Engine.Builder;
using ARWNI2S.SceneNode.Framework.CrossProcess;
using ARWNI2S.SceneNode.Framework.GameEngine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ARWNI2S.SceneNode.Framework.Infrastructure
{
    internal class NodeStartup : INodeStartup
    {
        public int Order => 2000;

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Game server hosting
            services.AddSingleton<IGameServerHostManager, GameServerHostManager>();

            // Cross process
            services.AddHostedService<NamedPipeServer>();


        }

        public void Configure(IEngineBuilder engine)
        {

        }

    }
}
