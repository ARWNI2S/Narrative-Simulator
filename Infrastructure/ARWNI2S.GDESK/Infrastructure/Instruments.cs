using System.Diagnostics.Metrics;

namespace ARWNI2S.Engine.Infrastructure
{
    public static class Instruments
    {
        public static readonly Meter Meter = new("ARWNI2S.GDESK");
    }
}