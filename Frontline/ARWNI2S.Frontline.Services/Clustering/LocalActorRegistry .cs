using ARWNI2S.Node.Runtime.Clustering;

namespace ARWNI2S.Node.Services.Clustering
{
    // Implementation of the Local Actor Registry
    public class LocalActorRegistry : ILocalActorRegistry
    {
        private readonly HashSet<Guid> _localActors = [];

        public Task<IEnumerable<Guid>> GetLocalActors()
        {
            throw new NotImplementedException();
        }

        public Task RegisterLocalActor(Guid actorId)
        {
            throw new NotImplementedException();
        }

        public Task UnregisterLocalActor(Guid actorId)
        {
            throw new NotImplementedException();
        }
    }
}
