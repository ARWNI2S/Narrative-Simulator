using ARWNI2S.Node.Builder;
using ARWNI2S.Node.Configuration.Options;
using ARWNI2S.Node.Hosting.Infrastructure;
using ARWNI2S.Node.Hosting.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ARWNI2S.Node.Hosting.Extensions
{
    public static class GenericHostBuilderExtensions
    {
        /// <summary>
        /// Configures a <see cref="IHostBuilder" /> with defaults for hosting a node engine. This should be called
        /// before engine specific configuration to avoid it overwriting provided services, configuration sources,
        /// environments, content root, etc.
        /// </summary>
        /// <remarks>
        /// The following defaults are applied to the <see cref="IHostBuilder"/>:
        /// <list type="bullet">
        ///     <item><description>use Niis Kernel as the node engine and configure it using the engine's configuration providers</description></item>
        ///     <item><description>configure <see cref="IHostEnvironment.ContentRootFileProvider"/> to include static node assets from projects referenced by the entry assembly during development</description></item>
        ///     <item><description>adds the HostFiltering middleware</description></item>
        ///     <item><description>adds the ForwardedHeaders middleware if ARWNI2S_FORWARDEDHEADERS_ENABLED=true,</description></item>
        ///     <item><description>enable IIS integration</description></item>
        ///   </list>
        /// </remarks>
        /// <param name="builder">The <see cref="IHostBuilder" /> instance to configure.</param>
        /// <param name="configure">The configure callback</param>
        /// <param name="configureOptions">The delegate that configures the <see cref="NodeHostBuilderOptions"/>.</param>
        /// <returns>A reference to the <paramref name="builder"/> after the operation has completed.</returns>
        public static IHostBuilder ConfigureNodeHostDefaults(this IHostBuilder builder, Action<INodeHostBuilder> configure, Action<NodeHostBuilderOptions> configureOptions)
        {
            ArgumentNullException.ThrowIfNull(configure);

            return builder.ConfigureNodeHost(nodeHostBuilder =>
            {
                NiisHost.ConfigureNodeDefaults(nodeHostBuilder);

                configure(nodeHostBuilder);
            }, configureOptions);
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
