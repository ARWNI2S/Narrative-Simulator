using ARWNI2S.Infrastructure.Entities;
using System.Reflection;

namespace ARWNI2S.Engine
{
    public abstract class EntityBase : IEntity
    {
        public Guid UUID { get; internal set; }

        object IEntity.Id => UUID;
    }

    internal sealed class EntityProxy<T> : DispatchProxy where T : EntityBase
    {
        private T _instance;

        public void Initialize(T instance)
        {
            _instance = instance;
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
