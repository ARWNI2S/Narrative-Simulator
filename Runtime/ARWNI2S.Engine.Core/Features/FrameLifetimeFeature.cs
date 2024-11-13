using ARWNI2S.Infrastructure.Engine.Features;

namespace ARWNI2S.Engine.Features
{
    public class FrameLifetimeFeature : IFrameLifetimeFeature
    {
        /// <inheritdoc />
        public CancellationToken FrameAborted { get; set; }

        /// <inheritdoc />
        public void Abort()
        {
        }
    }
}
