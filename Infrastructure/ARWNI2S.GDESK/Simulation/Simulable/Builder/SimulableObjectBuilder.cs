using ARWNI2S.Engine.Simulation.Simulable.Factory;

namespace ARWNI2S.Engine.Simulation.Simulable.Builder
{
    public abstract class SimulableObjectBuilder<TSimulable> : ISimulableObjectBuilder<TSimulable> where TSimulable : class, ISimulable
    {
        public ISimulableObjectFactory<TSimulable> Factory { get; protected set; }
        public TSimulable SimulableObject { get; set; }

        protected abstract Guid Build(TSimulable simulableBase);

        protected virtual void BeginComposition()
        {

        }

        protected virtual void FinalizeComposition()
        {

        }

        #region ISimulableObjectBuilder impl

        ISimulableObjectFactory ISimulableObjectBuilder.Factory => Factory;
        ISimulable ISimulableObjectBuilder.SimulableObject { get => SimulableObject; set => SimulableObject = (TSimulable)value; }
        Type ISimulableObjectBuilder.SimulableType => typeof(TSimulable);

        void ISimulableObjectBuilder.BeginComposition() => BeginComposition();
        void ISimulableObjectBuilder.FinalizeComposition() => FinalizeComposition();

        Guid ISimulableObjectBuilder.Build() => Build(SimulableObject);

        #endregion
    }
}
