using ARWNI2S.Node;

namespace ARWNI2S.Frontline
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //await NI2SNode.Create(args).RunAsync();



            var builder = NI2SNode.CreateBuilder(args);

            builder.UseOrleansClient();

            var engine = builder.Build();

            await engine.RunAsync();
        }
    }
}