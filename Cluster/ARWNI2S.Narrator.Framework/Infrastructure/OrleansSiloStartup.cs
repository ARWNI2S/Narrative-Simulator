using ARWNI2S.Infrastructure;
using ARWNI2S.Infrastructure.Engine.Builder;
using ARWNI2S.Narrator.Framework.Hosting.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ARWNI2S.Narrator.Framework.Infrastructure
{
    public class OrleansSiloStartup : INI2SStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {

            services.AddOrleans(siloBuilder => siloBuilder.ConfigureNI2S());

            //services.AddSingleton<ILifecycleParticipant<ISiloLifecycle>, SiloStartupNotifier>();


        }

        public void Configure(IEngineBuilder engine)
        {
        }

        public int Order => 149;    // Puedes ajustar el orden según sea necesario
    }
}
