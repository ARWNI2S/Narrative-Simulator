using ARWNI2S.Infrastructure.Entities;

namespace ARWNI2S.Engine.Entities
{
    public abstract partial class EntityBase : IEntity
    {
        public Guid UUID { get; internal set; }

        public string Name { get; internal set; }

        object IEntity.Id => UUID;
    }
}
