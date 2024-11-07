namespace ARWNI2S.Engine.Network.Session
{
    public interface ISessionFactory
    {
        IAppSession Create();

        Type SessionType { get; }
    }
}