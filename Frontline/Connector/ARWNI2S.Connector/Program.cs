using ARWNI2S.Frontline;

namespace ARWNI2S.Connector
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