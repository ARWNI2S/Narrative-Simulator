using System.Reflection;

namespace ARWNI2S.Engine.Dispatching
{
    public class EntityProxy<T> : DispatchProxy where T : class
    {
        private T _instance;

        // DispatchProxy's parameterless ctor is called when a 
        // new proxy instance is Created
        public EntityProxy() : base() { }

        public static T Create(T simObject = null)
        {
            // DispatchProxy.Create creates proxy objects
            var proxy = Create<T, EntityProxy<T>>() as EntityProxy<T>;

            proxy._instance = simObject ?? Activator.CreateInstance<T>();

            return proxy as T;
        }

        protected override object Invoke(MethodInfo method, object[] args)
        {
            // Verificar si el método está marcado como SimEventMethod
            if (method.GetCustomAttribute<NI2S_MethodAttribute>() != null)
            {
                // Crear y encolar el evento automáticamente
                //var simEvent = new SimEvent(_instance, _instance, method, args ?? [], SimulationEngine.CurrentTime + 1000);
                //SimulationEngine.EnqueueEvent(simEvent);

                // Devolver Task.CompletedTask si el método es void o async
                if (method.ReturnType == typeof(void))
                {
                    return null;
                }
                else if (method.ReturnType == typeof(Task))
                {
                    return Task.CompletedTask;
                }
            }

            // Invocar el método directamente si no está marcado
            return method.Invoke(_instance, args);
        }
    }
}
