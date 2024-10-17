using ARWNI2S.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ARWNI2S.Connector.Framework.Infrastructure
{
    internal class NodeStartup : INI2SStartup
    {
        public int Order => 2000;

        public void Configure(IHost application)
        {

        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {

        }
    }
}
