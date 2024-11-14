using ARWNI2S.Engine.Orleans.Lifecycle;
using ARWNI2S.Infrastructure;
using ARWNI2S.Infrastructure.Engine.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Configuration.Internal;

namespace ARWNI2S.Narrator.Framework.Infrastructure
{
    internal class NarratorNodeStartup : INI2SStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //services.UseSimulationClock<NarratorClock>();
            //services.UseSimulation<NarratorEngine>();

            //services.AddFromExisting<ILifecycleParticipant<ISiloLifecycle>, NarratorEngine>();

            services.AddSingleton<SiloLifecycleMonitor>();
            services.AddFromExisting<ILifecycleParticipant<ISiloLifecycle>, SiloLifecycleMonitor>();

        }

        public void Configure(IEngineBuilder engine)
        {

        }

        public int Order => 2000;
    }
}
