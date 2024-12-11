using ARWNI2S.Hosting;

namespace ARWNI2S.Narrator
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await NI2SHost.Create(args).RunAsync();
        }
    }
}