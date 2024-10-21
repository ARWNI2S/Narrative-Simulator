using ARWNI2S.Engine.Simulation.Entities.Scene;

namespace ARWNI2S.Engine.Simulation.Entities
{
    /// <summary>
    /// Interface for all objects in the simulation capable of send.
    /// </summary>
    public interface ISimulableEntity
    {
        #region Properties
        public Guid Id { get; protected set; }

        SimulationScene Scene { get; }

        #endregion

        #region Methods

        internal virtual void TickSimulable(float deltaSeconds, SceneTickType TickType, Action<float> tickFunction)
        {
            //root of tick hierarchy

            // Non-player update.
            // If an Actor has been Destroyed or its level has been unloaded don't execute any queued ticks
            if (IsValidChecked(this) && Scene != null)
            {
                Tick(deltaSeconds); // perform any tick functions unique to an actor subclass
            }

        }

        protected virtual void Tick(float deltaSeconds)
        {

        }

        bool IsValidChecked(ISimulableEntity simulableEntity);

        #endregion

        #region Factory

        internal void SetUUID(Guid uuid) { Id = uuid; }

        #endregion

    }
}