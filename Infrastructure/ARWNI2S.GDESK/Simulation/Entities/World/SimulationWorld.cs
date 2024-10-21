using ARWNI2S.Engine.Simulation.Entities;
using ARWNI2S.Infrastructure;
using ARWNI2S.Infrastructure.Entities;

namespace ARWNI2S.Engine.Simulation.World
{
    public partial class SimulationWorld : IActorEntity, INI2SEntity
    {
        public static Guid Id => Constants._UUID_WORLD_ACTOR;

        public IActorComponent RootComponent { get; }

        public SimulationWorld()
        {
            RootComponent = new WorldRootComponent(this);
        }

        #region Factory

        Guid ISimulableEntity.Id { get => Id; set { /* Do Nothing */ } }
        void ISimulableEntity.SetUUID(Guid uuid) { /* Do Nothing */ }
        object INI2SEntity.Id => Id;


        #endregion

    }
}
