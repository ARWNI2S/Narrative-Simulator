using ARWNI2S.Runtime.Services.Clustering;

namespace ARWNI2S.Frontline.Services.Clustering.Services
{
    // Implementation of the Actor Directory Service
    public class ActorDirectoryService : IActorDirectoryService
    {
        public Task<string> GetActorSilo(Guid actorId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Guid>> GetAllActors()
        {
            throw new NotImplementedException();
        }

        public Task RegisterActor(Guid actorId, string siloAddress)
        {
            throw new NotImplementedException();
        }

        public Task UnregisterActor(Guid actorId)
        {
            throw new NotImplementedException();
        }
    }
}
