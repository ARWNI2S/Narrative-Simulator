using ARWNI2S.Infrastructure.EngineParts;
using Microsoft.Extensions.DependencyInjection;

namespace ARWNI2S.Engine.Builder
{
    public interface IMVRMCoreBuilder
    {
        IServiceCollection Services { get; }
        EnginePartManager PartManager { get; }
    }
}