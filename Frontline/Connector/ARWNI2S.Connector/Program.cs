using ARWNI2S.Node;

namespace ARWNI2S.Connector
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await EntryPoints.RunDefaultsAsync(args);
        }
    }
}