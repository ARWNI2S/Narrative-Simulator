namespace ARWNI2S.Engine.Relayer
{
    public abstract class SceneRelayer
    {
        private List<(IRelayListener, IRelaySender)> _connections = new();

        protected virtual void AddConnection(IRelayListener listener, IRelaySender sender)
        {
            _connections.Add((listener, sender));
        }

        public void RelayMessages()
        {
            foreach (var (listener, sender) in _connections)
            {
                // Leemos los mensajes desde el Listener (es decir, desde Layer 3)
                var reader = listener.RelayReader;
                var data = reader.Read();

                // Procesamos o retransmitimos el mensaje
                var writer = sender.RelayWriter;
                writer.Write(data);
            }
        }
    }
}
