using ARWNI2S.Engine.Entities;
using ARWNI2S.Engine.Orleans.Grains;

namespace ARWNI2S.Engine.Core.Object
{
    public abstract class NiisObject : EntityBase, IDistributedEntity
    {
        private IEntityGrain _entityProxy;

        IEntityGrain IDistributedEntity.EntityProxy { get => _entityProxy; set => _entityProxy = value; }
    }
}