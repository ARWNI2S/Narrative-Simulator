using ARWNI2S.Engine.Features;
using ARWNI2S.Engine.Hosting.Diagnostics;
using ARWNI2S.Engine.Hosting.Internal;
using ARWNI2S.Infrastructure.Engine;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ARWNI2S.Engine.Hosting
{
    internal sealed class HostingEngine : IEngine<HostingEngine.Context>
    {
        private readonly UpdateDelegate _engine;
        private readonly IEngineContextFactory _engineContextFactory;
        private readonly DefaultEngineContextFactory _defaultEngineContextFactory;
        private readonly HostingEngineDiagnostics _diagnostics;

        public HostingEngine(
            UpdateDelegate engine,
            ILogger logger,
            DiagnosticListener diagnosticSource,
            ActivitySource activitySource,
            DistributedContextPropagator propagator,
            IEngineContextFactory engineContextFactory,
            HostingEngineEventSource eventSource,
            HostingEngineMetrics metrics)
        {
            _engine = engine;
            _diagnostics = new HostingEngineDiagnostics(logger, diagnosticSource, activitySource, propagator, eventSource, metrics);
            if (engineContextFactory is DefaultEngineContextFactory factory)
            {
                _defaultEngineContextFactory = factory;
            }
            else
            {
                _engineContextFactory = engineContextFactory;
            }
        }

        // Set up the frame
        public Context CreateContext(IFeatureCollection contextFeatures)
        {
            Context hostContext;
            if (contextFeatures is IHostContextContainer<Context> container)
            {
                hostContext = container.HostContext;
                if (hostContext is null)
                {
                    hostContext = new Context();
                    container.HostContext = hostContext;
                }
            }
            else
            {
                // Server doesn't support pooling, so create a new Context
                hostContext = new Context();
            }

            EngineContext engineContext;
            if (_defaultEngineContextFactory != null)
            {
                var defaultEngineContext = (DefaultEngineContext)hostContext.EngineContext;
                if (defaultEngineContext is null)
                {
                    engineContext = _defaultEngineContextFactory.Create(contextFeatures);
                    hostContext.EngineContext = engineContext;
                }
                else
                {
                    _defaultEngineContextFactory.Initialize(defaultEngineContext, contextFeatures);
                    engineContext = defaultEngineContext;
                }
            }
            else
            {
                engineContext = _engineContextFactory!.Create(contextFeatures);
                hostContext.EngineContext = engineContext;
            }

            _diagnostics.BeginFrame(engineContext, hostContext);
            return hostContext;
        }

        // Execute the frame
        public Task ProcessFrameAsync(Context context)
        {
            return _engine(context.EngineContext!);
        }

        // Clean up the frame
        public void DisposeContext(Context context, Exception exception)
        {
            var engineContext = context.EngineContext!;
            _diagnostics.FrameEnd(engineContext, exception, context);

            if (_defaultEngineContextFactory != null)
            {
                _defaultEngineContextFactory.Dispose((DefaultEngineContext)engineContext);

                if (_defaultEngineContextFactory.EngineContextAccessor != null)
                {
                    // Clear the EngineContext if the accessor was used. It's likely that the lifetime extends
                    // past the end of the http request and we want to avoid changing the reference from under
                    // consumers.
                    context.EngineContext = null;
                }
            }
            else
            {
                _engineContextFactory!.Dispose(engineContext);
            }

            _diagnostics.ContextDisposed(context);

            // Reset the context as it may be pooled
            context.Reset();
        }

        internal sealed class Context
        {
            public EngineContext EngineContext { get; set; }
            public IDisposable Scope { get; set; }
            public Activity Activity
            {
                get => ActivityFeature?.Activity;
                set
                {
                    if (ActivityFeature is null)
                    {
                        if (value != null)
                        {
                            ActivityFeature = new CoreActivityFeature(value);
                        }
                    }
                    else
                    {
                        ActivityFeature.Activity = value!;
                    }
                }
            }
            //internal HostingFrameStartingLog StartLog { get; set; }

            public long StartTimestamp { get; set; }
            internal bool HasDiagnosticListener { get; set; }
            public bool MetricsEnabled { get; set; }
            public bool EventLogEnabled { get; set; }

            internal CoreActivityFeature ActivityFeature;
            internal CoreMetricsTagsFeature MetricsTagsFeature;

            public void Reset()
            {
                // Not resetting EngineContext here as we pool it on the Context

                Scope = null;
                Activity = null;
                //StartLog = null;

                StartTimestamp = 0;
                HasDiagnosticListener = false;
                MetricsEnabled = false;
                EventLogEnabled = false;
                MetricsTagsFeature?.Reset();
            }
        }
    }
}