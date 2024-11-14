using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ARWNI2S.Engine.Configuration.Options
{
    /// <summary>
    /// Provides programmatic configuration of NI2SEngine-specific features.
    /// </summary>
    public class EngineOptions
    {
        static EngineOptions()
        {
            //AppContext.TryGetSwitch(FinOnErrorSwitch, out _finOnError);
            //AppContext.TryGetSwitch(CertificateFileWatchingSwitch, out _disableCertificateFileWatching);
        }

        /// <summary>
        /// Enables the Listen options callback to resolve and use services registered by the engine during startup.
        /// Typically initialized by UseNI2SEngine().
        /// </summary>
        public IServiceProvider EngineServices { get; set; } = default!; // This should typically be set

        ///// <summary>
        ///// Provides access to request limit options.
        ///// </summary>
        //public NI2SEngineLimits Limits { get; } = new NI2SEngineLimits();

        /// <summary>
        /// Provides a configuration source where endpoints will be loaded from on server start.
        /// The default is <see langword="null"/>.
        /// </summary>
        public EngineConfigurationLoader ConfigurationLoader { get; set; }

        ///// <summary>
        ///// A default configuration action for all endpoints. Use for Listen, configuration, the default url, and URLs.
        ///// </summary>
        //private Action<ListenOptions> EndpointDefaults { get; set; } = _ => { };

        ///// <summary>
        ///// A default configuration action for all https endpoints.
        ///// </summary>
        //private Action<HttpsConnectionAdapterOptions> HttpsDefaults { get; set; } = _ => { };

        ///// <summary>
        ///// The development server certificate for https endpoints. This is applied lazily after HttpsDefaults and user options.
        ///// </summary>
        ///// <remarks>
        ///// Getter exposed for testing.
        ///// </remarks>
        //internal X509Certificate2 DevelopmentCertificate { get; private set; }

        ///// <summary>
        ///// Allow tests to explicitly set the default certificate.
        ///// </summary>
        //internal X509Certificate2 TestOverrideDefaultCertificate { get; set; }

        ///// <summary>
        ///// Has the default dev certificate load been attempted?
        ///// </summary>
        //internal bool IsDevelopmentCertificateLoaded { get; set; }

        ///// <summary>
        ///// Internal AppContext switch to toggle the WebTransport and HTTP/3 datagrams experiemental features.
        ///// </summary>
        //private bool? _enableWebTransportAndH3Datagrams;
        //internal bool EnableWebTransportAndH3Datagrams
        //{
        //    get
        //    {
        //        if (!_enableWebTransportAndH3Datagrams.HasValue)
        //        {
        //            _enableWebTransportAndH3Datagrams = AppContext.TryGetSwitch("Microsoft.AspNetCore.Server.NI2SEngine.Experimental.WebTransportAndH3Datagrams", out var enabled) && enabled;
        //        }

        //        return _enableWebTransportAndH3Datagrams.Value;
        //    }
        //    set => _enableWebTransportAndH3Datagrams = value;
        //}

        ///// <summary>
        ///// Internal AppContext switch to toggle whether a request line can end with LF only instead of CR/LF.
        ///// </summary>
        //private bool? _disableHttp1LineFeedTerminators;
        //internal bool DisableHttp1LineFeedTerminators
        //{
        //    get
        //    {
        //        if (!_disableHttp1LineFeedTerminators.HasValue)
        //        {
        //            _disableHttp1LineFeedTerminators = AppContext.TryGetSwitch(DisableHttp1LineFeedTerminatorsSwitchKey, out var disabled) && disabled;
        //        }

        //        return _disableHttp1LineFeedTerminators.Value;
        //    }
        //    set => _disableHttp1LineFeedTerminators = value;
        //}

        ///// <summary>
        ///// Specifies a configuration Action to run for each newly created endpoint. Calling this again will replace
        ///// the prior action.
        ///// </summary>
        //public void ConfigureEndpointDefaults(Action<ListenOptions> configureOptions)
        //{
        //    EndpointDefaults = configureOptions ?? throw new ArgumentNullException(nameof(configureOptions));
        //}

        //internal void ApplyEndpointDefaults(ListenOptions listenOptions)
        //{
        //    listenOptions.NI2SEngineOptions = this;
        //    ConfigurationLoader?.ApplyEndpointDefaults(listenOptions);
        //    EndpointDefaults(listenOptions);
        //}

        ///// <summary>
        ///// Specifies a configuration Action to run for each newly created https endpoint. Calling this again will replace
        ///// the prior action.
        ///// </summary>
        //public void ConfigureHttpsDefaults(Action<HttpsConnectionAdapterOptions> configureOptions)
        //{
        //    HttpsDefaults = configureOptions ?? throw new ArgumentNullException(nameof(configureOptions));
        //}

        //internal void ApplyHttpsDefaults(HttpsConnectionAdapterOptions httpsOptions)
        //{
        //    ConfigurationLoader?.ApplyHttpsDefaults(httpsOptions);
        //    HttpsDefaults(httpsOptions);
        //}

        //internal void ApplyDefaultCertificate(HttpsConnectionAdapterOptions httpsOptions)
        //{
        //    if (httpsOptions.HasServerCertificateOrSelector)
        //    {
        //        return;
        //    }

        //    // It's important (and currently true) that we don't reach here with https configuration uninitialized because
        //    // we might incorrectly favor the development certificate over one specified by the user.
        //    Debug.Assert(EngineServices.GetRequiredService<IHttpsConfigurationService>().IsInitialized, "HTTPS configuration should have been enabled");

        //    if (TestOverrideDefaultCertificate is X509Certificate2 certificateFromTest)
        //    {
        //        httpsOptions.ServerCertificate = certificateFromTest;
        //        return;
        //    }

        //    if (ConfigurationLoader?.DefaultCertificate is X509Certificate2 certificateFromLoader)
        //    {
        //        httpsOptions.ServerCertificate = certificateFromLoader;
        //        return;
        //    }

        //    if (!IsDevelopmentCertificateLoaded)
        //    {
        //        IsDevelopmentCertificateLoaded = true;
        //        Debug.Assert(DevelopmentCertificate is null);
        //        var logger = EngineServices!.GetRequiredService<ILogger<NI2SEngine>>();
        //        DevelopmentCertificate = GetDevelopmentCertificateFromStore(logger);
        //    }

        //    httpsOptions.ServerCertificate = DevelopmentCertificate;
        //}

        //internal void EnableHttpsConfiguration()
        //{
        //    var httpsConfigurationService = EngineServices.GetRequiredService<IHttpsConfigurationService>();

        //    if (!httpsConfigurationService.IsInitialized)
        //    {
        //        var hostEnvironment = EngineServices.GetRequiredService<IHostEnvironment>();
        //        var logger = EngineServices.GetRequiredService<ILogger<NI2SEngine>>();
        //        var httpsLogger = EngineServices.GetRequiredService<ILogger<HttpsConnectionMiddleware>>();
        //        httpsConfigurationService.Initialize(hostEnvironment, logger, httpsLogger);
        //    }
        //}

        //internal void Serialize(Utf8JsonWriter writer)
        //{
        //    writer.WritePropertyName(nameof(AllowSynchronousIO));
        //    writer.WriteBooleanValue(AllowSynchronousIO);

        //    writer.WritePropertyName(nameof(AddServerHeader));
        //    writer.WriteBooleanValue(AddServerHeader);

        //    writer.WritePropertyName(nameof(AllowAlternateSchemes));
        //    writer.WriteBooleanValue(AllowAlternateSchemes);

        //    writer.WritePropertyName(nameof(AllowResponseHeaderCompression));
        //    writer.WriteBooleanValue(AllowResponseHeaderCompression);

        //    writer.WritePropertyName(nameof(IsDevelopmentCertificateLoaded));
        //    writer.WriteBooleanValue(IsDevelopmentCertificateLoaded);

        //    writer.WriteString(nameof(RequestHeaderEncodingSelector), RequestHeaderEncodingSelector == DefaultHeaderEncodingSelector ? "default" : "configured");
        //    writer.WriteString(nameof(ResponseHeaderEncodingSelector), ResponseHeaderEncodingSelector == DefaultHeaderEncodingSelector ? "default" : "configured");

        //    // Limits
        //    writer.WritePropertyName(nameof(Limits));
        //    writer.WriteStartObject();
        //    Limits.Serialize(writer);
        //    writer.WriteEndObject();

        //    // ListenOptions
        //    writer.WritePropertyName(nameof(ListenOptions));
        //    writer.WriteStartArray();
        //    foreach (var listenOptions in OptionsInUse)
        //    {
        //        writer.WriteStartObject();
        //        writer.WriteString("Address", listenOptions.GetDisplayName());
        //        writer.WritePropertyName(nameof(listenOptions.IsTls));
        //        writer.WriteBooleanValue(listenOptions.IsTls);
        //        writer.WriteString(nameof(listenOptions.Protocols), listenOptions.Protocols.ToString());
        //        writer.WriteEndObject();
        //    }
        //    writer.WriteEndArray();
        //}

        //private static X509Certificate2? GetDevelopmentCertificateFromStore(ILogger<NI2SEngine> logger)
        //{
        //    try
        //    {
        //        var certs = CertificateManager.Instance.ListCertificates(StoreName.My, StoreLocation.CurrentUser, isValid: true, requireExportable: false);

        //        var cert = certs.Count > 0 ? certs[0] : null;
        //        if (cert is null)
        //        {
        //            logger.UnableToLocateDevelopmentCertificate();
        //            return null;
        //        }

        //        var status = CertificateManager.Instance.CheckCertificateState(cert);
        //        if (!status.Success)
        //        {
        //            // Failure is only possible on MacOS and indicates that, if there is a dev cert, it must be from
        //            // a dotnet version prior to 7.0 - newer versions store it in such a way that this check succeeds.
        //            // (Success does not mean that the dev cert has been trusted).
        //            // In practice, success.FailureMessage will always be MacOSCertificateManager.InvalidCertificateState.
        //            // Basically, we're just going to encourage the user to generate and trust the dev cert.  We support
        //            // these older certificates not by accepting them as-is, but by modernizing them when dev-certs is run.
        //            // If we detect an issue here, we can avoid a UI prompt below.
        //            Debug.Assert(status.FailureMessage != null, "Status with a failure result must have a message.");
        //            logger.DeveloperCertificateFirstRun(status.FailureMessage);

        //            // Prevent binding to HTTPS if the certificate is not valid (avoid the prompt)
        //            return null;
        //        }

        //        // On MacOS, this may cause a UI prompt, since it requires accessing the keychain.  NI2SEngine must NEVER
        //        // cause a UI prompt on a production system. We only attempt this here because MacOS is not supported
        //        // in production.
        //        switch (CertificateManager.Instance.GetTrustLevel(cert))
        //        {
        //            case CertificateManager.TrustLevel.Partial:
        //                logger.DeveloperCertificatePartiallyTrusted();
        //                break;
        //            case CertificateManager.TrustLevel.None:
        //                logger.DeveloperCertificateNotTrusted();
        //                break;
        //        }

        //        return cert;
        //    }
        //    catch
        //    {
        //        logger.UnableToLocateDevelopmentCertificate();
        //        return null;
        //    }
        //}

        /// <summary>
        /// Creates a configuration loader for setting up NI2SEngine.
        /// </summary>
        /// <returns>A <see cref="EngineConfigurationLoader"/> for configuring endpoints.</returns>
        public EngineConfigurationLoader Configure() => Configure(new ConfigurationBuilder().Build());

        /// <summary>
        /// Creates a configuration loader for setting up NI2SEngine that takes an <see cref="IConfiguration"/> as input.
        /// This configuration must be scoped to the configuration section for NI2SEngine.
        /// Call <see cref="Configure(IConfiguration, bool)"/> to enable dynamic endpoint binding updates.
        /// </summary>
        /// <param name="config">The configuration section for NI2SEngine.</param>
        /// <returns>A <see cref="EngineConfigurationLoader"/> for further endpoint configuration.</returns>
        public EngineConfigurationLoader Configure(IConfiguration config) => Configure(config, reloadOnChange: false);

        /// <summary>
        /// Creates a configuration loader for setting up NI2SEngine that takes an <see cref="IConfiguration"/> as input.
        /// This configuration must be scoped to the configuration section for NI2SEngine.
        /// </summary>
        /// <param name="config">The configuration section for NI2SEngine.</param>
        /// <param name="reloadOnChange">
        /// If <see langword="true"/>, NI2SEngine will dynamically update endpoint bindings when configuration changes.
        /// This will only reload endpoints defined in the "Endpoints" section of your <paramref name="config"/>. Endpoints defined in code will not be reloaded.
        /// </param>
        /// <returns>A <see cref="EngineConfigurationLoader"/> for further endpoint configuration.</returns>
        public EngineConfigurationLoader Configure(IConfiguration config, bool reloadOnChange)
        {
            if (EngineServices is null)
            {
                throw new InvalidOperationException($"{nameof(EngineServices)} must not be null. This is normally set automatically via {nameof(IConfigureOptions<EngineOptions>)}.");
            }

            //var httpsConfigurationService = EngineServices.GetRequiredService<IHttpsConfigurationService>();
            //var certificatePathWatcher = reloadOnChange && !_disableCertificateFileWatching
            //    ? new CertificatePathWatcher(
            //        EngineServices.GetRequiredService<IHostEnvironment>(),
            //        EngineServices.GetRequiredService<ILogger<CertificatePathWatcher>>())
            //    : null;
            //var loader = new NI2SEngineConfigurationLoader(this, config, httpsConfigurationService, certificatePathWatcher, reloadOnChange);
            var loader = new EngineConfigurationLoader();
            ConfigurationLoader = loader;
            return loader;
        }

        ///// <summary>
        ///// Bind to the given IP address and port.
        ///// </summary>
        //public void Listen(IPAddress address, int port)
        //{
        //    Listen(address, port, _ => { });
        //}

        ///// <summary>
        ///// Bind to the given IP address and port.
        ///// The callback configures endpoint-specific settings.
        ///// </summary>
        //public void Listen(IPAddress address, int port, Action<ListenOptions> configure)
        //{
        //    ArgumentNullException.ThrowIfNull(address);

        //    Listen(new IPEndPoint(address, port), configure);
        //}

        ///// <summary>
        ///// Bind to the given IP endpoint.
        ///// </summary>
        //public void Listen(IPEndPoint endPoint)
        //{
        //    Listen((EndPoint)endPoint);
        //}

        ///// <summary>
        ///// Bind to the given endpoint.
        ///// </summary>
        ///// <param name="endPoint"></param>
        //public void Listen(EndPoint endPoint)
        //{
        //    Listen(endPoint, _ => { });
        //}

        ///// <summary>
        ///// Bind to the given IP address and port.
        ///// The callback configures endpoint-specific settings.
        ///// </summary>
        //public void Listen(IPEndPoint endPoint, Action<ListenOptions> configure)
        //{
        //    Listen((EndPoint)endPoint, configure);
        //}

        ///// <summary>
        ///// Bind to the given endpoint.
        ///// The callback configures endpoint-specific settings.
        ///// </summary>
        //public void Listen(EndPoint endPoint, Action<ListenOptions> configure)
        //{
        //    ArgumentNullException.ThrowIfNull(endPoint);
        //    ArgumentNullException.ThrowIfNull(configure);

        //    var listenOptions = new ListenOptions(endPoint);
        //    ApplyEndpointDefaults(listenOptions);
        //    configure(listenOptions);
        //    CodeBackedListenOptions.Add(listenOptions);
        //}

        ///// <summary>
        ///// Listens on ::1 and 127.0.0.1 with the given port. Requesting a dynamic port by specifying 0 is not supported
        ///// for this type of endpoint.
        ///// </summary>
        //public void ListenLocalhost(int port) => ListenLocalhost(port, options => { });

        ///// <summary>
        ///// Listens on ::1 and 127.0.0.1 with the given port. Requesting a dynamic port by specifying 0 is not supported
        ///// for this type of endpoint.
        ///// </summary>
        //public void ListenLocalhost(int port, Action<ListenOptions> configure)
        //{
        //    ArgumentNullException.ThrowIfNull(configure);

        //    var listenOptions = new LocalhostListenOptions(port);
        //    ApplyEndpointDefaults(listenOptions);
        //    configure(listenOptions);
        //    CodeBackedListenOptions.Add(listenOptions);
        //}

        ///// <summary>
        ///// Listens on all IPs using IPv6 [::], or IPv4 0.0.0.0 if IPv6 is not supported.
        ///// </summary>
        //public void ListenAnyIP(int port) => ListenAnyIP(port, options => { });

        ///// <summary>
        ///// Listens on all IPs using IPv6 [::], or IPv4 0.0.0.0 if IPv6 is not supported.
        ///// </summary>
        //public void ListenAnyIP(int port, Action<ListenOptions> configure)
        //{
        //    ArgumentNullException.ThrowIfNull(configure);

        //    var listenOptions = new AnyIPListenOptions(port);
        //    ApplyEndpointDefaults(listenOptions);
        //    configure(listenOptions);
        //    CodeBackedListenOptions.Add(listenOptions);
        //}

        ///// <summary>
        ///// Bind to the given Unix domain socket path.
        ///// </summary>
        //public void ListenUnixSocket(string socketPath)
        //{
        //    ListenUnixSocket(socketPath, _ => { });
        //}

        ///// <summary>
        ///// Bind to the given Unix domain socket path.
        ///// Specify callback to configure endpoint-specific settings.
        ///// </summary>
        //public void ListenUnixSocket(string socketPath, Action<ListenOptions> configure)
        //{
        //    ArgumentNullException.ThrowIfNull(socketPath);

        //    if (!Path.IsPathRooted(socketPath))
        //    {
        //        throw new ArgumentException(CoreStrings.UnixSocketPathMustBeAbsolute, nameof(socketPath));
        //    }
        //    ArgumentNullException.ThrowIfNull(configure);

        //    var listenOptions = new ListenOptions(socketPath);
        //    ApplyEndpointDefaults(listenOptions);
        //    configure(listenOptions);
        //    CodeBackedListenOptions.Add(listenOptions);
        //}

        ///// <summary>
        ///// Open a socket file descriptor.
        ///// </summary>
        //public void ListenHandle(ulong handle)
        //{
        //    ListenHandle(handle, _ => { });
        //}

        ///// <summary>
        ///// Open a socket file descriptor.
        ///// The callback configures endpoint-specific settings.
        ///// </summary>
        //public void ListenHandle(ulong handle, Action<ListenOptions> configure)
        //{
        //    ArgumentNullException.ThrowIfNull(configure);

        //    var listenOptions = new ListenOptions(handle);
        //    ApplyEndpointDefaults(listenOptions);
        //    configure(listenOptions);
        //    CodeBackedListenOptions.Add(listenOptions);
        //}

        ///// <summary>
        ///// Bind to the given named pipe.
        ///// </summary>
        //public void ListenNamedPipe(string pipeName)
        //{
        //    ListenNamedPipe(pipeName, _ => { });
        //}

        ///// <summary>
        ///// Bind to the given named pipe.
        ///// Specify callback to configure endpoint-specific settings.
        ///// </summary>
        //public void ListenNamedPipe(string pipeName, Action<ListenOptions> configure)
        //{
        //    ArgumentNullException.ThrowIfNull(pipeName);
        //    ArgumentNullException.ThrowIfNull(configure);

        //    var listenOptions = new ListenOptions(new NamedPipeEndPoint(pipeName));
        //    ApplyEndpointDefaults(listenOptions);
        //    configure(listenOptions);
        //    CodeBackedListenOptions.Add(listenOptions);
        //}
    }
}