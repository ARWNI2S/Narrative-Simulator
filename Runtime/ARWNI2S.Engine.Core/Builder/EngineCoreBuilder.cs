using ARWNI2S.Node.Core.Engine;
using ARWNI2S.Node.Core.Engine.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ARWNI2S.Engine.Builder
{
    /// <summary>
    /// Allows fine grained configuration of essential MVRM services.
    /// </summary>
    internal sealed class EngineCoreBuilder : IEngineCoreBuilder
    {
        /// <summary>
        /// Initializes a new <see cref="EngineCoreBuilder"/> instance.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="manager">The <see cref="EnginePartManager"/> of the engine.</param>
        public EngineCoreBuilder(
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