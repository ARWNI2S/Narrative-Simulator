
namespace ARWNI2S.Engine.Simulation.Runtime
{
    public interface ISimulableRuntime
    {
        void Start(CancellationToken? token = null);

        public void Stop();
    }
}
