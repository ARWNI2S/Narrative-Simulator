namespace ARWNI2S.Engine.Simulation.Entities
{
    public interface IActorComponent
    {
        #region Properties

        IActorEntity Owner { get; }

        IActorComponent? Parent { get; }

        /// <summary>
        /// See if this component is currently registered
        /// </summary>
        bool Registered { get; }

        /// <summary>
        /// Check whether the component class allows reregistration during ReregisterComponents
        /// </summary>
        bool AllowReregistration { get; }

        #endregion

        #region Methods

        ///// <summary>
        ///// Register this component, creating any required state. Will also add itself to the outer Actor's Components array,
        ///// if not already present.
        ///// </summary>
        //void RegisterComponent();

        ///// <summary>
        ///// Unregister this component, destroying any state.
        ///// </summary>
        //void UnregisterComponent();

        ///// <summary>
        ///// Unregister the component, remove it from its outer Actor's Components array and mark for pending kill.
        ///// </summary>
        ///// <param name="promoteChildren"></param>
        //void DestroyComponent(bool promoteChildren = false);

        ///// <summary>
        ///// Called when a component is created (not loaded). This can happen in the editor or during gameplay
        ///// </summary>
        //void OnComponentCreated();

        #region Factory

        #endregion

        #endregion

    }
}
