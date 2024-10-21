using ARWNI2S.Infrastructure.Collections.Trees;

namespace ARWNI2S.Engine.Simulation.Entities.Builder
{
    internal class EntityHierarchyBuilder : TreeBuilder<ISimulableEntity>
    {
        public EntityHierarchyBuilder()
        {
        }

        public EntityHierarchyBuilder(ISimulableEntity owner, ISimulableEntity root = null)
        {
            // Si no se pasa un componente, crea el DefaultRootComponent.
            root ??= new DefaultRootComponent(owner);
            SetRootValue(root);
        }

        internal IActorComponent GetRootObject()
        {
            return RootNode.Value;
        }

        internal bool SetRootObject(IActorComponent rootComponent)
        {
            try
            {
                RootNode.Value = rootComponent;
                return true;
            }
            catch
            {
                //TODO: HANDLE ERROR
            }
            return false;
        }
    }
}
