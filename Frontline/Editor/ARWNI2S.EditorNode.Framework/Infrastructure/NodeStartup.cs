using ARWNI2S.Infrastructure;
using ARWNI2S.Infrastructure.Engine.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ARWNI2S.EditorNode.Framework.Infrastructure
{
    internal class NodeStartup : INI2SStartup
    {
        public int Order => 2000;

        public void Configure(IEngineBuilder application)
        {

        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {

        }
    }
}
