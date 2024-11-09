using ARWNI2S.Engine.Network.Configuration.Options;
using ARWNI2S.Engine.Network.Connection.Pipes;
using System.Buffers;
using System.IO.Pipes;

namespace ARWNI2S.Engine.Network.NamedPipes
{
    public class NamedPipeConnection : PipeConnection
    {
        private NamedPipeServerStream _pipeStream;

        public NamedPipeConnection(NamedPipeServerStream pipeStream, ConnectionOptions options)
            : base(options)
        {
            _pipeStream = pipeStream;
        }

        protected override void OnClosed()
        {
            _pipeStream?.Dispose();
            _pipeStream = null;
            base.OnClosed();
        }

        protected override async ValueTask<int> FillPipeWithDataAsync(Memory<byte> memory, CancellationToken cancellationToken)
        {
            return await _pipeStream.ReadAsync(memory, cancellationToken).ConfigureAwait(false);
        }

        protected override async ValueTask<int> SendOverIOAsync(ReadOnlySequence<byte> buffer, CancellationToken cancellationToken)
        {
            foreach (var segment in buffer)
            {
                await _pipeStream.WriteAsync(segment, cancellationToken).ConfigureAwait(false);
            }

            await _pipeStream.FlushAsync(cancellationToken).ConfigureAwait(false);
            return (int)buffer.Length;
        }

        protected override void Close()
        {
            _pipeStream?.Dispose();
        }
    }
}
