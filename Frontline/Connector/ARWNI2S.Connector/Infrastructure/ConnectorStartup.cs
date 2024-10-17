using ARWNI2S.Infrastructure;

namespace ARWNI2S.Connector.Infrastructure
{
    internal class ConnectorStartup : INI2SStartup
    {
        public int Order => 2002;

        public void Configure(IHost application)
        {

        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {

        }
    }
}
