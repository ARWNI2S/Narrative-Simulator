using ARWNI2S.Node.Core.Engine;
using Microsoft.Extensions.DependencyInjection;

namespace ARWNI2S.Engine.Builder
{
    public interface IEngineCoreBuilder
    {
        IServiceCollection Services { get; }

        EnginePartManager PartManager { get; }
    }
}