using ARWNI2S.Infrastructure.Engine;
using ARWNI2S.Infrastructure.Engine.Features;
using Microsoft.Extensions.DependencyInjection;

namespace ARWNI2S.Engine.Features
{
    /// <summary>
    /// An implementation for <see cref="IServiceProvidersFeature"/> for accessing request services.
    /// </summary>
    public class EngineServicesFeature : IServiceProvidersFeature, IDisposable, IAsyncDisposable
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private IServiceProvider _requestServices;
        private IServiceScope _scope;
        private bool _requestServicesSet;
        private readonly EngineContext _context;

        /// <summary>
        /// Initializes a new instance of <see cref="EngineServicesFeature"/>.
        /// </summary>
        /// <param name="context">The <see cref="EngineContext"/>.</param>
        /// <param name="scopeFactory">The <see cref="IServiceScopeFactory"/>.</param>
        public EngineServicesFeature(EngineContext context, IServiceScopeFactory scopeFactory)
        {
            _context = context;
            _scopeFactory = scopeFactory;
        }

        /// <inheritdoc />
        public IServiceProvider EngineServices
        {
            get
            {
                if (!_requestServicesSet && _scopeFactory != null)
                {
                    //_context.Callback.RegisterForDisposeAsync(this);
                    _scope = _scopeFactory.CreateScope();
                    _requestServices = _scope.ServiceProvider;
                    _requestServicesSet = true;
                }
                return _requestServices!;
            }

            set
            {
                _requestServices = value;
                _requestServicesSet = true;
            }
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            switch (_scope)
            {
                case IAsyncDisposable asyncDisposable:
                    var vt = asyncDisposable.DisposeAsync();
                    if (!vt.IsCompletedSuccessfully)
                    {
                        return Awaited(this, vt);
                    }
                    // If its a IValueTaskSource backed ValueTask,
                    // inform it its result has been read so it can reset
                    vt.GetAwaiter().GetResult();
                    break;
                case IDisposable disposable:
                    disposable.Dispose();
                    break;
            }

            _scope = null;
            _requestServices = null;

            return default;

            static async ValueTask Awaited(EngineServicesFeature servicesFeature, ValueTask vt)
            {
                await vt;
                servicesFeature._scope = null;
                servicesFeature._requestServices = null;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            DisposeAsync().AsTask().GetAwaiter().GetResult();
        }
    }
}