using System.Reflection;

namespace ARWNI2S.Engine.Simulation.Kernel
{
    internal class Event
    {
        private static int IDs;

        public Event() { Id = IDs; IDs++; }

        public double TimeMs => Time.TotalMilliseconds;
        public TimeSpan Time { get; internal set; }
        public int Priority { get; internal set; }
        public ISimulable Sender { get; internal set; }
        public ISimulable Receiver { get; internal set; }
        public string Data { get; internal set; }
        public int Id { get; private set; }
    }
}