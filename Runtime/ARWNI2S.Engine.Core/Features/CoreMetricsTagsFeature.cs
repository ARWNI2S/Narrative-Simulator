using ARWNI2S.Infrastructure.Engine.Features;

namespace ARWNI2S.Engine.Features
{
    internal sealed class CoreMetricsTagsFeature : IMetricsTagsFeature
    {
        ICollection<KeyValuePair<string, object>> IMetricsTagsFeature.Tags => TagsList;
        public bool MetricsDisabled { get; set; }

        public List<KeyValuePair<string, object>> TagsList { get; } = [];

        // Cache request values when request starts. These are used when writing metrics when the request ends.
        // This ensures that the tags match between the start and end of the request. Important for up/down counters.
        public string Method { get; set; }
        public string Scheme { get; set; }
        public string Protocol { get; set; }

        public void Reset()
        {
            TagsList.Clear();
            MetricsDisabled = false;

            Method = null;
            Scheme = null;
            Protocol = null;
        }
    }
}
