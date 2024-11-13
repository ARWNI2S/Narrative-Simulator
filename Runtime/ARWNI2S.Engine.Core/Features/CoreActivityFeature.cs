using ARWNI2S.Infrastructure.Engine.Features;
using System.Diagnostics;

namespace ARWNI2S.Engine.Features
{
    /// <summary>
    /// Default implementation for <see cref="IActivityFeature"/>.
    /// </summary>
    internal sealed class CoreActivityFeature : IActivityFeature
    {
        internal CoreActivityFeature(Activity activity)
        {
            Activity = activity;
        }

        /// <inheritdoc />
        public Activity Activity { get; set; }
    }
}