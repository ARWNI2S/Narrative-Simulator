namespace ARWNI2S.Engine.Orleans.GrainExtensions
{
    public interface IGrainStateAccessor<T> : IGrainExtension
    {
        Task<T> GetState();
        Task SetState(T state);
    }
}
