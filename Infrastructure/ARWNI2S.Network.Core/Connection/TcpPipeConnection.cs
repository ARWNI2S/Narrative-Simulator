﻿using System.Net.Sockets;
using System.Buffers;

namespace ARWNI2S.Engine.Network.Connection
{
    public class TcpPipeConnection : PipeConnection
    {

        private Socket _socket;

        private List<ArraySegment<byte>> _segmentsForSend;

        public TcpPipeConnection(Socket socket, ConnectionOptions options)
            : base(options)
        {
            _socket = socket;
            RemoteEndPoint = socket.RemoteEndPoint;
            LocalEndPoint = socket.LocalEndPoint;
        }

        protected override void OnClosed()
        {
            _socket = null;
            base.OnClosed();
        }

        protected override async ValueTask<int> FillPipeWithDataAsync(Memory<byte> memory, CancellationToken cancellationToken)
        {
            return await ReceiveAsync(_socket, memory, SocketFlags.None, cancellationToken)
                .ConfigureAwait(false);
        }

        private async ValueTask<int> ReceiveAsync(Socket socket, Memory<byte> memory, SocketFlags socketFlags, CancellationToken cancellationToken)
        {
            return await socket
                .ReceiveAsync(GetArrayByMemory(memory), socketFlags, cancellationToken)
                .ConfigureAwait(false);
        }

        protected override async ValueTask<int> SendOverIOAsync(ReadOnlySequence<byte> buffer, CancellationToken cancellationToken)
        {
            if (buffer.IsSingleSegment)
            {
                return await _socket
                    .SendAsync(GetArrayByMemory(buffer.First), SocketFlags.None, cancellationToken)
                    .ConfigureAwait(false);
            }

            if (_segmentsForSend == null)
            {
                _segmentsForSend = [];
            }
            else
            {
                _segmentsForSend.Clear();
            }

            var segments = _segmentsForSend;

            foreach (var piece in buffer)
            {
                cancellationToken.ThrowIfCancellationRequested();
                _segmentsForSend.Add(GetArrayByMemory(piece));
            }

            cancellationToken.ThrowIfCancellationRequested();

            return await _socket
                .SendAsync(_segmentsForSend, SocketFlags.None)
                .ConfigureAwait(false);
        }

        protected override void Close()
        {
            var socket = _socket;

            if (socket == null)
                return;

            if (Interlocked.CompareExchange(ref _socket, null, socket) == socket)
            {
                try
                {
                    socket.Shutdown(SocketShutdown.Both);
                }
                finally
                {
                    socket.Close();
                }
            }
        }

        protected override bool IsIgnorableException(Exception e)
        {
            if (base.IsIgnorableException(e))
                return true;

            if (e is SocketException se)
            {
                if (se.IsIgnorableSocketException())
                    return true;
            }

            return false;
        }
    }
}