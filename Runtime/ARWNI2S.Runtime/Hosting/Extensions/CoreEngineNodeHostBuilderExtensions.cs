using ARWNI2S.Engine.Configuration.Options;
using ARWNI2S.Engine.Internal;
using ARWNI2S.Infrastructure.Engine;
using ARWNI2S.Node.Builder;
using ARWNI2S.Node.Hosting.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace ARWNI2S.Node.Hosting.Extensions
{
    internal static class CoreEngineNodeHostBuilderExtensions
    {
        /// <summary>
        /// In <see cref="UseCoreEngineCore(INodeHostBuilder)"/> scenarios, it may be necessary to explicitly
        /// opt in to certain HTTPS functionality.  For example, if <code>ASPNETCORE_URLS</code> includes
        /// an <code>https://</code> address, <see cref="UseCoreEngineHttpsConfiguration"/> will enable configuration
        /// of HTTPS on that endpoint.
        ///
        /// Has no effect in <see cref="UseCoreEngine(INodeHostBuilder)"/> scenarios.
        /// </summary>
        /// <param name="hostBuilder">
        /// The Microsoft.AspNetCore.Hosting.INodeHostBuilder to configure.
        /// </param>
        /// <returns>
        /// The Microsoft.AspNetCore.Hosting.INodeHostBuilder.
        /// </returns>
        public static INodeHostBuilder UseCoreEngineHttpsConfiguration(this INodeHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureServices(services =>
            {
                //services.AddSingleton<HttpsConfigurationService.IInitializer, HttpsConfigurationService.Initializer>();
            });
        }

        /// <summary>
        /// Specify CoreEngine as the server to be used by the web host.
        /// </summary>
        /// <param name="hostBuilder">
        /// The Microsoft.AspNetCore.Hosting.INodeHostBuilder to configure.
        /// </param>
        /// <returns>
        /// The Microsoft.AspNetCore.Hosting.INodeHostBuilder.
        /// </returns>
        public static INodeHostBuilder UseCoreEngine(this INodeHostBuilder hostBuilder)
        {
            return hostBuilder
                .UseCoreEngineCore()
                .UseCoreEngineHttpsConfiguration()
                /*.UseQuic(options =>
                {
                    //// Configure server defaults to match client defaults.
                    //// https://github.com/dotnet/runtime/blob/a5f3676cc71e176084f0f7f1f6beeecd86fbeafc/src/libraries/System.Net.Http/src/System/Net/Http/SocketsHttpHandler/ConnectHelper.cs#L118-L119
                    //options.DefaultStreamErrorCode = (long)Http3ErrorCode.RequestCancelled;
                    //options.DefaultCloseErrorCode = (long)Http3ErrorCode.NoError;
                })*/;
        }

        /// <summary>
        /// Specify CoreEngine as the server to be used by the web host.
        /// Includes less automatic functionality than <see cref="UseCoreEngine(INodeHostBuilder)"/> to make trimming more effective
        /// (e.g. for <see href="https://aka.ms/aspnet/nativeaot">Native AOT</see> scenarios).  If the host ends up depending on
        /// some of the absent functionality, a best-effort attempt will be made to enable it on-demand.  Failing that, an
        /// exception with an informative error message will be raised when the host is started.
        /// </summary>
        /// <param name="hostBuilder">
        /// The Microsoft.AspNetCore.Hosting.INodeHostBuilder to configure.
        /// </param>
        /// <returns>
        /// The Microsoft.AspNetCore.Hosting.INodeHostBuilder.
        /// </returns>
        public static INodeHostBuilder UseCoreEngineCore(this INodeHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices(services =>
            {
                // Don't override an already-configured transport
                //services.TryAddSingleton<IConnectionListenerFactory, SocketTransportFactory>();

                services.AddTransient<IConfigureOptions<EngineOptions>, EngineOptionsSetup>();
                //services.AddSingleton<IHttpsConfigurationService, HttpsConfigurationService>();
                services.AddSingleton<IEngine, CoreEngineImpl>();
                //services.AddSingleton<CoreEngineMetrics>();
            });

            if (OperatingSystem.IsWindows())
            {
                //hostBuilder.UseNamedPipes();
            }

            return hostBuilder;
        }

        /// <summary>
        /// Specify CoreEngine as the server to be used by the web host.
        /// </summary>
        /// <param name="hostBuilder">
        /// The Microsoft.AspNetCore.Hosting.INodeHostBuilder to configure.
        /// </param>
        /// <param name="options">
        /// A callback to configure CoreEngine options.
        /// </param>
        /// <returns>
        /// The Microsoft.AspNetCore.Hosting.INodeHostBuilder.
        /// </returns>
        public static INodeHostBuilder UseCoreEngine(this INodeHostBuilder hostBuilder, Action<EngineOptions> options)
        {
            return hostBuilder.UseCoreEngine().ConfigureCoreEngine(options);
        }

        /// <summary>
        /// Configures CoreEngine options but does not register an IServer. See <see cref="UseCoreEngine(INodeHostBuilder)"/>.
        /// </summary>
        /// <param name="hostBuilder">
        /// The Microsoft.AspNetCore.Hosting.INodeHostBuilder to configure.
        /// </param>
        /// <param name="options">
        /// A callback to configure CoreEngine options.
        /// </param>
        /// <returns>
        /// The Microsoft.AspNetCore.Hosting.INodeHostBuilder.
        /// </returns>
        public static INodeHostBuilder ConfigureCoreEngine(this INodeHostBuilder hostBuilder, Action<EngineOptions> options)
        {
            return hostBuilder.ConfigureServices(services =>
            {
                services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<EngineOptions>, EngineOptionsSetup>());
                services.Configure(options);
            });
        }

        /// <summary>
        /// Specify CoreEngine as the server to be used by the web host.
        /// </summary>
        /// <param name="hostBuilder">
        /// The Microsoft.AspNetCore.Hosting.INodeHostBuilder to configure.
        /// </param>
        /// <param name="configureOptions">A callback to configure CoreEngine options.</param>
        /// <returns>
        /// The Microsoft.AspNetCore.Hosting.INodeHostBuilder.
        /// </returns>
        public static INodeHostBuilder UseCoreEngine(this INodeHostBuilder hostBuilder, Action<NodeHostBuilderContext, EngineOptions> configureOptions)
        {
            return hostBuilder.UseCoreEngine().ConfigureCoreEngine(configureOptions);
        }

        /// <summary>
        /// Specify CoreEngine as the server to be used by the web host.
        /// </summary>
        /// <param name="hostBuilder">
        /// The Microsoft.AspNetCore.Hosting.INodeHostBuilder to configure.
        /// </param>
        /// <returns>
        /// The Microsoft.AspNetCore.Hosting.INodeHostBuilder.
        /// </returns>
        public static INodeHostBuilder UseNetwork(this INodeHostBuilder hostBuilder)
        {
            return hostBuilder;
        }
        /// <summary>
        /// Specify CoreEngine as the server to be used by the web host.
        /// </summary>
        /// <param name="hostBuilder">
        /// The Microsoft.AspNetCore.Hosting.INodeHostBuilder to configure.
        /// </param>
        /// <returns>
        /// The Microsoft.AspNetCore.Hosting.INodeHostBuilder.
        /// </returns>
        public static INodeHostBuilder UseNI2SIntegration(this INodeHostBuilder hostBuilder)
        {
            return hostBuilder;
        }

        /// <summary>
        /// Configures CoreEngine options but does not register an IServer. See <see cref="UseCoreEngine(INodeHostBuilder)"/>.
        /// </summary>
        /// <param name="hostBuilder">
        /// The Microsoft.AspNetCore.Hosting.INodeHostBuilder to configure.
        /// </param>
        /// <param name="configureOptions">A callback to configure CoreEngine options.</param>
        /// <returns>
        /// The Microsoft.AspNetCore.Hosting.INodeHostBuilder.
        /// </returns>
        public static INodeHostBuilder ConfigureCoreEngine(this INodeHostBuilder hostBuilder, Action<NodeHostBuilderContext, EngineOptions> configureOptions)
        {
            ArgumentNullException.ThrowIfNull(configureOptions);

            return hostBuilder.ConfigureServices((context, services) =>
            {
                services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<EngineOptions>, EngineOptionsSetup>());
                services.Configure<EngineOptions>(options =>
                {
                    configureOptions(context, options);
                });
            });
        }
    }
}
