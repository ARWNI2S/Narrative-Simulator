using ARWNI2S.Infrastructure;
using ARWNI2S.Infrastructure.Engine.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ARWNI2S.Narrator.Framework.Infrastructure
{
    internal class NodeStartup : INodeStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //services.UseSimulationClock<NarratorClock>();
            //services.UseSimulation<NarratorEngine>();

            //services.AddFromExisting<ILifecycleParticipant<ISiloLifecycle>, NarratorEngine>();
        }

        public void Configure(IEngineBuilder engine)
        {

        }

        public int Order => 2000;
    }
}
