using ARWNI2S.Frontline.Hosting;
using Microsoft.Extensions.Hosting;

namespace ARWNI2S.Frontline
{
    public sealed class EntryPoints
    {
        public static async Task CreateStartAsync(string[] args)
        {
            var builder = NodeHostBuilder.Create(args);

            // Construir y ejecutar el host
            var host = builder.Build();

            host.ConfigureEngine();
            await host.StartEngineAsync();

            await host.RunAsync();
        }

        public static async Task CreateRuntimeStartAsync(string[] args)
        {
            // Construir y ejecutar el host
            var host = NodeHostBuilder.CreateRuntimeHost(args);

            await host.StartEngineAsync();

            await host.RunAsync();
        }

        public static async Task CreateRuntimeAsyncStartAsync(string[] args)
        {
            // Construir y ejecutar el host
            var host = await NodeHostBuilder.CreateRuntimeHostAsync(args, true);

            await host.RunAsync();
        }
    }
}