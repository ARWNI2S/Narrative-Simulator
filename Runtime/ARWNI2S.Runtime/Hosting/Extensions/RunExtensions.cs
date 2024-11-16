using ARWNI2S.Infrastructure.Engine;
using ARWNI2S.Infrastructure.Engine.Builder;

namespace ARWNI2S.Node.Hosting.Extensions
{
    internal static class RunExtensions
    {
        public static void Run(this IEngineBuilder engine, UpdateDelegate updateHandler)
        {
            UpdateDelegate handler = updateHandler;
            ArgumentNullException.ThrowIfNull(engine, "engine");
            ArgumentNullException.ThrowIfNull(handler, "handler");
            engine.Use((UpdateDelegate _) => handler);
        }
    }
}
