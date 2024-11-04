using ARWNI2S.Engine.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ARWNI2S.Engine.Hosting.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IHostBuilder"/>.
    /// </summary>
    public static class GDESKGenericHostBuilderExtensions
    {
        private static readonly Type MarkerType = typeof(GDESKBuilderMarker);

        /// <summary>
        /// Configures the host app builder to host GDESK.
        /// </summary>
        /// <param name="hostAppBuilder">The host app builder.</param>
        /// <returns>The host builder.</returns>
        /// <remarks>
        /// Calling this method multiple times on the same <see cref="IHostApplicationBuilder"/> instance will result in one silo being configured.
        /// </remarks>
        public static IHostApplicationBuilder UseGDESK(
            this IHostApplicationBuilder hostAppBuilder)
            => hostAppBuilder.UseGDESK(_ => { });

        /// <summary>
        /// Configures the host builder to host GDESK.
        /// </summary>
        /// <param name="hostAppBuilder">The host app builder.</param>
        /// <param name="configureDelegate">The delegate used to configure the silo.</param>
        /// <returns>The host builder.</returns>
        /// <remarks>
        /// Calling this method multiple times on the same <see cref="IHostApplicationBuilder"/> instance will result in one silo being configured.
        /// However, the effects of <paramref name="configureDelegate"/> will be applied once for each call.
        /// </remarks>
        public static IHostApplicationBuilder UseGDESK(
            this IHostApplicationBuilder hostAppBuilder,
            Action<IGDESKBuilder> configureDelegate)
        {
            ArgumentNullException.ThrowIfNull(hostAppBuilder);
            ArgumentNullException.ThrowIfNull(configureDelegate);

            configureDelegate(AddGDESKCore(hostAppBuilder.Services, hostAppBuilder.Configuration));

            return hostAppBuilder;
        }

        /// <summary>
        /// Configures the host builder to host GDESK.
        /// </summary>
        /// <param name="hostBuilder">The host builder.</param>
        /// <param name="configureDelegate">The delegate used to configure the silo.</param>
        /// <returns>The host builder.</returns>
        /// <remarks>
        /// Calling this method multiple times on the same <see cref="IHostBuilder"/> instance will result in one silo being configured.
        /// However, the effects of <paramref name="configureDelegate"/> will be applied once for each call.
        /// </remarks>
        public static IHostBuilder UseGDESK(
            this IHostBuilder hostBuilder,
            Action<IGDESKBuilder> configureDelegate) => hostBuilder.UseGDESK((_, siloBuilder) => configureDelegate(siloBuilder));

        /// <summary>
        /// Configures the host builder to host GDESK.
        /// </summary>
        /// <param name="hostBuilder">The host builder.</param>
        /// <param name="configureDelegate">The delegate used to configure the silo.</param>
        /// <returns>The host builder.</returns>
        /// <remarks>
        /// Calling this method multiple times on the same <see cref="IHostBuilder"/> instance will result in one silo being configured.
        /// However, the effects of <paramref name="configureDelegate"/> will be applied once for each call.
        /// </remarks>
        public static IHostBuilder UseGDESK(
            this IHostBuilder hostBuilder,
            Action<HostBuilderContext, IGDESKBuilder> configureDelegate)
        {
            ArgumentNullException.ThrowIfNull(hostBuilder);
            ArgumentNullException.ThrowIfNull(configureDelegate);

            if (hostBuilder.Properties.ContainsKey("HasGDESKBuilder"))
            {
                throw GetGDESKAlreadyAddedException();
            }

            hostBuilder.Properties["HasGDESKBuilder"] = "true";

            return hostBuilder.ConfigureServices((context, services) => configureDelegate(context, AddGDESKCore(services, context.Configuration)));
        }

        /// <summary>
        /// Configures the service collection to host GDESK.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configureDelegate">The delegate used to configure the silo.</param>
        /// <returns>The service collection.</returns>
        /// <remarks>
        /// Calling this method multiple times on the same <see cref="IHostBuilder"/> instance will result in one silo being configured.
        /// However, the effects of <paramref name="configureDelegate"/> will be applied once for each call.
        /// </remarks>
        public static IServiceCollection AddGDESK(
            this IServiceCollection services,
            Action<IGDESKBuilder> configureDelegate)
        {
            ArgumentNullException.ThrowIfNull(configureDelegate);

            var builder = AddGDESKCore(services, null);

            configureDelegate(builder);

            return services;
        }

        private static IGDESKBuilder AddGDESKCore(IServiceCollection services, IConfiguration configuration)
        {
            IGDESKBuilder builder = default;
            configuration ??= new ConfigurationBuilder().Build();
            foreach (var descriptor in services.Where(d => d.ServiceType.Equals(MarkerType)))
            {
                var marker = (GDESKBuilderMarker)descriptor.ImplementationInstance;
                builder = marker.BuilderInstance switch
                {
                    IGDESKBuilder existingBuilder => existingBuilder,
                    _ => throw GetGDESKAlreadyAddedException()
                };
            }

            if (builder is null)
            {
                builder = new GDESKBuilder(services, configuration);
                services.AddSingleton(new GDESKBuilderMarker(builder));
            }

            return builder;
        }

        private static GDESKConfigurationException GetGDESKAlreadyAddedException() =>
            new("Do not call twice UseGDESK/AddGDESK. If you want multiple simulation in the same process, only UseGDESK/AddGDESK is necessary and call UseMultiSimulationKernel/AddMultiSimulationKernel.");
    }

    /// <summary>
    /// Marker type used for storing a builder in a service collection.
    /// </summary>
    internal sealed class GDESKBuilderMarker
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GDESKBuilderMarker"/> class.
        /// </summary>
        /// <param name="builderInstance">The builder instance.</param>
        public GDESKBuilderMarker(object builderInstance) => BuilderInstance = builderInstance;

        /// <summary>
        /// Gets the builder instance.
        /// </summary>
        public object BuilderInstance { get; }
    }

}
