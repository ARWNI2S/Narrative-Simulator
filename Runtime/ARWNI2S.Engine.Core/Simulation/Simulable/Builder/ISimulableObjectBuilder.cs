using ARWNI2S.Engine.Simulation.Simulable.Factory;

namespace ARWNI2S.Engine.Simulation.Simulable.Builder
{
    public interface ISimulableObjectBuilder
    {
        ISimulableObjectFactory Factory { get; }

        Type SimulableType { get; }

        ISimulable SimulableObject { get; internal set; }

        Guid Build();

        void BeginComposition();
        void FinalizeComposition();
    }

    public interface ISimulableObjectBuilder<TSimulable> : ISimulableObjectBuilder where TSimulable : class, ISimulable
    {
        new ISimulableObjectFactory<TSimulable> Factory { get; }

        new Type SimulableType => typeof(TSimulable);

        new TSimulable SimulableObject { get; internal set; }

    }
}
