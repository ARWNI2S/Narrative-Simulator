using ARWNI2S.Node.Runtime.Services.Clustering;

namespace ARWNI2S.Node.Services.Clustering.Services
{
    // Implementation of the Actor Directory Service
    public class ActorDirectoryService : Grain, IActorDirectoryService
    {
        private readonly Dictionary<Guid, string> _actorRegistry = [];

        public Task RegisterActor(Guid actorId, string siloAddress)
        {
            _actorRegistry[actorId] = siloAddress;
            return Task.CompletedTask;
        }

        public Task UnregisterActor(Guid actorId)
        {
            _actorRegistry.Remove(actorId);
            return Task.CompletedTask;
        }

        public Task<string> GetActorSilo(Guid actorId)
        {
            if (_actorRegistry.TryGetValue(actorId, out string siloAddress))
            {
                return Task.FromResult(siloAddress);
            }
            return Task.FromResult<string>(null);
        }

        public Task<IEnumerable<Guid>> GetAllActors()
        {
            return Task.FromResult(_actorRegistry.Keys.AsEnumerable());
        }
    }
}
