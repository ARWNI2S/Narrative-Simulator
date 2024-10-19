using ARWNI2S.Infrastructure.Configuration;

namespace ARWNI2S.Node.Core.Configuration
{

    /// <summary>
    /// Represents distributed cache configuration parameters
    /// </summary>

    public partial class OrleansSiloConfig : IConfig
    {
        /// <inheritdoc/>
        public int GetOrder() => 5;
    }
}
