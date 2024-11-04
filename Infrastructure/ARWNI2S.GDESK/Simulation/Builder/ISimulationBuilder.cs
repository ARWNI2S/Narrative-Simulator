using Microsoft.Extensions.DependencyInjection;

namespace ARWNI2S.Engine.Simulation.Builder
{
    public interface ISimulationBuilder
    {
        IServiceCollection Services { get; }
        IServiceProvider BuildProvider();
    }
}
