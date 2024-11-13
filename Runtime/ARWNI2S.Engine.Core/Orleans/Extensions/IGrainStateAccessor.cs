namespace ARWNI2S.Engine.Orleans.Extensions
{
    public interface IGrainStateAccessor<T> : IGrainExtension
    {
        Task<T> GetState();
        Task SetState(T state);
    }
}
