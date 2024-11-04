using ARWNI2S.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ARWNI2S.Narrator.Framework.Infrastructure
{
    internal class NodeStartup : INI2SStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //services.UseSimulationClock<NarratorClock>();
            //services.UseSimulation<NarratorEngine>();

            //services.AddFromExisting<ILifecycleParticipant<ISiloLifecycle>, NarratorEngine>();
        }

        public void Configure(IHost application)
        {

        }

        public int Order => 2000;
    }
}
