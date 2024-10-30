namespace ARWNI2S.Engine.Simulation.Entities
{
    public interface IGameObject
    {
        Guid UUID { get; }

        void InitializeGameObject(IGameObjectBuilder objectBuilder);
    }
}
