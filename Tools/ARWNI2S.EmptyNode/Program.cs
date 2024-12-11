using ARWNI2S.Hosting;

namespace ARWNI2S.EmptyNode
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await NI2SHost.Create(args).RunAsync();
        }
    }
}