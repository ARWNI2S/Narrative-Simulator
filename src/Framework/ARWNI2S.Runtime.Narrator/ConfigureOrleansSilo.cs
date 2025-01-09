using ARWNI2S.Engine.Builder;
using ARWNI2S.Extensibility;
using ARWNI2S.Lifecycle;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ARWNI2S.Runtime.Narrator
{
    internal class ConfigureOrleansSilo : IConfigureEngine
    {
        public int Order => NI2SLifecycleStage.CoreInitialize;

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {

        }

        public void ConfigureEngine(IEngineBuilder engineBuilder)
        {
        }
    }
}
