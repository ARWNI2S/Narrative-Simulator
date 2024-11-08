namespace ARWNI2S.Engine.Network.Session
{
    public interface ISessionContainer
    {
        INodeSession GetSessionByID(string sessionID);

        int GetSessionCount();

        IEnumerable<INodeSession> GetSessions(Predicate<INodeSession> criteria = null);

        IEnumerable<TAppSession> GetSessions<TAppSession>(Predicate<TAppSession> criteria = null)
            where TAppSession : INodeSession;
    }

    public interface IAsyncSessionContainer
    {
        ValueTask<INodeSession> GetSessionByIDAsync(string sessionID);

        ValueTask<int> GetSessionCountAsync();

        ValueTask<IEnumerable<INodeSession>> GetSessionsAsync(Predicate<INodeSession> criteria = null);

        ValueTask<IEnumerable<TAppSession>> GetSessionsAsync<TAppSession>(Predicate<TAppSession> criteria = null)
            where TAppSession : INodeSession;
    }
}