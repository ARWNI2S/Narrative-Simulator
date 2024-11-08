using ARWNI2S.Infrastructure;

namespace ARWNI2S.Portal.Framework.Infrastructure.Extensions
{
    internal class PortalContextAccessor : IContextAccessor
    {
        private static readonly AsyncLocal<ContextHolder> _netContextCurrent = new AsyncLocal<ContextHolder>();

        /// <inheritdoc/>
        public IContext Context
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
                    _netContextCurrent.Value = new ContextHolder { Context = value };
                }
            }
        }


        private sealed class ContextHolder
        {
            public IContext Context;
        }
    }
}