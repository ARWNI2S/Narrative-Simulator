using ARWNI2S.Engine.Builder;
using ARWNI2S.Node.Core.Engine;
using Microsoft.Extensions.DependencyInjection;

namespace ARWNI2S.Node.Builder
{
    /// <summary>
    /// Allows fine grained configuration of essential MVRM services.
    /// </summary>
    internal sealed class NiisBuilder : INiisBuilder
    {
        /// <summary>
        /// Initializes a new <see cref="NiisBuilder"/> instance.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="manager">The <see cref="EnginePartManager"/> of the engine.</param>
        public NiisBuilder(
            IServiceCollection services,
            EnginePartManager manager)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(manager);

            Services = services;
            PartManager = manager;
        }

        /// <inheritdoc />
        public EnginePartManager PartManager { get; }

        /// <inheritdoc />
        public IServiceCollection Services { get; }
    }
}