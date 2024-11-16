using ARWNI2S.Engine.Features;
using ARWNI2S.Infrastructure;
using ARWNI2S.Infrastructure.Engine;
using ARWNI2S.Infrastructure.Engine.Features;
using ARWNI2S.Node.Core.Engine;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace ARWNI2S.Engine.Hosting.Internal
{
    /// <summary>
    /// Represents an implementation of the Engine Context class.
    /// </summary>
    internal sealed class DefaultEngineContext : EngineContext
    {
        // The initial size of the feature collection when using the default constructor; based on number of common features
        // https://github.com/dotnet/aspnetcore/issues/31249
        private const int DefaultFeatureCollectionSize = 10;

        //// Lambdas hoisted to static readonly fields to improve inlining https://github.com/dotnet/roslyn/issues/13624
        ////    //private static readonly Func<IFeatureCollection, IItemsFeature> _newItemsFeature = f => new ItemsFeature();
        private static readonly Func<DefaultEngineContext, IServiceProvidersFeature> _newServiceProvidersFeature = context => new EngineServicesFeature(context, context.ServiceScopeFactory);
        //private static readonly Func<IFeatureCollection, IAuthenticationFeature> _newAuthenticationFeature = f => new CoreAuthenticationFeature();
        //private static readonly Func<IFeatureCollection, IFrameLifetimeFeature> _newFrameLifetimeFeature = f => new FrameLifetimeFeature();
        ////    //private static readonly Func<IFeatureCollection, ISessionFeature> _newSessionFeature = f => new DefaultSessionFeature();
        ////    //private static readonly Func<IFeatureCollection, ISessionFeature?> _nullSessionFeature = f => null;
        private static readonly Func<IFeatureCollection, IFrameIdentifierFeature> _newFrameIdentifierFeature = f => new FrameIdentifierFeature();

        private FeatureReferences<FeatureInterfaces> _features;
        private IEvent _callback;
        ////private DefaultConnection _connection;

        // This is field exists to make analyzing memory dumps easier.
        // https://github.com/dotnet/aspnetcore/issues/29709
        internal bool _active;

        /// <inheritdoc/>
        public override IFeatureCollection Features => _features.Collection ?? ContextDisposed();

        ///// <inheritdoc/>
        //public override IConnection Connection => throw new NotImplementedException();
        ////public override IConnection Connection => _connection ?? (_connection = new DefaultConnection(Features));

        /// <inheritdoc/>
        public override IServiceProvider EngineServices
        {
            get { return ServiceProvidersFeature.EngineServices; }
            set { ServiceProvidersFeature.EngineServices = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="IServiceScopeFactory" /> for this instance.
        /// </summary>
        /// <returns>
        /// <see cref="IServiceScopeFactory"/>
        /// </returns>
        public IServiceScopeFactory ServiceScopeFactory { get; set; } = default!;

        /// <inheritdoc/>
        public override IEvent Callback => _callback ??= new NullCallback();

        /// <inheritdoc/>
        public override string TraceIdentifier
        {
            get { return FrameIdentifierFeature.TraceIdentifier; }
            set { FrameIdentifierFeature.TraceIdentifier = value; }
        }

        ///// <inheritdoc/>
        //public override ClaimsPrincipal User
        //{
        //    get
        //    {
        //        var user = AuthenticationFeature.User;
        //        if (user == null)
        //        {
        //            user = new ClaimsPrincipal(new ClaimsIdentity());
        //            AuthenticationFeature.User = user;
        //        }
        //        return user;
        //    }
        //    set { AuthenticationFeature.User = value; }
        //}

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultEngineContext"/> class.
        /// </summary>
        public DefaultEngineContext()
            : this(new FeatureCollection(DefaultFeatureCollectionSize))
        {
            //Features.Set<IFrameFeature>(new FrameFeature());
            //Features.Set<IResponseFeature>(new ResponseFeature());
            //Features.Set<IResponseBodyFeature>(new StreamResponseBodyFeature(Stream.Null));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultEngineContext"/> class with provided features.
        /// </summary>
        /// <param name="features">Initial set of features for the <see cref="DefaultEngineContext"/>.</param>
        public DefaultEngineContext(IFeatureCollection features)
        {
            //_features.Initalize(features);
            //_request = new DefaultFrame(this);
            //_response = new DefaultResponse(this);
        }

        /// <summary>
        /// Reinitialize  the current instant of the class with features passed in.
        /// </summary>
        /// <remarks>
        /// This method allows the consumer to re-use the <see cref="DefaultEngineContext" /> for another request, rather than having to allocate a new instance.
        /// </remarks>
        /// <param name="features">The new set of features for the <see cref="DefaultEngineContext" />.</param>
        public void Initialize(IFeatureCollection features)
        {
            var revision = features.Revision;
            _features.Initalize(features, revision);
            //_request.Initialize(revision);
            //_response.Initialize(revision);
            //_connection?.Initialize(features, revision);
            //_websockets?.Initialize(features, revision);
            _active = true;
        }

        /// <summary>
        /// Uninitialize all the features in the <see cref="DefaultEngineContext" />.
        /// </summary>
        public void Uninitialize()
        {
            _features = default;
            //_request.Uninitialize();
            //_response.Uninitialize();
            //_connection?.Uninitialize();
            //_websockets?.Uninitialize();
            _active = false;
        }

        private IServiceProvidersFeature ServiceProvidersFeature =>
            _features.Fetch(ref _features.Cache.ServiceProviders, this, _newServiceProvidersFeature)!;
        private IFrameIdentifierFeature FrameIdentifierFeature =>
            _features.Fetch(ref _features.Cache.FrameIdentifier, _newFrameIdentifierFeature)!;

        //private IAuthenticationFeature AuthenticationFeature =>
        //    _features.Fetch(ref _features.Cache.Authentication, _newAuthenticationFeature)!;
        //private IFrameLifetimeFeature LifetimeFeature =>
        //    _features.Fetch(ref _features.Cache.Lifetime, _newFrameLifetimeFeature)!;






        //public override IEvent Event => throw new NotImplementedException();


        //public override IDictionary<object, object> Items { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public override CancellationToken FrameAborted { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public override ISession Session { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        // This property exists because of backwards compatibility.
        // We send an anonymous object with an EngineContext property
        // via DiagnosticListener in various events throughout the pipeline. Instead
        // we just send the EngineContext to avoid extra allocations
        /// <summary>
        /// This API is used by ASP.NET Core's infrastructure and should not be used by application code.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public EngineContext EngineContext => this;



        ///// <inheritdoc/>
        //public override void Abort()
        //{
        //    LifetimeFeature.Abort();
        //}

        private static IFeatureCollection ContextDisposed()
        {
            ThrowContextDisposed();
            return null;
        }

        [DoesNotReturn]
        private static void ThrowContextDisposed()
        {
            throw new ObjectDisposedException(nameof(EngineContext), $"Frame has finished and {nameof(EngineContext)} disposed.");
        }

        struct FeatureInterfaces
        {
            //public IItemsFeature? Items;
            public IServiceProvidersFeature ServiceProviders;
            //public IAuthenticationFeature Authentication;
            //public IFrameLifetimeFeature Lifetime;
            //public ISessionFeature? Session;
            public IFrameIdentifierFeature FrameIdentifier;
        }
    }

    //{



    //    //private readonly DefaultFrame _request;
    //    //private readonly DefaultResponse _response;

    //    //private DefaultWebSocketManager? _websockets;




    //    /// <summary>
    //    /// Gets or set the <see cref="FormOptions" /> for this instance.
    //    /// </summary>
    //    /// <returns>
    //    /// <see cref="FormOptions"/>
    //    /// </returns>
    //    public FormOptions FormOptions { get; set; } = default!;


    //    //private IItemsFeature ItemsFeature =>
    //    //    _features.Fetch(ref _features.Cache.Items, _newItemsFeature)!;




    //    //private ISessionFeature SessionFeature =>
    //    //    _features.Fetch(ref _features.Cache.Session, _newSessionFeature)!;

    //    //private ISessionFeature? SessionFeatureOrNull =>
    //    //    _features.Fetch(ref _features.Cache.Session, _nullSessionFeature);


    //    ///// <inheritdoc/>
    //    //public override Frame Frame => _request;

    //    ///// <inheritdoc/>
    //    //public override Response Response => _response;


    //    /// <inheritdoc/>
    //    public override WebSocketManager WebSockets => _websockets ?? (_websockets = new DefaultWebSocketManager(Features));


    //    /// <inheritdoc/>
    //    public override IDictionary<object, object?> Items
    //    {
    //        get { return ItemsFeature.Items; }
    //        set { ItemsFeature.Items = value; }
    //    }


    //    /// <inheritdoc/>
    //    public override CancellationToken FrameAborted
    //    {
    //        get { return LifetimeFeature.FrameAborted; }
    //        set { LifetimeFeature.FrameAborted = value; }
    //    }

    //    /// <inheritdoc/>
    //    public override string TraceIdentifier
    //    {
    //        get { return FrameIdentifierFeature.TraceIdentifier; }
    //        set { FrameIdentifierFeature.TraceIdentifier = value; }
    //    }

    //    /// <inheritdoc/>
    //    public override ISession Session
    //    {
    //        get
    //        {
    //            var feature = SessionFeatureOrNull;
    //            if (feature == null)
    //            {
    //                throw new InvalidOperationException("Session has not been configured for this application " +
    //                    "or request.");
    //            }
    //            return feature.Session;
    //        }
    //        set
    //        {
    //            SessionFeature.Session = value;
    //        }
    //    }





    //    private string DebuggerToString()
    //    {
    //        // DebuggerToString is also on this type because this project has access to ReasonPhrases.
    //        return EngineContextDebugFormatter.ContextToString(this, ReasonPhrases.GetReasonPhrase(Response.StatusCode));
    //    }

    //}
}