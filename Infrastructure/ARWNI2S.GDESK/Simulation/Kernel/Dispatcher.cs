using ARWNI2S.Engine.Infrastructure;
using ARWNI2S.Engine.Simulation.Time;
using ARWNI2S.Infrastructure.Collections.Sorting;
using System.Collections;

namespace ARWNI2S.Engine.Simulation.Kernel
{
    internal sealed class Dispatcher
    {
        public struct Statistics
        {
            public ulong TotalEvents;
            public TimeSpan LastTime;
            public TimeSpan LastEventTime;

        }

        private readonly QuickSorter _quickSorter;

        private readonly FrameTaskScheduler _frameScheduler;
        private readonly ISimulationClock _clock;
        private readonly List<Event> _eventQueue;


        public Dispatcher(FrameTaskScheduler frameScheduler, ISimulationClock clock)
        {
            _frameScheduler = frameScheduler;
            _clock = clock;

            _quickSorter = new QuickSorter(new TimePriorityComparer(), new DefaultSwap());
            _eventQueue = [];

        }

        public void EnqueueEvent(Event @event)
        {
            if (@event == null) return;
            lock (_eventQueue)
            {
                _eventQueue.Add(@event);
            }
        }

        public Task ProcessEvents(CancellationToken cancellationToken)
        {
            var time = _clock.GetCurrentTimeAsync().GetAwaiter().GetResult();
            lock (_eventQueue)
            {
                _quickSorter.Sort(_eventQueue);
            }

            return Task.Run(() =>
            {
                lock (_eventQueue)
                {
                    int queueLen = _eventQueue.Count;
                    while (queueLen > 0 && time >= _eventQueue[queueLen - 1]?.Time)
                    {
                        Event next = _eventQueue[queueLen - 1];
                        _eventQueue.RemoveAt(queueLen - 1);

                        Task.Factory.StartNew(() =>
                        {
                            next.Receiver.HandleEvent(next);
                        }, cancellationToken, TaskCreationOptions.None, _frameScheduler);

                        queueLen = _eventQueue.Count;
                    }
                }
            }, cancellationToken);
        }

        private class TimePriorityComparer : IComparer<Event>, IComparer
        {
            public int Compare(Event x, Event y)
            {
                if (x.Time < y.Time) return 1;
                if (x.Time > y.Time) return -1;
                if (x.Priority < y.Priority) return 1;
                if (x.Priority > y.Priority) return -1;
                return 0;
            }

            public int Compare(object x, object y)
            {
                return Compare((Event)x, (Event)y);
            }
        }
    }
}
