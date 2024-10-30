using ARWNI2S.Engine.Simulation.Entities;

namespace ARWNI2S.Engine.Simulation.Objects
{
    public abstract class GameObjectBase : IGameObject
    {
        public virtual Guid UUID { get; protected set; }

        public virtual void InitializeGameObject(IGameObjectBuilder objectBuilder)
        {
            UUID = objectBuilder.Build(this);
        }
    }
}
