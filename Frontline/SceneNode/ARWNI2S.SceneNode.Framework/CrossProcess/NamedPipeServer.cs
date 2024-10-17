using Microsoft.Extensions.Hosting;

namespace ARWNI2S.SceneNode.Framework.CrossProcess
{
    internal class NamedPipeServer : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.CompletedTask;
        }
    }
}