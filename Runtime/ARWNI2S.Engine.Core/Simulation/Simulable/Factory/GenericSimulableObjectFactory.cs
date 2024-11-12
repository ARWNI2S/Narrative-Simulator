namespace ARWNI2S.Engine.Simulation.Simulable.Factory
{
    internal sealed class GenericSimulableObjectFactory<TSimulable> : SimulationObjectFactory<TSimulable> where TSimulable : SimulableBase
    {
        protected override Type GetFactoryType() => typeof(TSimulable);
    }
}
