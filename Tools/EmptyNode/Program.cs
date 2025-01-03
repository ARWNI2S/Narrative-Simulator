using ARWNI2S.Node;

namespace ARWNI2S.Game
{
    public class Program
    {
        [MTAThread]
        public static void Main(string[] args)
        {
            NI2SNode.Create(args).Run();
        }
    }
}