using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ARWNI2S.Engine.Hosting
{
    internal class GDESKBuilder : IGDESKBuilder
    {
        public GDESKBuilder(IServiceCollection services, IConfiguration configuration)
        {
            Services = services;
            Configuration = configuration;
            DefaultGDESKServices.AddDefaultServices(this);
        }

        public IServiceCollection Services { get; }
        public IConfiguration Configuration { get; }
    }
}
