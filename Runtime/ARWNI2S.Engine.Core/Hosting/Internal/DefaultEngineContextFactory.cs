using ARWNI2S.Infrastructure.Engine;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ARWNI2S.Engine.Hosting.Internal
{
    /// <summary>
    /// A factory for creating <see cref="EngineContext" /> instances.
    /// </summary>
    internal class DefaultEngineContextFactory : IEngineContextFactory
    {
        private readonly IEngineContextAccessor _engineContextAccessor;
        //private readonly FormOptions _formOptions;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        // This takes the IServiceProvider because it needs to support an ever expanding
        // set of services that flow down into EngineContext features
        /// <summary>
        /// Creates a factory for creating <see cref="EngineContext" /> instances.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> to be used when retrieving services.</param>
        public DefaultEngineContextFactory(IServiceProvider serviceProvider)
        {
            // May be null
            _engineContextAccessor = serviceProvider.GetService<IEngineContextAccessor>();
            //_formOptions = serviceProvider.GetRequiredService<IOptions<FormOptions>>().Value;
            _serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
        }

        internal IEngineContextAccessor EngineContextAccessor => _engineContextAccessor;

        /// <summary>
        /// Create an <see cref="EngineContext"/> instance given an <paramref name="featureCollection" />.
        /// </summary>
        /// <param name="featureCollection"></param>
        /// <returns>An initialized <see cref="EngineContext"/> object.</returns>
        public EngineContext Create(IFeatureCollection featureCollection)
        {
            ArgumentNullException.ThrowIfNull(featureCollection);

            var engineContext = new DefaultEngineContext(featureCollection);
            Initialize(engineContext, featureCollection);
            return engineContext;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Initialize(DefaultEngineContext engineContext, IFeatureCollection featureCollection)
        {
            Debug.Assert(featureCollection != null);
            Debug.Assert(engineContext != null);

            engineContext.Initialize(featureCollection);

            if (_engineContextAccessor != null)
            {
                _engineContextAccessor.EngineContext = engineContext;
            }

            //engineContext.FormOptions = _formOptions;
            engineContext.ServiceScopeFactory = _serviceScopeFactory;
        }

        /// <summary>
        /// Clears the current <see cref="EngineContext" />.
        /// </summary>
        public void Dispose(EngineContext engineContext)
        {
            if (_engineContextAccessor != null)
            {
                _engineContextAccessor.EngineContext = null;
            }
        }

        internal void Dispose(DefaultEngineContext engineContext)
        {
            if (_engineContextAccessor != null)
            {
                _engineContextAccessor.EngineContext = null;
            }

            engineContext.Uninitialize();
        }
    }
}