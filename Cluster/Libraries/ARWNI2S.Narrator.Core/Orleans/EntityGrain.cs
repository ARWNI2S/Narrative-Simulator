using Orleans.EventSourcing;

namespace ARWNI2S.Engine.Orleans
{
    internal class EntityGrain : JournaledGrain<GrainState, GrainEvent>, IEntityGrain
    {

    }
}
