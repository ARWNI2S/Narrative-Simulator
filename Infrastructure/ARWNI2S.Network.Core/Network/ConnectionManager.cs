using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace ARWNI2S.Engine.Network
{
    public class ConnectionManager
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentDictionary<string, IServiceScope> _connectionScopes = new();

        public ConnectionManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IServiceScope CreateConnectionScope(string connectionId)
        {
            if (_connectionScopes.ContainsKey(connectionId))
                throw new InvalidOperationException("Scope already exists for this connection.");

            var scope = _serviceProvider.CreateScope();
            _connectionScopes[connectionId] = scope;
            return scope;
        }

        public IServiceScope GetConnectionScope(string connectionId)
        {
            return _connectionScopes.TryGetValue(connectionId, out var scope)
                ? scope
                : throw new KeyNotFoundException("Scope not found for the specified connection ID.");
        }

        public void RemoveConnectionScope(string connectionId)
        {
            if (_connectionScopes.TryGetValue(connectionId, out var scope))
            {
                scope.Dispose();
                _connectionScopes.TryRemove(connectionId, out _);
            }
        }
    }
}
