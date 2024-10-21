using ARWNI2S.Engine.Simulation.Entities.Components;

namespace ARWNI2S.Engine.Simulation.World
{
    public sealed class WorldRootComponent : ActorComponentBase
    {
        public WorldRootComponent(SimulationWorld world) : base(world) { }
    }
}