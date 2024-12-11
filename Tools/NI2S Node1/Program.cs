using ARWNI2S.Hosting;

namespace NI2S_Node1
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await NI2SHost.Create(args).RunAsync();
        }
    }
}