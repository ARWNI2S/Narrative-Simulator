using ARWNI2S.Engine.Builder;
using ARWNI2S.Engine.Configuration.Options;
using ARWNI2S.Engine.Hosting.Internal;
using ARWNI2S.Infrastructure.Engine;
using ARWNI2S.Infrastructure.Engine.Builder;
using ARWNI2S.Node.Engine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace ARWNI2S.Engine.Hosting
{
    public sealed class NodeEngineHost : IHost, IEngineBuilder, IMessageRelayBuilder, IAsyncDisposable
    {
        internal const string GlobalNodeRelayerBuilderKey = "__GlobalNodeRelayerBuilder";

        private readonly IHost _host;
        private readonly List<RelayDataSource> _dataSources = [];

        internal NodeEngineHost(IHost host)
        {
            _host = host;
            EngineBuilder = new EngineBuilder(host.Services/*, ServerFeatures*/);
            Logger = host.Services.GetRequiredService<ILoggerFactory>().CreateLogger(Environment.ApplicationName ?? nameof(NodeEngineHost));

            Properties[GlobalNodeRelayerBuilderKey] = this;
        }

        /// <summary>
        /// The engine's configured services.
        /// </summary>
        public IServiceProvider Services => _host.Services;

        /// <summary>
        /// The engine's configured <see cref="IConfiguration"/>.
        /// </summary>
        public IConfiguration Configuration => _host.Services.GetRequiredService<IConfiguration>();

        /// <summary>
        /// The engine's configured <see cref="IHostEnvironment"/>.
        /// </summary>
        public IHostEnvironment Environment => _host.Services.GetRequiredService<IHostEnvironment>();

        /// <summary>
        /// Allows consumers to be notified of engine lifetime events.
        /// </summary>
        public IHostApplicationLifetime Lifetime => _host.Services.GetRequiredService<IHostApplicationLifetime>();

        /// <summary>
        /// The default logger for the engine.
        /// </summary>
        public ILogger Logger { get; }

        ///// <summary>
        ///// The list of URLs that the HTTP server is bound to.
        ///// </summary>
        //public ICollection<string> Urls => ServerFeatures.GetRequiredFeature<IServerAddressesFeature>().Addresses;

        IServiceProvider IEngineBuilder.EngineServices
        {
            get => EngineBuilder.EngineServices;
            set => EngineBuilder.EngineServices = value;
        }

        //internal IFeatureCollection ServerFeatures => _host.Services.GetRequiredService<IServer>().Features;
        //IFeatureCollection IEngineBuilder.ServerFeatures => ServerFeatures;

        internal IDictionary<string, object> Properties => EngineBuilder.Properties;
        IDictionary<string, object> IEngineBuilder.Properties => Properties;

        internal ICollection<RelayDataSource> DataSources => _dataSources;
        ICollection<RelayDataSource> IMessageRelayBuilder.DataSources => DataSources;

        internal EngineBuilder EngineBuilder { get; }

        IServiceProvider IMessageRelayBuilder.ServiceProvider => Services;

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeEngineHost"/> class with preconfigured defaults.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        /// <returns>The <see cref="NodeEngineHost"/>.</returns>
        public static NodeEngineHost Create(string[] args = null) =>
            new NodeEngineBuilder(new() { Args = args }).Build();

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeEngineBuilder"/> class with preconfigured defaults.
        /// </summary>
        /// <returns>The <see cref="NodeEngineBuilder"/>.</returns>
        public static NodeEngineBuilder CreateBuilder() =>
            new(new());

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeEngineBuilder"/> class with preconfigured defaults.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        /// <returns>The <see cref="NodeEngineBuilder"/>.</returns>
        public static NodeEngineBuilder CreateBuilder(string[] args) =>
            new(new() { Args = args });

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeEngineBuilder"/> class with preconfigured defaults.
        /// </summary>
        /// <param name="options">The <see cref="NodeEngineOptions"/> to configure the <see cref="NodeEngineBuilder"/>.</param>
        /// <returns>The <see cref="NodeEngineBuilder"/>.</returns>
        public static NodeEngineBuilder CreateBuilder(NodeEngineOptions options) =>
            new(options);

        /// <summary>
        /// Start the engine.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>
        /// A <see cref="Task"/> that represents the startup of the <see cref="NodeEngineHost"/>.
        /// Successful completion indicates the HTTP server is ready to accept new requests.
        /// </returns>
        public Task StartAsync(CancellationToken cancellationToken = default) =>
            _host.StartAsync(cancellationToken);

        /// <summary>
        /// Shuts down the engine.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>
        /// A <see cref="Task"/> that represents the shutdown of the <see cref="NodeEngineHost"/>.
        /// Successful completion indicates that all the HTTP server has stopped.
        /// </returns>
        public Task StopAsync(CancellationToken cancellationToken = default) =>
            _host.StopAsync(cancellationToken);

        /// <summary>
        /// Runs a engine and returns a Task that only completes when the token is triggered or shutdown is triggered.
        /// </summary>
        /// <param name="url">The URL to listen to if the server hasn't been configured directly.</param>
        /// <returns>
        /// A <see cref="Task"/> that represents the entire runtime of the <see cref="NodeEngineHost"/> from startup to shutdown.
        /// </returns>
        public Task RunAsync([StringSyntax(StringSyntaxAttribute.Uri)] string url = null)
        {
            Listen(url);
            return HostingAbstractionsHostExtensions.RunAsync(this);
        }

        /// <summary>
        /// Runs a engine and block the calling thread until host shutdown.
        /// </summary>
        /// <param name="url">The URL to listen to if the server hasn't been configured directly.</param>
        public void Run([StringSyntax(StringSyntaxAttribute.Uri)] string url = null)
        {
            Listen(url);
            HostingAbstractionsHostExtensions.Run(this);
        }

        /// <summary>
        /// Disposes the engine.
        /// </summary>
        void IDisposable.Dispose() => _host.Dispose();

        /// <summary>
        /// Disposes the engine.
        /// </summary>
        public ValueTask DisposeAsync() => ((IAsyncDisposable)_host).DisposeAsync();

        internal FrameDelegate BuildFrameDelegate() => EngineBuilder.Build();
        FrameDelegate IEngineBuilder.Build() => BuildFrameDelegate();

        // REVIEW: Should this be wrapping another type?
        IEngineBuilder IEngineBuilder.New()
        {
            var newBuilder = EngineBuilder.New();
            // Remove the route builder so branched pipelines have their own routing world
            newBuilder.Properties.Remove(GlobalNodeRelayerBuilderKey);
            return newBuilder;
        }

        /// <summary>
        /// Adds the middleware to the engine request pipeline.
        /// </summary>
        /// <param name="middleware">The middleware.</param>
        /// <returns>An instance of <see cref="IEngineBuilder"/> after the operation has completed.</returns>
        public IEngineBuilder Use(Func<FrameDelegate, FrameDelegate> middleware)
        {
            EngineBuilder.Use(middleware);
            return this;
        }

        IEngineBuilder IMessageRelayBuilder.CreateEngineBuilder() => ((IEngineBuilder)this).New();

        private void Listen(string url)
        {
            if (url is null)
            {
                return;
            }

            //var addresses = ServerFeatures.Get<IServerAddressesFeature>()?.Addresses;
            //if (addresses is null)
            //{
            //    throw new InvalidOperationException($"Changing the URL is not supported because no valid {nameof(IServerAddressesFeature)} was found.");
            //}
            //if (addresses.IsReadOnly)
            //{
            //    throw new InvalidOperationException($"Changing the URL is not supported because {nameof(IServerAddressesFeature.Addresses)} {nameof(ICollection<string>.IsReadOnly)}.");
            //}

            //addresses.Clear();
            //addresses.Add(url);
        }

        private string DebuggerToString()
        {
            return $@"EngineName = ""{Environment.ApplicationName}"", IsRunning = {(IsRunning ? "true" : "false")}";
        }

        // Node engine is running if the engine has been started and hasn't been stopped.
        private bool IsRunning => Lifetime.ApplicationStarted.IsCancellationRequested && !Lifetime.ApplicationStopped.IsCancellationRequested;

    }
}
