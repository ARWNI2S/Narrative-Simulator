using ARWNI2S.Engine.Simulation.Entities.Builder;

namespace ARWNI2S.Engine.Simulation.Entities
{
    class VTableHelper
    {
        /// <summary>
        /// DO NOT USE. This constructor is for internal usage only for hot-reload purposes.
        /// </summary>
        public VTableHelper()
        {
            //EnsureRetrievingVTablePtrDuringCtor(TEXT("FVTableHelper()"));
        }
    };

    [Flags]
    enum EntityFlags
    {

    }

    public abstract class SimulableEntity : ISimulableEntity
    {
        #region Fields

        private Guid _uuid;

        #endregion

        #region Properties


        #endregion

        #region Ctor

        ///// <summary>
        ///// Default constructor
        ///// </summary>
        //public SimulableEntity() { }

        ///// <summary>
        ///// Constructor that takes an ObjectInitializer.
        ///// Typically not needed, but can be useful for class hierarchies that support
        ///// optional subobjects or subobject class overriding
        ///// </summary>
        //public SimulableEntity(EntityInitializer entityInitializer) { }

        ///* DO NOT USE. This constructor is for internal usage only for statically-created objects. */
        //internal SimulableEntity(EntityFlags flags) { }

        ///* DO NOT USE. This constructor is for internal usage only for hot-reload purposes. */
        //internal SimulableEntity(VTableHelper helper) { }

        #endregion


        #region Methods

        //public ISimulableEntity CreateDefaultSubentity(string subentityName, Type returnType, Type typeToCreateByDefault, bool required = false, bool transient = false)
        //{
        //    throw new NotImplementedException();
        //}

        #region Factory

        Guid ISimulableEntity.Id { get => _uuid; set => _uuid = value; }

        #endregion

        #endregion
    }
}
