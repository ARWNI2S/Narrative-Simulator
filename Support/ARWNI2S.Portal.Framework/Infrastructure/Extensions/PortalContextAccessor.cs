using ARWNI2S.Node.Core.Network;

namespace ARWNI2S.Portal.Framework.Infrastructure.Extensions
{
    internal class PortalContextAccessor : INetworkContextAccessor
    {
        private static readonly AsyncLocal<NetworkContextHolder> _netContextCurrent = new AsyncLocal<NetworkContextHolder>();

        /// <inheritdoc/>
        public INetworkContext NetworkContext
        {
            get
            {
                return _netContextCurrent.Value?.Context;
            }
            set
            {
                var holder = _netContextCurrent.Value;
                if (holder != null)
                {
                    // Clear current EngineContext trapped in the AsyncLocals, as its done.
                    holder.Context = null;
                }

                if (value != null)
                {
                    // Use an object indirection to hold the EngineContext in the AsyncLocal,
                    // so it can be cleared in all ExecutionContexts when its cleared.
                    _netContextCurrent.Value = new NetworkContextHolder { Context = value };
                }
            }
        }


        private sealed class NetworkContextHolder
        {
            public INetworkContext Context;
        }
    }
}