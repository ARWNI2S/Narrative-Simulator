using ARWNI2S.Infrastructure;
using ARWNI2S.Infrastructure.Collections;
using ARWNI2S.Infrastructure.Engine;
using ARWNI2S.Infrastructure.Engine.Builder;
using ARWNI2S.Node.Core.Engine;

namespace ARWNI2S.Engine.Hosting.Internal
{
    internal sealed class EngineBuilder : IEngineBuilder
    {
        private const string ServerFeaturesKey = "server.Features";
        private const string EngineServicesKey = "engine.Services";
        private const string MiddlewareDescriptionsKey = "__MiddlewareDescriptions";
        private const string RequestUnhandledKey = "__RequestUnhandled";

        private readonly List<Func<FrameDelegate, FrameDelegate>> _components = [];
        private readonly List<string> _descriptions;
        private readonly IDebugger _debugger;

        /// <summary>
        /// Initializes a new instance of <see cref="EngineBuilder"/>.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> for engine services.</param>
        public EngineBuilder(IServiceProvider serviceProvider) : this(serviceProvider, new FeatureCollection())
        {
        }

        private int MiddlewareCount => _components.Count;

        /// <summary>
        /// Initializes a new instance of <see cref="EngineBuilder"/>.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> for engine services.</param>
        /// <param name="server">The server instance that hosts the engine.</param>
        public EngineBuilder(IServiceProvider serviceProvider, object server)
        {
            Properties = new Dictionary<string, object>(StringComparer.Ordinal);
            EngineServices = serviceProvider;

            SetProperty(ServerFeaturesKey, server);

            // IDebugger service can optionally be added by tests to simulate the debugger being attached.
            _debugger = (IDebugger)serviceProvider?.GetService(typeof(IDebugger)) ?? DebuggerWrapper.Instance;

            if (_debugger.IsAttached)
            {
                _descriptions = [];
                // Add component descriptions collection to properties so debugging tools can display
                // a list of configured middleware for a engine.
                SetProperty(MiddlewareDescriptionsKey, _descriptions);
            }
        }

        private EngineBuilder(EngineBuilder builder)
        {
            Properties = new CopyOnWriteDictionary<string, object>(builder.Properties, StringComparer.Ordinal);
            _debugger = builder._debugger;
            if (_debugger.IsAttached)
            {
                _descriptions = [];
            }
        }

        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> for engine services.
        /// </summary>
        public IServiceProvider EngineServices
        {
            get
            {
                return GetProperty<IServiceProvider>(EngineServicesKey)!;
            }
            set
            {
                SetProperty(EngineServicesKey, value);
            }
        }

        ///// <summary>
        ///// Gets the <see cref="IFeatureCollection"/> for server features.
        ///// </summary>
        ///// <remarks>
        ///// An empty collection is returned if a server wasn't specified for the engine builder.
        ///// </remarks>
        //public IFeatureCollection ServerFeatures
        //{
        //    get
        //    {
        //        return GetProperty<IFeatureCollection>(ServerFeaturesKey)!;
        //    }
        //}

        /// <summary>
        /// Gets a set of properties for <see cref="EngineBuilder"/>.
        /// </summary>
        public IDictionary<string, object> Properties { get; }

        private T GetProperty<T>(string key)
        {
            return Properties.TryGetValue(key, out var value) ? (T)value : default;
        }

        private void SetProperty<T>(string key, T value)
        {
            Properties[key] = value;
        }

        /// <summary>
        /// Adds the middleware to the engine request pipeline.
        /// </summary>
        /// <param name="middleware">The middleware.</param>
        /// <returns>An instance of <see cref="IEngineBuilder"/> after the operation has completed.</returns>
        public IEngineBuilder Use(Func<FrameDelegate, FrameDelegate> middleware)
        {
            _components.Add(middleware);
            _descriptions?.Add(CreateMiddlewareDescription(middleware));

            return this;
        }

        private static string CreateMiddlewareDescription(Func<FrameDelegate, FrameDelegate> middleware)
        {
            if (middleware.Target != null)
            {
                // To IEngineBuilder, middleware is just a func. Getting a good description is hard.
                // Inspect the incoming func and attempt to resolve it back to a middleware type if possible.
                // UseMiddlewareExtensions adds middleware via a method with the name CreateMiddleware.
                // If this pattern is matched, then ToString on the target returns the middleware type name.
                if (middleware.Method.Name == "CreateMiddleware")
                {
                    return middleware.Target.ToString()!;
                }

                return middleware.Target.GetType().FullName + "." + middleware.Method.Name;
            }

            return middleware.Method.Name.ToString();
        }

        /// <summary>
        /// Creates a copy of this engine builder.
        /// <para>
        /// The created clone has the same properties as the current instance, but does not copy
        /// the request pipeline.
        /// </para>
        /// </summary>
        /// <returns>The cloned instance.</returns>
        public IEngineBuilder New()
        {
            return new EngineBuilder(this);
        }

        /// <summary>
        /// Produces a <see cref="FrameDelegate"/> that executes added middlewares.
        /// </summary>
        /// <returns>The <see cref="FrameDelegate"/>.</returns>
        public FrameDelegate Build()
        {
            FrameDelegate engine = context =>
            {
                //// If we reach the end of the pipeline, but we have an endpoint, then something unexpected has happened.
                //// This could happen if user code sets an endpoint, but they forgot to add the UseEndpoint middleware.
                //var endpoint = context.GetEndpoint();
                //var endpointRequestDelegate = endpoint?.RequestDelegate;
                //if (endpointRequestDelegate != null)
                //{
                //    var message =
                //        $"The request reached the end of the pipeline without executing the endpoint: '{endpoint!.DisplayName}'. " +
                //        $"Please register the EndpointMiddleware using '{nameof(IEngineBuilder)}.UseEndpoints(...)' if using " +
                //        $"routing.";
                //    throw new InvalidOperationException(message);
                //}

                //// Flushing the response and calling through to the next middleware in the pipeline is
                //// a user error, but don't attempt to set the status code if this happens. It leads to a confusing
                //// behavior where the client response looks fine, but the server side logic results in an exception.
                //if (!context.Response.HasStarted)
                //{
                //    context.Response.StatusCode = StatusCodes.Status404NotFound;
                //}

                //// Communicates to higher layers that the request wasn't handled by the engine pipeline.
                //context.Items[RequestUnhandledKey] = true;

                return Task.CompletedTask;
            };

            for (var c = _components.Count - 1; c >= 0; c--)
            {
                engine = _components[c](engine);
            }

            return engine;
        }
    }
}
