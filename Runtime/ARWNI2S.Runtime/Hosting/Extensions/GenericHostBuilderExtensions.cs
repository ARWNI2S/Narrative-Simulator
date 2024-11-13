using ARWNI2S.Node.Builder;
using ARWNI2S.Node.Configuration.Options;
using Microsoft.Extensions.Hosting;

namespace ARWNI2S.Engine.Hosting.Extensions
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
    }
}
