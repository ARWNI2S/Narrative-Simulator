
namespace ARWNI2S.Engine.Simulation.Runtime
{
    public interface IGameRuntime
    {
        void Start(CancellationToken? token = null);

        public void Stop();
    }
}
