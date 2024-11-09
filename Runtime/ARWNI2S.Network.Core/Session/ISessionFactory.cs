namespace ARWNI2S.Engine.Network.Session
{
    public interface ISessionFactory
    {
        INodeSession Create();

        Type SessionType { get; }
    }
}