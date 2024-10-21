using ARWNI2S.Engine.EngineParts;
using Microsoft.Extensions.DependencyInjection;

namespace ARWNI2S.Engine.Builder
{
    public interface INI2SCoreBuilder
    {
        IServiceCollection Services { get; }
        EnginePartManager PartManager { get; }
    }
}