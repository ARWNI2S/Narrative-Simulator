namespace ARWNI2S.Engine.Simulation.World
{
    internal class DefaultWorldAccessor : IWorldAccessor
    {
        private readonly ISimulation _simulation;

        public DefaultWorldAccessor(ISimulation simulation) {
            _simulation = simulation;
        }
        public IWorld World => GetWorld();

        public IWorld GetWorld() => _simulation?.World;
    }
}
