namespace ARWNI2S.Engine.Simulation.Simulable.Factory
{
    public abstract class SimulableObjectFactory : ISimulableObjectFactory<SimulableBase>
    {
        protected virtual Type GetFactoryType() => typeof(SimulableBase);

        Type ISimulableObjectFactory.SimulableType => GetFactoryType();
    }

    public abstract class SimulationObjectFactory<TSimulable> : SimulableObjectFactory, ISimulableObjectFactory<TSimulable> where TSimulable : class, ISimulable
    {
        protected override Type GetFactoryType() => typeof(TSimulable);
    }
}
