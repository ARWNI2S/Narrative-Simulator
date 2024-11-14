using ARWNI2S.Engine.Entities;
using ARWNI2S.Engine.Orleans;

namespace ARWNI2S.Engine.Object
{
    public abstract class NiisObject : EntityBase, IDistributedEntity
    {
        private IEntityGrain _entityProxy;

        IEntityGrain IDistributedEntity.EntityProxy { get => _entityProxy; set => _entityProxy = value; }
    }
}