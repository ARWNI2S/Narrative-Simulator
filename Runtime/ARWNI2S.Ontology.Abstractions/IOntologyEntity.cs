using ARWNI2S.Infrastructure.Entities;

namespace ARWNI2S.Ontology.Abstractions
{
    public interface IOntologyEntity : IEntity
    {
        new int Id { get; set; }
    }
}
