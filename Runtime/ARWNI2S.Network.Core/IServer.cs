namespace ARWNI2S.Engine.Network
{
    public interface IServer : IServerInfo, IDisposable, IAsyncDisposable
    {

        Task StartAsync(CancellationToken cancellationToken);

        Task StopAsync(CancellationToken cancellationToken);
    }
}