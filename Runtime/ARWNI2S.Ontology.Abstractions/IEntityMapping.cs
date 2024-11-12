namespace ARWNI2S.Ontology.Abstractions
{
    public interface IEntityMapping
    {
        Guid UUID { get; set; }

        int OntologyId { get; set; }
    }
}
