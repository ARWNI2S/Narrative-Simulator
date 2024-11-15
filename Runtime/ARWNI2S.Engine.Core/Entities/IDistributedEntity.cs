using ARWNI2S.Engine.Orleans.Grains;

namespace ARWNI2S.Engine.Entities
{
    public interface IDistributedEntity
    {
        internal IEntityGrain EntityProxy { get; set; }
    }
}
