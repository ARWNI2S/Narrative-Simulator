using ARWNI2S.Engine.Orleans;

namespace ARWNI2S.Engine.Entities
{
    public interface IDistributedEntity
    {
        internal IEntityGrain EntityProxy { get; set; }
    }
}
