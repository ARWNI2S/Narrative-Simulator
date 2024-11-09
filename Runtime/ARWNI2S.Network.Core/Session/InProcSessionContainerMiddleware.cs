using System.Collections.Concurrent;
using ARWNI2S.Engine.Network.Middleware;

namespace ARWNI2S.Engine.Network.Session
{
    public class InProcSessionContainerMiddleware : MiddlewareBase, ISessionContainer
    {
        private ConcurrentDictionary<string, INodeSession> _sessions;

        public InProcSessionContainerMiddleware(IServiceProvider serviceProvider)
        {
            Order = int.MaxValue; // make sure it is the last middleware
            _sessions = new ConcurrentDictionary<string, INodeSession>(StringComparer.OrdinalIgnoreCase);
        }

        public override ValueTask<bool> RegisterSession(INodeSession session)
        {
            if (session is IHandshakeRequiredSession handshakeSession)
            {
                if (!handshakeSession.Handshaked)
                    return new ValueTask<bool>(true);
            }

            _sessions.TryAdd(session.SessionID, session);
            return new ValueTask<bool>(true);
        }

        public override ValueTask<bool> UnRegisterSession(INodeSession session)
        {
            _sessions.TryRemove(session.SessionID, out INodeSession removedSession);
            return new ValueTask<bool>(true);
        }

        public INodeSession GetSessionByID(string sessionID)
        {
            _sessions.TryGetValue(sessionID, out INodeSession session);
            return session;
        }

        public int GetSessionCount()
        {
            return _sessions.Count;
        }

        public IEnumerable<INodeSession> GetSessions(Predicate<INodeSession> criteria = null)
        {
            var enumerator = _sessions.GetEnumerator();

            while (enumerator.MoveNext())
            {
                var s = enumerator.Current.Value;

                if (s.State != SessionState.Connected)
                    continue;

                if (criteria == null || criteria(s))
                    yield return s;
            }
        }

        public IEnumerable<TAppSession> GetSessions<TAppSession>(Predicate<TAppSession> criteria = null) where TAppSession : INodeSession
        {
            var enumerator = _sessions.GetEnumerator();

            while (enumerator.MoveNext())
            {
                if (enumerator.Current.Value is TAppSession s)
                {
                    if (s.State != SessionState.Connected)
                        continue;

                    if (criteria == null || criteria(s))
                        yield return s;
                }
            }
        }
    }
}
