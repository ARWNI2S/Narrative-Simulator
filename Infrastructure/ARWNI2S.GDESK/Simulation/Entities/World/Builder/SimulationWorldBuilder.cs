using ARWNI2S.Infrastructure.Collections.Trees;

namespace ARWNI2S.Engine.Simulation.World.Builder
{
    internal class SimulationWorldBuilder : TreeBuilder<WorldTreeNode>
    {
        public SimulationWorldBuilder(SimulationWorld simulationWorld) 
        {
            SetRootValue(new WorldTreeNode(simulationWorld));
        }
    }
}
