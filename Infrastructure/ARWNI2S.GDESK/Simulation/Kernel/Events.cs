using System.Reflection;


namespace ARWNI2S.Engine.Simulation.Kernel
{
    internal class Event : IDisposable
    {
        protected readonly Dispatcher _dispatcher;
        protected readonly EventPool _eventPool;

        public object Sender { get; private set; }              // Emisor del evento
        public object Target { get; private set; }              // Destinatario del evento
        public MethodInfo Method { get; private set; }          // Método a invocar en el destinatario
        public object[] Parameters { get; private set; }        // Parámetros para el método
        public ulong Time { get; private set; }                 // Tiempo absoluto en el que se ejecutará

        internal Event(Dispatcher dispatcher, EventPool eventPool)
        {
            _dispatcher = dispatcher;
            _eventPool = eventPool;
        }

        public void Schedule(ulong time, object sender, object target, MethodInfo method, params object[] parameters)
        {
            Time = time;
            Sender = sender;
            Target = target;
            Method = method;
            Parameters = parameters;

            OnScheduling();
        }

        protected virtual void OnScheduling()
        {
            _dispatcher.ScheduleEvent(this);
        }

        public void Execute() => OnExecuting();

        protected virtual void OnExecuting()
        {
            Method?.Invoke(Target, Parameters);
        }

        public void Dispose() => OnDisposing();

        protected virtual void OnDisposing()
        {
            Time = 0;
            Sender = null;
            Target = null;
            Method = null;
            Parameters = null;

            _eventPool.Push(this);
        }
    }


    internal class TickEvent : Event
    {
        public TickEvent(Dispatcher dispatcher, EventPool eventPool)
          : base(dispatcher, eventPool) { }

        protected override void OnDisposing()
        {
            _eventPool.Push(this);
        }
    }
}