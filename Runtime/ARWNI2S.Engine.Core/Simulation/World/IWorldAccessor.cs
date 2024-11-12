namespace ARWNI2S.Engine.Simulation.World
{
    public interface IWorldAccessor
    {
        IWorld World { get; }

        IWorld GetWorld();
    }
}
