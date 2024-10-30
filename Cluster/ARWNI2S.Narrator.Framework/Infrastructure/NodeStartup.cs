using ARWNI2S.Infrastructure;
using ARWNI2S.Node.Core.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ARWNI2S.Infrastructure.Extensions.Configuration;
using ARWNI2S.Runtime.Infrastructure.Extensions;

namespace ARWNI2S.Narrator.Framework.Infrastructure
{
    internal class NodeStartup : INI2SStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //services.UseSimulationClock<NarratorClock>();
            services.UseSimulation<NarratorEngine>();

            services.AddFromExisting<ILifecycleParticipant<ISiloLifecycle>, NarratorEngine>();
        }

        public void Configure(IHost application)
        {

        }

        public int Order => 2000;
    }
}
