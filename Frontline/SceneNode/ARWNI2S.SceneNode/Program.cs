using ARWNI2S.Node.Runtime;

namespace ARWNI2S.SceneNode
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await EntryPoints.CreateStartAsync(args);
            //var builder = Host.CreateApplicationBuilder(args);
            //builder.Services.AddHostedService<Worker>();

            //var host = builder.Build();
            //host.Run();
        }
    }
}