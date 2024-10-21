using ARWNI2S.Infrastructure.Collections.Trees;

namespace ARWNI2S.Engine.Simulation.Entities.Builder
{
    internal class ComposeTreeNode : TreeNode<IActorComponent>
    {
        public ComposeTreeNode(IActorComponent value)
            : base(value)
        {
        }
    }
}
