using ARWNI2S.Engine.Features;
using ARWNI2S.Infrastructure.Engine;
using ARWNI2S.Infrastructure.Engine.Features;
using Microsoft.Extensions.Logging;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace ARWNI2S.Engine.Hosting.Diagnostics
{
    internal sealed class HostingEngineDiagnostics
    {
        // internal so it can be used in tests
        internal const string ActivityName = "ARWNI2S.Node.Hosting.HttpFrameIn";
        private const string ActivityStartKey = ActivityName + ".Start";
        private const string ActivityStopKey = ActivityName + ".Stop";

        private const string DeprecatedDiagnosticsBeginFrameKey = "ARWNI2S.Node.Hosting.BeginFrame";
        private const string DeprecatedDiagnosticsEndFrameKey = "ARWNI2S.Node.Hosting.EndFrame";
        private const string DiagnosticsUnhandledExceptionKey = "ARWNI2S.Node.Hosting.UnhandledException";

        private const string FrameUnhandledKey = "__FrameUnhandled";

        private readonly ActivitySource _activitySource;
        private readonly DiagnosticListener _diagnosticListener;
        private readonly DistributedContextPropagator _propagator;
        private readonly HostingEngineEventSource _eventSource;
        private readonly HostingEngineMetrics _metrics;
        private readonly ILogger _logger;

        public HostingEngineDiagnostics(
            ILogger logger,
            DiagnosticListener diagnosticListener,
            ActivitySource activitySource,
            DistributedContextPropagator propagator,
            HostingEngineEventSource eventSource,
            HostingEngineMetrics metrics)
        {
            _logger = logger;
            _diagnosticListener = diagnosticListener;
            _activitySource = activitySource;
            _propagator = propagator;
            _eventSource = eventSource;
            _metrics = metrics;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BeginFrame(EngineContext engineContext, HostingEngine.Context context)
        {
            long startTimestamp = 0;

            if (_metrics.IsEnabled())
            {
                context.MetricsEnabled = true;
                context.MetricsTagsFeature ??= new CoreMetricsTagsFeature();
                engineContext.Features.Set<IMetricsTagsFeature>(context.MetricsTagsFeature);

                //context.MetricsTagsFeature.Method = engineContext.Event.Method;
                //context.MetricsTagsFeature.Protocol = engineContext.Event.Protocol;
                //context.MetricsTagsFeature.Scheme = engineContext.Event.Scheme;

                startTimestamp = Stopwatch.GetTimestamp();

                // To keep the hot path short we defer logging in this function to non-inlines
                RecordFrameStartMetrics(engineContext);
            }

            if (_eventSource.IsEnabled())
            {
                context.EventLogEnabled = true;

                if (startTimestamp == 0)
                {
                    startTimestamp = Stopwatch.GetTimestamp();
                }

                // To keep the hot path short we defer logging in this function to non-inlines
                RecordFrameStartEventLog(engineContext);
            }

            var diagnosticListenerEnabled = _diagnosticListener.IsEnabled();
            var diagnosticListenerActivityCreationEnabled = diagnosticListenerEnabled && _diagnosticListener.IsEnabled(ActivityName, engineContext);
            var loggingEnabled = _logger.IsEnabled(LogLevel.Critical);

            if (loggingEnabled || diagnosticListenerActivityCreationEnabled || _activitySource.HasListeners())
            {
                context.Activity = StartActivity(engineContext, loggingEnabled, diagnosticListenerActivityCreationEnabled, out var hasDiagnosticListener);
                context.HasDiagnosticListener = hasDiagnosticListener;

                if (context.Activity != null)
                {
                    engineContext.Features.Set<IActivityFeature>(context.ActivityFeature);
                }
            }

            if (diagnosticListenerEnabled)
            {
                if (_diagnosticListener.IsEnabled(DeprecatedDiagnosticsBeginFrameKey))
                {
                    if (startTimestamp == 0)
                    {
                        startTimestamp = Stopwatch.GetTimestamp();
                    }

                    RecordBeginFrameDiagnostics(engineContext, startTimestamp);
                }
            }

            // To avoid allocation, return a null scope if the logger is not on at least to some degree.
            if (loggingEnabled)
            {
                // Scope may be relevant for a different level of logging, so we always create it
                // see: https://github.com/aspnet/Hosting/pull/944
                // Scope can be null if logging is not on.
                context.Scope = Log.FrameScope(_logger, engineContext);

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    if (startTimestamp == 0)
                    {
                        startTimestamp = Stopwatch.GetTimestamp();
                    }

                    // Non-inline
                    LogFrameStarting(context);
                }
            }
            context.StartTimestamp = startTimestamp;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FrameEnd(EngineContext engineContext, Exception exception, HostingEngine.Context context)
        {
            // Local cache items resolved multiple items, in order of use so they are primed in cpu pipeline when used
            var startTimestamp = context.StartTimestamp;
            long currentTimestamp = 0;

            // startTimestamp has a value if:
            // - Information logging was enabled at for this frame (and calculated time will be wildly wrong)
            //   Is used as proxy to reduce calls to virtual: _logger.IsEnabled(LogLevel.Information)
            // - EventLog or metrics was enabled
            if (startTimestamp != 0)
            {
                currentTimestamp = Stopwatch.GetTimestamp();
                var reachedPipelineEnd = engineContext.Items.ContainsKey(FrameUnhandledKey);

                // Non-inline
                LogFrameFinished(context, startTimestamp, currentTimestamp);

                if (context.MetricsEnabled)
                {
                    Debug.Assert(context.MetricsTagsFeature != null, "MetricsTagsFeature should be set if MetricsEnabled is true.");

                    //var endpoint = HttpExtensions.GetOriginalEndpoint(engineContext);
                    //var disableFrameDurationMetric = endpoint?.Metadata.GetMetadata<IDisableMetricsMetadata>() != null || context.MetricsTagsFeature.MetricsDisabled;
                    //var route = endpoint?.Metadata.GetMetadata<IRouteDiagnosticsMetadata>()?.Route;

                    //_metrics.FrameEnd(
                    //    context.MetricsTagsFeature.Protocol!,
                    //    context.MetricsTagsFeature.Scheme!,
                    //    context.MetricsTagsFeature.Method!,
                    //    route,
                    //    engineContext.Event.StatusCode,
                    //    reachedPipelineEnd,
                    //    exception,
                    //    context.MetricsTagsFeature.TagsList,
                    //    startTimestamp,
                    //    currentTimestamp,
                    //    disableFrameDurationMetric);
                }

                if (reachedPipelineEnd)
                {
                    LogFrameUnhandled(context);
                }
            }

            if (_diagnosticListener.IsEnabled())
            {
                if (currentTimestamp == 0)
                {
                    currentTimestamp = Stopwatch.GetTimestamp();
                }

                if (exception == null)
                {
                    // No exception was thrown, frame was successful
                    if (_diagnosticListener.IsEnabled(DeprecatedDiagnosticsEndFrameKey))
                    {
                        // Diagnostics is enabled for EndFrame, but it may not be for BeginFrame
                        // so call GetTimestamp if currentTimestamp is zero (from above)
                        RecordEndFrameDiagnostics(engineContext, currentTimestamp);
                    }
                }
                else
                {
                    // Exception was thrown from frame
                    if (_diagnosticListener.IsEnabled(DiagnosticsUnhandledExceptionKey))
                    {
                        // Diagnostics is enabled for UnhandledException, but it may not be for BeginFrame
                        // so call GetTimestamp if currentTimestamp is zero (from above)
                        RecordUnhandledExceptionDiagnostics(engineContext, currentTimestamp, exception);
                    }
                }
            }

            var activity = context.Activity;
            // Always stop activity if it was started.
            // The HTTP activity must be stopped after the HTTP frame duration metric is recorded.
            // This order means the activity is ongoing while the metric is recorded and libraries like OTEL
            // can capture the activity as a metric exemplar.
            if (activity is not null)
            {
                StopActivity(engineContext, activity, context.HasDiagnosticListener);
            }

            if (context.EventLogEnabled)
            {
                if (exception != null)
                {
                    // Non-inline
                    _eventSource.UnhandledException();
                }

                // Count 500 as failed frames
                //if (engineContext.Event.StatusCode >= 500)
                //{
                //    _eventSource.FrameFailed();
                //}
            }

            // Logging Scope is finshed with
            context.Scope?.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ContextDisposed(HostingEngine.Context context)
        {
            if (context.EventLogEnabled)
            {
                //_eventSource.FrameStop();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void LogFrameStarting(HostingEngine.Context context)
        {
            // IsEnabled is checked in the caller, so if we are here just log
            var startLog = new HostingFrameStartingLog(context.EngineContext!);
            context.StartLog = startLog;

            //_logger.Log(
            //    logLevel: LogLevel.Information,
            //    eventId: LoggerEventIds.FrameStarting,
            //    state: startLog,
            //    exception: null,
            //    formatter: HostingFrameStartingLog.Callback);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void LogFrameFinished(HostingEngine.Context context, long startTimestamp, long currentTimestamp)
        {
            // IsEnabled isn't checked in the caller, startTimestamp > 0 is used as a fast proxy check
            // but that may be because diagnostics are enabled, which also uses startTimestamp,
            // so check if we logged the start event
            if (context.StartLog != null)
            {
                var elapsed = Stopwatch.GetElapsedTime(startTimestamp, currentTimestamp);

                //_logger.Log(
                //    logLevel: LogLevel.Information,
                //    eventId: LoggerEventIds.FrameFinished,
                //    state: new HostingFrameFinishedLog(context, elapsed),
                //    exception: null,
                //    formatter: HostingFrameFinishedLog.Callback);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void LogFrameUnhandled(HostingEngine.Context context)
        {
            //_logger.Log(
            //    logLevel: LogLevel.Information,
            //    eventId: LoggerEventIds.FrameUnhandled,
            //    state: new HostingFrameUnhandledLog(context.EngineContext!),
            //    exception: null,
            //    formatter: HostingFrameUnhandledLog.Callback);
        }

        [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026",
            Justification = "The values being passed into Write have the commonly used properties being preserved with DynamicDependency.")]
        private static void WriteDiagnosticEvent<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] TValue>(
            DiagnosticSource diagnosticSource, string name, TValue value)
        {
            diagnosticSource.Write(name, value);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void RecordBeginFrameDiagnostics(EngineContext engineContext, long startTimestamp)
        {
            WriteDiagnosticEvent(
                _diagnosticListener,
                DeprecatedDiagnosticsBeginFrameKey,
                new DeprecatedFrameData(engineContext, startTimestamp));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void RecordEndFrameDiagnostics(EngineContext engineContext, long currentTimestamp)
        {
            WriteDiagnosticEvent(
                _diagnosticListener,
                DeprecatedDiagnosticsEndFrameKey,
                new DeprecatedFrameData(engineContext, currentTimestamp));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void RecordUnhandledExceptionDiagnostics(EngineContext engineContext, long currentTimestamp, Exception exception)
        {
            WriteDiagnosticEvent(
                _diagnosticListener,
                DiagnosticsUnhandledExceptionKey,
                new UnhandledExceptionData(engineContext, currentTimestamp, exception));
        }

        private sealed class DeprecatedFrameData
        {
            // Common properties. Properties not in this list could be trimmed.
            //[DynamicDependency(nameof(EngineContext.Frame), typeof(EngineContext))]
            //[DynamicDependency(nameof(EngineContext.Response), typeof(EngineContext))]
            //[DynamicDependency(nameof(HttpFrame.Path), typeof(HttpFrame))]
            //[DynamicDependency(nameof(HttpFrame.Method), typeof(HttpFrame))]
            //[DynamicDependency(nameof(HttpResponse.StatusCode), typeof(HttpResponse))]
            internal DeprecatedFrameData(EngineContext engineContext, long timestamp)
            {
                this.engineContext = engineContext;
                this.timestamp = timestamp;
            }

            // Compatibility with anonymous object property names
            public EngineContext engineContext { get; }
            public long timestamp { get; }

            public override string ToString() => $"{{ {nameof(engineContext)} = {engineContext}, {nameof(timestamp)} = {timestamp} }}";
        }

        private sealed class UnhandledExceptionData
        {
            // Common properties. Properties not in this list could be trimmed.
            //[DynamicDependency(nameof(EngineContext.Frame), typeof(EngineContext))]
            //[DynamicDependency(nameof(EngineContext.Response), typeof(EngineContext))]
            //[DynamicDependency(nameof(HttpFrame.Path), typeof(HttpFrame))]
            //[DynamicDependency(nameof(HttpFrame.Method), typeof(HttpFrame))]
            //[DynamicDependency(nameof(HttpResponse.StatusCode), typeof(HttpResponse))]
            internal UnhandledExceptionData(EngineContext engineContext, long timestamp, Exception exception)
            {
                this.engineContext = engineContext;
                this.timestamp = timestamp;
                this.exception = exception;
            }

            // Compatibility with anonymous object property names
            public EngineContext engineContext { get; }
            public long timestamp { get; }
            public Exception exception { get; }

            public override string ToString() => $"{{ {nameof(engineContext)} = {engineContext}, {nameof(timestamp)} = {timestamp}, {nameof(exception)} = {exception} }}";
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void RecordFrameStartEventLog(EngineContext engineContext)
        {
            //_eventSource.FrameStart(engineContext.Frame.Method, engineContext.Frame.Path);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void RecordFrameStartMetrics(EngineContext engineContext)
        {
            //_metrics.FrameStart(engineContext.Frame.Scheme, engineContext.Frame.Method);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private Activity StartActivity(EngineContext engineContext, bool loggingEnabled, bool diagnosticListenerActivityCreationEnabled, out bool hasDiagnosticListener)
        {
            hasDiagnosticListener = false;

            var headers = engineContext.Event;//.Frame.Headers;
            var activity = ActivityCreator.CreateFromRemote(
                _activitySource,
                _propagator,
                headers,
                static (object carrier, string fieldName, out string fieldValue, out IEnumerable<string> fieldValues) =>
                {
                    fieldValues = default;
                    //var headers = (IHeaderDictionary)carrier!;
                    //fieldValue = headers[fieldName];
                    fieldValue = default;
                },
                ActivityName,
                ActivityKind.Server,
                tags: null,
                links: null,
                loggingEnabled || diagnosticListenerActivityCreationEnabled);
            if (activity is null)
            {
                return null;
            }

            _diagnosticListener.OnActivityImport(activity, engineContext);

            if (_diagnosticListener.IsEnabled(ActivityStartKey))
            {
                hasDiagnosticListener = true;
                StartActivity(activity, engineContext);
            }
            else
            {
                activity.Start();
            }

            return activity;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void StopActivity(EngineContext engineContext, Activity activity, bool hasDiagnosticListener)
        {
            if (hasDiagnosticListener)
            {
                StopActivity(activity, engineContext);
            }
            else
            {
                activity.Stop();
            }
        }

        // These are versions of DiagnosticSource.Start/StopActivity that don't allocate strings per call (see https://github.com/dotnet/corefx/issues/37055)
        // DynamicDependency matches the properties selected in:
        // https://github.com/dotnet/diagnostics/blob/7cc6fbef613cdfe5ff64393120d59d7a15e98bd6/src/Microsoft.Diagnostics.Monitoring.EventPipe/Configuration/HttpFrameSourceConfiguration.cs#L20-L33
        //[DynamicDependency(nameof(EngineContext.Frame), typeof(EngineContext))]
        //[DynamicDependency(nameof(HttpFrame.Scheme), typeof(HttpFrame))]
        //[DynamicDependency(nameof(HttpFrame.Host), typeof(HttpFrame))]
        //[DynamicDependency(nameof(HttpFrame.PathBase), typeof(HttpFrame))]
        //[DynamicDependency(nameof(HttpFrame.QueryString), typeof(HttpFrame))]
        //[DynamicDependency(nameof(HttpFrame.Path), typeof(HttpFrame))]
        //[DynamicDependency(nameof(HttpFrame.Method), typeof(HttpFrame))]
        //[DynamicDependency(nameof(HttpFrame.Headers), typeof(HttpFrame))]
        //[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(QueryString))]
        //[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(HostString))]
        //[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(PathString))]
        // OpenTelemetry gets the context from the context using the DefaultEngineContext.EngineContext property.
        [DynamicDependency(nameof(DefaultEngineContext.EngineContext), typeof(DefaultEngineContext))]
        private Activity StartActivity(Activity activity, EngineContext engineContext)
        {
            activity.Start();
            WriteDiagnosticEvent(_diagnosticListener, ActivityStartKey, engineContext);
            return activity;
        }

        // DynamicDependency matches the properties selected in:
        // https://github.com/dotnet/diagnostics/blob/7cc6fbef613cdfe5ff64393120d59d7a15e98bd6/src/Microsoft.Diagnostics.Monitoring.EventPipe/Configuration/HttpFrameSourceConfiguration.cs#L35-L38
        //[DynamicDependency(nameof(EngineContext.Response), typeof(EngineContext))]
        //[DynamicDependency(nameof(HttpResponse.StatusCode), typeof(HttpResponse))]
        //[DynamicDependency(nameof(HttpResponse.Headers), typeof(HttpResponse))]
        // OpenTelemetry gets the context from the context using the DefaultEngineContext.EngineContext property.
        [DynamicDependency(nameof(DefaultEngineContext.EngineContext), typeof(DefaultEngineContext))]
        private void StopActivity(Activity activity, EngineContext engineContext)
        {
            // Stop sets the end time if it was unset, but we want it set before we issue the write
            // so we do it now.
            if (activity.Duration == TimeSpan.Zero)
            {
                activity.SetEndTime(DateTime.UtcNow);
            }
            WriteDiagnosticEvent(_diagnosticListener, ActivityStopKey, engineContext);
            activity.Stop();    // Resets Activity.Current (we want this after the Write)
        }

        private static class Log
        {
            public static IDisposable FrameScope(ILogger logger, EngineContext engineContext)
            {
                return logger.BeginScope(new HostingLogScope(engineContext));
            }

            private sealed class HostingLogScope : IReadOnlyList<KeyValuePair<string, object>>
            {
                private readonly string _path;
                private readonly string _traceIdentifier;

                private string _cachedToString;

                public int Count => 2;

                public KeyValuePair<string, object> this[int index]
                {
                    get
                    {
                        if (index == 0)
                        {
                            return new KeyValuePair<string, object>("FrameId", _traceIdentifier);
                        }
                        else if (index == 1)
                        {
                            return new KeyValuePair<string, object>("FramePath", _path);
                        }

                        throw new ArgumentOutOfRangeException(nameof(index));
                    }
                }

                public HostingLogScope(EngineContext engineContext)
                {
                    _traceIdentifier = engineContext.TraceIdentifier;
                    //_path = (engineContext.Frame.PathBase.HasValue
                    //         ? engineContext.Frame.PathBase + engineContext.Frame.Path
                    //         : engineContext.Frame.Path).ToString();
                }

                public override string ToString()
                {
                    if (_cachedToString == null)
                    {
                        _cachedToString = string.Format(
                            CultureInfo.InvariantCulture,
                            "FramePath:{0} FrameId:{1}",
                            _path,
                            _traceIdentifier);
                    }

                    return _cachedToString;
                }

                public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
                {
                    for (var i = 0; i < Count; ++i)
                    {
                        yield return this[i];
                    }
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return GetEnumerator();
                }
            }
        }
    }
}