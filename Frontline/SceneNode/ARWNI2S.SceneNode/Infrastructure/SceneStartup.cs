using ARWNI2S.Infrastructure;

namespace ARWNI2S.SceneNode.Infrastructure
{
    internal class SceneStartup : INI2SStartup
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
