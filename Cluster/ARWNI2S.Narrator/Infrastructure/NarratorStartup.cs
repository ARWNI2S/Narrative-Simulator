using ARWNI2S.Infrastructure;

namespace ARWNI2S.Narrator.Infrastructure
{
    internal class NarratorStartup : INI2SStartup
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
