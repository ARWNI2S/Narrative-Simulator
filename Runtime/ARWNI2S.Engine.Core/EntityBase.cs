using ARWNI2S.Infrastructure.Entities;

namespace ARWNI2S.Engine
{
    public abstract partial class EntityBase : IEntity
    {
        public Guid UUID { get; internal set; }

        object IEntity.Id => UUID;
    }
}
