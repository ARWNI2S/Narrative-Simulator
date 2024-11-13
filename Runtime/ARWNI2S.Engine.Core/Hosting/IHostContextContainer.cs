using ARWNI2S.Infrastructure.Engine;

namespace ARWNI2S.Engine.Hosting
{
    /// <summary>
    /// When implemented by a Server allows an <see cref="IEngine{TContext}"/> to pool and reuse
    /// its <typeparamref name="TContext"/> between requests.
    /// </summary>
    /// <typeparam name="TContext">The <see cref="IEngine{TContext}"/> Host context</typeparam>
    public interface IHostContextContainer<TContext> where TContext : notnull
    {
        /// <summary>
        /// Represents the <typeparamref name="TContext"/>  of the host.
        /// </summary>
        TContext HostContext { get; set; }
    }
}