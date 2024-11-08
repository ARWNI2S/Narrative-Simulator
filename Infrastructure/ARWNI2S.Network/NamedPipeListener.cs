using System.IO.Pipes;
using System.Text;

#nullable enable
namespace ARWNI2S.Engine.Network
{
    public class NamedPipeListener
    {
        private readonly string _pipeName;
        private bool _isRunning;
        private Task? _listenerTask;
        private CancellationTokenSource? _cts;

        public NamedPipeListener(string pipeName)
        {
            _pipeName = pipeName;
        }

        public void StartListening()
        {
            _cts = new CancellationTokenSource();
            _isRunning = true;
            _listenerTask = Task.Run(() => Listen(_cts.Token));
        }

        private async Task Listen(CancellationToken token)
        {
            while (_isRunning && !token.IsCancellationRequested)
            {
                try
                {
                    using (var pipeServer = new NamedPipeServerStream(_pipeName,
                                                                      PipeDirection.InOut,
                                                                      1,
                                                                      PipeTransmissionMode.Byte,
                                                                      PipeOptions.Asynchronous))
                    {
                        Console.WriteLine($"Waiting for client connection on pipe: {_pipeName}");
                        await pipeServer.WaitForConnectionAsync(token);

                        if (pipeServer.IsConnected)
                        {
                            Console.WriteLine("Client connected.");
                            await HandleClientAsync(pipeServer);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Listener stopped.");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        private async Task HandleClientAsync(NamedPipeServerStream pipeServer)
        {
            using StreamReader reader = new(pipeServer, Encoding.UTF8);
            using StreamWriter writer = new(pipeServer, Encoding.UTF8) { AutoFlush = true };
            while (pipeServer.IsConnected)
            {
                string? message = await reader.ReadLineAsync();
                if (!string.IsNullOrEmpty(message))
                {
                    Console.WriteLine($"Received: {message}");
                    await writer.WriteLineAsync($"Echo: {message}");
                }
            }
        }

        public void StopListening()
        {
            _isRunning = false;
            _cts?.Cancel();
            _listenerTask?.Wait();
            Console.WriteLine("NamedPipeListener stopped.");
        }
    }
}
#nullable disable