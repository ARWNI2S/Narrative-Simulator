namespace ARWNI2S.Engine.Simulation.Entities
{
    public interface IGameObjectBuilder
    {
        Guid Build(IGameObject gameObject);
    }

    public interface IGameObjectBuilder<TGameObj> : IGameObjectBuilder where TGameObj : class, IGameObject
    {
        Guid Build(TGameObj gameObject);
    }
}