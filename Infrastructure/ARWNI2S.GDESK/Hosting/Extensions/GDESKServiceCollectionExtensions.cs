using ARWNI2S.Engine.Simulation;
using Microsoft.Extensions.DependencyInjection;

namespace ARWNI2S.Engine.Hosting.Extensions
{
    public static class GDESKServiceCollectionExtensions
    {
        public static IServiceCollection AddGDESK(this IServiceCollection services)
        {
            services.AddSingleton<DefaultSimulation>();
            services.AddHostedService<DefaultSimulationHost>();

            return services;
        }
    }
}
