namespace ARWNI2S.Engine.Simulation.Entities.Components
{
    public abstract class ActorComponentBase : SimulableEntity, IActorComponent
    {
        #region Fields

        #endregion

        #region Properties

        public IActorEntity Owner { get; protected set; }

        public IActorComponent? Parent { get; protected set; }

        public bool Registered => throw new NotImplementedException();

        public bool AllowReregistration => throw new NotImplementedException();

        #endregion

        #region Ctor

        protected ActorComponentBase(IActorEntity ownerActor, IActorComponent? parent = null)
        {
            Owner = ownerActor;
            Parent = parent;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Register this component, creating any required state. Will also add itself to the outer Actor's Components array,
        /// if not already present.
        /// </summary>
        public void RegisterComponent()
        {
            // TODO: RegisterComponent
        }

        /// <summary>
        /// Unregister this component, destroying any state.
        /// </summary>
        public void UnregisterComponent()
        {
            // TODO: UnregisterComponent
        }

        /// <summary>
        /// Unregister the component, remove it from its outer Actor's Components array and mark for pending kill.
        /// </summary>
        /// <param name="promoteChildren"></param>
        public virtual void DestroyComponent(bool promoteChildren = false)
        {
            // TODO: DestroyComponent
        }

        /// <summary>
        /// Called when a component is created (not loaded). This can happen in the editor or during gameplay
        /// </summary>
        public virtual void OnComponentCreated()
        {
            // TODO: OnComponentCreated
        }


        #region Factory

        #endregion

        #endregion

    }
}
