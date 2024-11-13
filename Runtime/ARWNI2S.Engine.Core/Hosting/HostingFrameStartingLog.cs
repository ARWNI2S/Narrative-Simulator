using ARWNI2S.Engine.Tasks;
using ARWNI2S.Infrastructure.Engine;
using System.Collections;
using System.Globalization;

namespace ARWNI2S.Engine.Hosting
{
    internal sealed class HostingFrameStartingLog : IReadOnlyList<KeyValuePair<string, object>>
    {
        private const string OriginalFormat = "Frame starting {Protocol} {Method} {Scheme}://{Host}{PathBase}{Path}{QueryString} - {ContentType} {ContentLength}";
        private const string EmptyEntry = "-";

        internal static readonly Func<object, Exception, string> Callback = (state, exception) => ((HostingFrameStartingLog)state).ToString();

        private readonly NiisFrame _frame;

        private string _cachedToString;

        public int Count => 10;

        public KeyValuePair<string, object> this[int index] => index switch
        {
            0 => new KeyValuePair<string, object>(nameof(_frame.Protocol), _frame.Protocol),
            1 => new KeyValuePair<string, object>(nameof(_frame.Method), _frame.Method),
            2 => new KeyValuePair<string, object>(nameof(_frame.ContentType), _frame.ContentType),
            3 => new KeyValuePair<string, object>(nameof(_frame.ContentLength), _frame.ContentLength),
            4 => new KeyValuePair<string, object>(nameof(_frame.Scheme), _frame.Scheme),
            5 => new KeyValuePair<string, object>(nameof(_frame.Host), _frame.Host),
            6 => new KeyValuePair<string, object>(nameof(_frame.PathBase), _frame.PathBase),
            7 => new KeyValuePair<string, object>(nameof(_frame.Path), _frame.Path),
            8 => new KeyValuePair<string, object>(nameof(_frame.QueryString), _frame.QueryString),
            9 => new KeyValuePair<string, object>("{OriginalFormat}", OriginalFormat),
            _ => throw new ArgumentOutOfRangeException(nameof(index)),
        };

        public HostingFrameStartingLog(EngineContext engineContext)
        {
            //_frame = engineContext.Frame;
        }

        public override string ToString()
        {
            if (_cachedToString == null)
            {
                var frame = _frame;
                _cachedToString = $"Frame starting {frame.Protocol} {frame.Method} {frame.Scheme}://{frame.Host}{frame.PathBase}{frame.Path}{frame.QueryString} - {EscapedValueOrEmptyMarker(frame.ContentType)} {ValueOrEmptyMarker(frame.ContentLength)}";
            }

            return _cachedToString;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            for (var i = 0; i < Count; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal static string EscapedValueOrEmptyMarker(string potentialValue)
            // Encode space as +
            => potentialValue?.Length > 0 ? potentialValue.Replace(' ', '+') : EmptyEntry;

        internal static string ValueOrEmptyMarker<T>(T? potentialValue) where T : struct, IFormattable
            => potentialValue?.ToString(null, CultureInfo.InvariantCulture) ?? EmptyEntry;
    }
}