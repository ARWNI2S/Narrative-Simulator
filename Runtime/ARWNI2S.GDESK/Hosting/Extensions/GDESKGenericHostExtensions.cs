using ARWNI2S.Engine.Simulation;
using Microsoft.Extensions.Hosting;

namespace ARWNI2S.Engine.Hosting.Extensions
{
    public static class GDESKGenericHostExtensions
    {
        public static IHost ConfigureSimulation(this IHost host)
        {
            SimulationContext.Initialize(host.Services);

            return host;
        }
    }
}
