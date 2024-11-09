namespace ARWNI2S.Engine.Network.Session
{
    public class AsyncToSyncSessionContainerWrapper : ISessionContainer
    {
        IAsyncSessionContainer _asyncSessionContainer;

        public AsyncToSyncSessionContainerWrapper(IAsyncSessionContainer asyncSessionContainer)
        {
            _asyncSessionContainer = asyncSessionContainer;
        }

        public INodeSession GetSessionByID(string sessionID)
        {
            return _asyncSessionContainer.GetSessionByIDAsync(sessionID).Result;
        }

        public int GetSessionCount()
        {
            return _asyncSessionContainer.GetSessionCountAsync().Result;
        }

        public IEnumerable<INodeSession> GetSessions(Predicate<INodeSession> criteria)
        {
            return _asyncSessionContainer.GetSessionsAsync(criteria).Result;
        }

        public IEnumerable<TAppSession> GetSessions<TAppSession>(Predicate<TAppSession> criteria) where TAppSession : INodeSession
        {
            return _asyncSessionContainer.GetSessionsAsync(criteria).Result;
        }
    }
}