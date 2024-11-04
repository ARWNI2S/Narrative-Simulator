using ARWNI2S.Runtime;

namespace ARWNI2S.Narrator
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await EntryPoints.CreateStartAsync(args);
        }
    }
}