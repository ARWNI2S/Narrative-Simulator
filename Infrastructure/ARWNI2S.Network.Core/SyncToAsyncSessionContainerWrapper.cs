using ARWNI2S.Engine.Network.Session;

namespace ARWNI2S.Engine.Network
{
    public class SyncToAsyncSessionContainerWrapper : IAsyncSessionContainer
    {
        ISessionContainer _syncSessionContainer;

        public ISessionContainer SessionContainer
        {
            get { return _syncSessionContainer; }
        }

        public SyncToAsyncSessionContainerWrapper(ISessionContainer syncSessionContainer)
        {
            _syncSessionContainer = syncSessionContainer;
        }

        public ValueTask<INodeSession> GetSessionByIDAsync(string sessionID)
        {
            return new ValueTask<INodeSession>(_syncSessionContainer.GetSessionByID(sessionID));
        }

        public ValueTask<int> GetSessionCountAsync()
        {
            return new ValueTask<int>(_syncSessionContainer.GetSessionCount());
        }

        public ValueTask<IEnumerable<INodeSession>> GetSessionsAsync(Predicate<INodeSession> criteria = null)
        {
            return new ValueTask<IEnumerable<INodeSession>>(_syncSessionContainer.GetSessions(criteria));
        }

        public ValueTask<IEnumerable<TAppSession>> GetSessionsAsync<TAppSession>(Predicate<TAppSession> criteria = null) where TAppSession : INodeSession
        {
            return new ValueTask<IEnumerable<TAppSession>>(_syncSessionContainer.GetSessions(criteria));
        }
    }
}