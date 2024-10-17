using ARWNI2S.Infrastructure;

namespace ARWNI2S.EditorNode.Infrastructure
{
    internal class EditorStartup : INI2SStartup
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
