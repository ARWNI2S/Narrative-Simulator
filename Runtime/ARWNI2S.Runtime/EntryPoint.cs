using ARWNI2S.Engine.Hosting.Extensions;
using ARWNI2S.Node.Builder;
using ARWNI2S.Node.Hosting;
namespace ARWNI2S.Node
{
    public static class EntryPoint
    {
        public static async Task RunDefaultsAsync(string[] args)
        {
            var app = await StartDefaultsAsync(args);

            await app.RunAsync();
        }

        public static NodeEngineBuilder CreateDefaults(string[] args)
        {
            return NodeEngineHost.CreateDefaultBuilder(args);
        }

        public static NodeEngineHost BuildDefaults(string[] args)
        {
            return CreateDefaults(args).Build();
        }

        public static NodeEngineHost ConfigureDefaults(string[] args)
        {
            var engine = BuildDefaults(args);

            //configure the engine
            engine.ConfigureEngine();

            return engine;
        }

        public static async Task<NodeEngineHost> StartDefaultsAsync(string[] args)
        {
            var app = ConfigureDefaults(args);

            await app.StartEngineAsync();

            return app;
        }

    }
}