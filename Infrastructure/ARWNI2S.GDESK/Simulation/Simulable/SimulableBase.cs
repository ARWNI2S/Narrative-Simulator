using ARWNI2S.Engine.Simulation.Runtime.Update;
using ARWNI2S.Engine.Simulation.Simulable.Builder;

namespace ARWNI2S.Engine.Simulation.Simulable
{
    public abstract class SimulableBase : ISimulable
    {
        protected UpdateFunction updateFunction;

        public virtual Guid UUID { get; protected set; }

        public virtual string Name { get; protected internal set; }

        public virtual ISimulable Outer { get; protected internal set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SimulableBase()
        {
            ISimulableObjectBuilder objectBuilder = SimulationContext.Context.GetContextBuilder() ?? throw new InvalidOperationException("The simulable is not being constructed by a factory.");
        }

        public SimulableBase(ISimulableObjectBuilder builder)
        {
            builder.SimulableObject = this;
            builder.FinalizeComposition();
        }

        protected virtual void InitializeInternal(ISimulableObjectBuilder objectBuilder)
        {
            UUID = objectBuilder.Build();
        }


        UpdateFunction ISimulable.UpdateFunction => updateFunction;


        void ISimulable.InitializeInternal(ISimulableObjectBuilder objectBuilder) => InitializeInternal(objectBuilder);

    }
}
