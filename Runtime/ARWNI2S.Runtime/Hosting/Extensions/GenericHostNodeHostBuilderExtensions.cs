using ARWNI2S.Engine.Hosting.Internal;
using ARWNI2S.Node.Builder;
using ARWNI2S.Node.Configuration.Options;
using ARWNI2S.Node.Hosting;
using ARWNI2S.Node.Hosting.Infrastructure;
using ARWNI2S.Node.Hosting.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ARWNI2S.Engine.Hosting.Extensions
{
    //
    // Resumen:
    //     Contains extensions for an Microsoft.Extensions.Hosting.IHostBuilder.
    public static class GenericHostNodeHostBuilderExtensions
    {
        /// <summary>
        /// Adds and configures an NI2S node engine.
        /// </summary>
        /// <param name="builder">The <see cref="IHostBuilder"/> to add the <see cref="INodeHostBuilder"/> to.</param>
        /// <param name="configure">The delegate that configures the <see cref="INodeHostBuilder"/>.</param>
        /// <returns>The <see cref="IHostBuilder"/>.</returns>
        public static IHostBuilder ConfigureNodeHost(this IHostBuilder builder, Action<INodeHostBuilder> configure)
        {
            ArgumentNullException.ThrowIfNull(configure);

            return builder.ConfigureNodeHost(configure, _ => { });
        }

        /// <summary>
        /// Adds and configures an NI2S node engine.
        /// </summary>
        /// <param name="builder">The <see cref="IHostBuilder"/> to add the <see cref="INodeHostBuilder"/> to.</param>
        /// <param name="configure">The delegate that configures the <see cref="INodeHostBuilder"/>.</param>
        /// <param name="configureNodeHostBuilder">The delegate that configures the <see cref="NodeHostBuilderOptions"/>.</param>
        /// <returns>The <see cref="IHostBuilder"/>.</returns>
        public static IHostBuilder ConfigureNodeHost(this IHostBuilder builder, Action<INodeHostBuilder> configure, Action<NodeHostBuilderOptions> configureNodeHostBuilder)
        {
            return builder.ConfigureNodeHost(
                static (hostBuilder, options) => new GenericNodeHostBuilder(hostBuilder, options),
                configure,
                configureNodeHostBuilder);
        }

        private static IHostBuilder ConfigureNodeHost(
            this IHostBuilder builder,
            Func<IHostBuilder, NodeHostBuilderOptions, INodeHostBuilder> createNodeHostBuilder,
            Action<INodeHostBuilder> configure,
            Action<NodeHostBuilderOptions> configureNodeHostBuilder)
        {
            ArgumentNullException.ThrowIfNull(configure);
            ArgumentNullException.ThrowIfNull(configureNodeHostBuilder);

            // Light up custom implementations namely ConfigureHostBuilder which throws.
            if (builder is ISupportsConfigureNodeHost supportsConfigureNodeHost)
            {
                return supportsConfigureNodeHost.ConfigureNodeHost(configure, configureNodeHostBuilder);
            }

            var nodeHostBuilderOptions = new NodeHostBuilderOptions();
            configureNodeHostBuilder(nodeHostBuilderOptions);
            var nodehostBuilder = createNodeHostBuilder(builder, nodeHostBuilderOptions);
            configure(nodehostBuilder);
            builder.ConfigureServices((context, services) => services.AddHostedService<GenericNodeHostService>());
            return builder;
        }
    }
}