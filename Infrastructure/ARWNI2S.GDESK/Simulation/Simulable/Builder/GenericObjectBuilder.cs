namespace ARWNI2S.Engine.Simulation.Simulable.Builder
{
    internal sealed class GenericObjectBuilder<TSimulable> : SimulableObjectBuilder<TSimulable> where TSimulable : SimulableBase
    {
        protected override void BeginComposition()
        {
            base.BeginComposition();
        }

        protected override void FinalizeComposition()
        {
            base.FinalizeComposition();
        }

        protected override Guid Build(TSimulable simulableBase)
        {
            simulableBase.Name = "Name";
            return new Guid(simulableBase.Name);
        }
    }
}
