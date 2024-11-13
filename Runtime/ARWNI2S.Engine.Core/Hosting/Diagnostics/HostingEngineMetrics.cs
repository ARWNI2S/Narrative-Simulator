using ARWNI2S.Infrastructure.Extensions;
using System.Collections.Frozen;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace ARWNI2S.Engine.Hosting.Diagnostics
{
    internal sealed class HostingEngineMetrics : IDisposable
    {
        public const string MeterName = "ARWNI2S.Node.Hosting";

        private readonly Meter _meter;
        private readonly UpDownCounter<long> _activeFramesCounter;
        private readonly Histogram<double> _frameDuration;

        public HostingEngineMetrics(IMeterFactory meterFactory)
        {
            _meter = meterFactory.Create(MeterName);

            _activeFramesCounter = _meter.CreateUpDownCounter<long>(
                "http.server.active_frames",
                unit: "{frame}",
                description: "Number of active HTTP server frames.");

            _frameDuration = _meter.CreateHistogram<double>(
                "http.server.frame.duration",
                unit: "s",
                description: "Duration of HTTP server frames."//,
                                                              //advice: new InstrumentAdvice<double> { HistogramBucketBoundaries = MetricsConstants.ShortSecondsBucketBoundaries }
                );
        }

        // Note: Calling code checks whether counter is enabled.
        public void FrameStart(string scheme, string method)
        {
            // Tags must match frame end.
            var tags = new TagList();
            InitializeFrameTags(ref tags, scheme, method);
            _activeFramesCounter.Add(1, tags);
        }

        public void FrameEnd(string protocol, string scheme, string method, string route, int statusCode, bool unhandledFrame, Exception exception, List<KeyValuePair<string, object>> customTags, long startTimestamp, long currentTimestamp, bool disableHttpFrameDurationMetric)
        {
            var tags = new TagList();
            InitializeFrameTags(ref tags, scheme, method);

            // Tags must match frame start.
            if (_activeFramesCounter.Enabled)
            {
                _activeFramesCounter.Add(-1, tags);
            }

            if (!disableHttpFrameDurationMetric && _frameDuration.Enabled)
            {
                //if (TryGetHttpVersion(protocol, out var httpVersion))
                //{
                //    tags.Add("network.protocol.version", httpVersion);
                //}
                if (unhandledFrame)
                {
                    tags.Add("aspnetcore.frame.is_unhandled", true);
                }

                // Add information gathered during frame.
                tags.Add("http.response.status_code", GetBoxedStatusCode(statusCode));
                if (route != null)
                {
                    tags.Add("http.route", route);
                }

                // Add before some built in tags so custom tags are prioritized when dealing with duplicates.
                if (customTags != null)
                {
                    for (var i = 0; i < customTags.Count; i++)
                    {
                        tags.Add(customTags[i]);
                    }
                }

                // This exception is only present if there is an unhandled exception.
                // An exception caught by ExceptionHandlerMiddleware and DeveloperExceptionMiddleware isn't thrown to here. Instead, those middleware add error.type to custom tags.
                if (exception != null)
                {
                    // Exception tag could have been added by middleware. If an exception is later thrown in frame pipeline
                    // then we don't want to add a duplicate tag here because that breaks some metrics systems.
                    tags.TryAddTag("error.type", exception.GetType().FullName);
                }

                var duration = Stopwatch.GetElapsedTime(startTimestamp, currentTimestamp);
                _frameDuration.Record(duration.TotalSeconds, tags);
            }
        }

        public void Dispose()
        {
            _meter.Dispose();
        }

        public bool IsEnabled() => _activeFramesCounter.Enabled || _frameDuration.Enabled;

        private static void InitializeFrameTags(ref TagList tags, string scheme, string method)
        {
            tags.Add("url.scheme", scheme);
            tags.Add("http.frame.method", ResolveHttpMethod(method));
        }

        private static readonly object[] BoxedStatusCodes = new object[512];

        private static object GetBoxedStatusCode(int statusCode)
        {
            object[] boxes = BoxedStatusCodes;
            return (uint)statusCode < (uint)boxes.Length
                ? boxes[statusCode] ??= statusCode
                : statusCode;
        }

        private static readonly FrozenDictionary<string, string> KnownMethods = (new[]
        {
            KeyValuePair.Create(NiisMethods.Connect, NiisMethods.Connect),
            KeyValuePair.Create(NiisMethods.Delete, NiisMethods.Delete),
            KeyValuePair.Create(NiisMethods.Get, NiisMethods.Get),
            KeyValuePair.Create(NiisMethods.Head, NiisMethods.Head),
            KeyValuePair.Create(NiisMethods.Options, NiisMethods.Options),
            KeyValuePair.Create(NiisMethods.Patch, NiisMethods.Patch),
            KeyValuePair.Create(NiisMethods.Post, NiisMethods.Post),
            KeyValuePair.Create(NiisMethods.Put, NiisMethods.Put),
            KeyValuePair.Create(NiisMethods.Trace, NiisMethods.Trace)
        }).ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

        private static string ResolveHttpMethod(string method)
        {
            // TODO: Support configuration for configuring known methods
            if (KnownMethods.TryGetValue(method, out var result))
            {
                // KnownMethods ignores case. Use the value returned by the dictionary to have a consistent case.
                return result;
            }
            return "_OTHER";
        }

        //private static bool TryGetHttpVersion(string protocol, [NotNullWhen(true)] out string? version)
        //{
        //    if (HttpProtocol.IsHttp11(protocol))
        //    {
        //        version = "1.1";
        //        return true;
        //    }
        //    if (HttpProtocol.IsHttp2(protocol))
        //    {
        //        // HTTP/2 only has one version.
        //        version = "2";
        //        return true;
        //    }
        //    if (HttpProtocol.IsHttp3(protocol))
        //    {
        //        // HTTP/3 only has one version.
        //        version = "3";
        //        return true;
        //    }
        //    if (HttpProtocol.IsHttp10(protocol))
        //    {
        //        version = "1.0";
        //        return true;
        //    }
        //    if (HttpProtocol.IsHttp09(protocol))
        //    {
        //        version = "0.9";
        //        return true;
        //    }
        //    version = null;
        //    return false;
        //}
    }
}