using ARWNI2S.Infrastructure;
using ARWNI2S.Infrastructure.Engine.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ARWNI2S.Node.Infrastructure
{
    internal class RuntimeStartup : INI2SStartup
    {
        public void Configure(IEngineBuilder engineBuilder)
        {
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => 2000;
    }
}
