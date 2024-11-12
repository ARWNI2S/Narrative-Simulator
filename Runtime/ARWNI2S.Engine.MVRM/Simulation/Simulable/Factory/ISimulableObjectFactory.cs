namespace ARWNI2S.Engine.Simulation.Simulable.Factory
{
    public interface ISimulableObjectFactory
    {
        Type SimulableType { get; }
    }

    public interface ISimulableObjectFactory<TSimulable> : ISimulableObjectFactory where TSimulable : class, ISimulable
    {
        new Type SimulableType => typeof(TSimulable);
    }
}
