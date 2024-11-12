using ARWNI2S.Infrastructure.EngineParts;
using Microsoft.Extensions.DependencyInjection;

namespace ARWNI2S.Engine.Builder
{
    /// <summary>
    /// Allows fine grained configuration of essential MVRM services.
    /// </summary>
    internal sealed class MVRMCoreBuilder : IMVRMCoreBuilder
    {
        /// <summary>
        /// Initializes a new <see cref="MVRMCoreBuilder"/> instance.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="manager">The <see cref="EnginePartManager"/> of the application.</param>
        public MVRMCoreBuilder(
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