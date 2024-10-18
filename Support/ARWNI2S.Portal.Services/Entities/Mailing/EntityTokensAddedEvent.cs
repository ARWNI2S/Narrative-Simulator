using ARWNI2S.Node.Core.Entities;

namespace ARWNI2S.Portal.Services.Entities.Mailing
{
    /// <summary>
    /// A container for tokens that are added.
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <typeparam name="U"></typeparam>
    public partial class EntityTokensAddedEvent<T, U> where T : BaseEntity
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="tokens">Tokens</param>
        public EntityTokensAddedEvent(T entity, IList<U> tokens)
        {
            Entity = entity;
            Tokens = tokens;
        }

        /// <summary>
        /// Entity
        /// </summary>
        public T Entity { get; }

        /// <summary>
        /// Tokens
        /// </summary>
        public IList<U> Tokens { get; }
    }
}