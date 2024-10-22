using ARWNI2S.Engine.EngineParts;
using Microsoft.Extensions.DependencyInjection;

namespace ARWNI2S.Engine.Builder
{
    public interface IMVRMCoreBuilder
    {
        IServiceCollection Services { get; }
        EnginePartManager PartManager { get; }
    }
}