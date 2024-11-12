using ARWNI2S.Infrastructure;

namespace ARWNI2S.Engine.Simulation
{
    public class GDESKException : NI2SException
    {
        public GDESKException(string message) : base(message)
        {
        }
    }
}