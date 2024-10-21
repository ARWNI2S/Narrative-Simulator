using ARWNI2S.Engine.Simulation.Entities;
using ARWNI2S.Infrastructure.Collections.Trees;

namespace ARWNI2S.Engine.Simulation.World.Builder
{
    internal class WorldTreeNode : TreeNode<IActorEntity>
    {
        public WorldTreeNode(IActorEntity value)
            : base(value)
        {
        }
    }

}
