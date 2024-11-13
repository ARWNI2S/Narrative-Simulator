using ARWNI2S.Infrastructure.Engine;

namespace ARWNI2S.Portal.Framework.Infrastructure.Extensions
{
    internal class PortalContextAccessor : IEngineContextAccessor
    {
        private static readonly AsyncLocal<EngineContextHolder> _netContextCurrent = new();

        /// <inheritdoc/>
        public ARWNI2S.Infrastructure.Engine.EngineContext EngineContext
        {
            get
            {
                return _netContextCurrent.Value?.EngineContext;
            }
            set
            {
                var holder = _netContextCurrent.Value;
                if (holder != null)
                {
                    // Clear current EngineContext trapped in the AsyncLocals, as its done.
                    holder.EngineContext = null;
                }

                if (value != null)
                {
                    // Use an object indirection to hold the EngineContext in the AsyncLocal,
                    // so it can be cleared in all ExecutionContexts when its cleared.
                    _netContextCurrent.Value = new EngineContextHolder { EngineContext = value };
                }
            }
        }


        private sealed class EngineContextHolder
        {
            public ARWNI2S.Infrastructure.Engine.EngineContext EngineContext;
        }
    }
}