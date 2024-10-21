using ARWNI2S.Engine.Relayer.IO;

namespace ARWNI2S.Engine.Relayer
{
    public interface IRelayListener
    {
        IRelayReader RelayReader { get; }
    }

    public interface IRelayListener<TReader> where TReader : IRelayReader
    {
        TReader RelayReader { get; }
    }
}
