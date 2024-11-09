using ARWNI2S.Engine.Relayer.IO;

namespace ARWNI2S.Engine.Relayer
{
    public interface IRelaySender
    {
        IRelayWriter RelayWriter { get; }
    }

    public interface IRelaySender<TWriter> where TWriter : IRelayWriter
    {
        TWriter RelayWriter { get; }
    }

}
