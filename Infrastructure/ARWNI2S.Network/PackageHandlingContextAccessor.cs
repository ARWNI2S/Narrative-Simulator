using ARWNI2S.Engine.Network.Session;

namespace ARWNI2S.Engine.Network
{
    public class PackageHandlingContextAccessor<TPackageInfo> : IPackageHandlingContextAccessor<TPackageInfo>
    {
        private static AsyncLocal<PackageHandlingContextHolder> AppSessionCurrent { get; set; } = new AsyncLocal<PackageHandlingContextHolder>();

        /// <inheritdoc/>
        PackageHandlingContext<INodeSession, TPackageInfo> IPackageHandlingContextAccessor<TPackageInfo>.PackageHandlingContext
        {
            get
            {
                return AppSessionCurrent.Value?.Context;
            }
            set
            {
                var holder = AppSessionCurrent.Value;
                if (holder != null)
                {
                    holder.Context = null;
                }

                if (value != null)
                {
                    AppSessionCurrent.Value = new PackageHandlingContextHolder { Context = value };
                }
            }
        }

        private class PackageHandlingContextHolder
        {
            public PackageHandlingContext<INodeSession, TPackageInfo> Context { get; set; }
        }
    }


}
