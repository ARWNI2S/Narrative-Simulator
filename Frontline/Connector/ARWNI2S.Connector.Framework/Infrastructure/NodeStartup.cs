using ARWNI2S.Infrastructure;
using ARWNI2S.Infrastructure.Engine.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ARWNI2S.Connector.Framework.Infrastructure
{
    internal class NodeStartup : INI2SStartup
    {
        public int Order => 2000;

        public void Configure(IEngineBuilder engine)
        {

        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {

        }
    }
}
