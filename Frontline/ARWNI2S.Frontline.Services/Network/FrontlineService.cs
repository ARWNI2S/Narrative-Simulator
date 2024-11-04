using ARWNI2S.Node.Core.Network;
using ARWNI2S.Node.Services.Session;
using Fleck;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;

namespace ARWNI2S.Frontline.Services.Network
{
    public class FrontlineService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ConnectionManager _connectionManager;
        private readonly IPAddress _ipAddress;
        private readonly int _port;
        public FrontlineService(IServiceProvider serviceProvider, ConnectionManager connectionManager, IPAddress ipAddress, int port)
        {
            _serviceProvider = serviceProvider;
            _connectionManager = connectionManager;
            _ipAddress = ipAddress;
            _port = port;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            FleckLog.Level = LogLevel.Info;
            var server = new WebSocketServer("ws://0.0.0.0:8080");

            server.Start(socket =>
            {
                socket.OnOpen = () => HandleConnectionOpened(socket);
                socket.OnClose = () => HandleConnectionClosed(socket);
                socket.OnMessage = message => HandleMessageReceived(socket, message);
            });

            return Task.CompletedTask;
        }

        private void HandleConnectionOpened(IWebSocketConnection socket)
        {
            var connectionId = Guid.NewGuid().ToString();
            _connectionManager.CreateConnectionScope(connectionId);
        }

        private void HandleConnectionClosed(IWebSocketConnection socket)
        {
            var connectionId = GetConnectionId(socket);
            _connectionManager.RemoveConnectionScope(connectionId);
        }

        private void HandleMessageReceived(IWebSocketConnection socket, string message)
        {
            var sessionId = GetConnectionId(socket);
            var scope = _connectionManager.GetConnectionScope(sessionId);

            // Autenticación y manejo de mensajes en el Scope de la conexión
            var authService = scope.ServiceProvider.GetRequiredService<ISessionStateService>();
            var authResult = authService.AuthenticateAsync(sessionId, message).Result;

            var responseMessage = authResult.Succeeded ? "Autenticación exitosa" : "Autenticación fallida";
            socket.Send(responseMessage);
        }

        private string GetConnectionId(IWebSocketConnection socket)
        {
            return socket.ConnectionInfo.Id.ToString();
        }
    }
}
