using ARWNI2S.Infrastructure.Configuration;

namespace ARWNI2S.Engine.Configuration
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
