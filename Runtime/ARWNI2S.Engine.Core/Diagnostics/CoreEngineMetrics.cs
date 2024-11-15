namespace ARWNI2S.Engine.Diagnostics
{
    internal sealed class CoreEngineMetrics : IDisposable
    {
        public const string MeterName = "ARWNI2S.Engine";

        //private readonly Meter _meter;
        //private readonly UpDownCounter<long> _activeFramesCounter;
        //private readonly Histogram<double> _frameDuration;

        public CoreEngineMetrics()
        {
        }

        public void Dispose()
        {

        }
    }
}