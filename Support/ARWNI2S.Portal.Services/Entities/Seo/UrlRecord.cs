using ARWNI2S.Node.Core.Entities;

namespace ARWNI2S.Portal.Services.Entities.Seo
{
    /// <summary>
    /// Represents an URL record
    /// </summary>
    public partial class UrlRecord : DataEntity
    {
        /// <summary>
        /// Gets or sets the entity identifier
        /// </summary>
        public int EntityId { get; set; }

        /// <summary>
        /// Gets or sets the entity name
        /// </summary>
        public string EntityName { get; set; }

        /// <summary>
        /// Gets or sets the slug
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether the record is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the language identifier
        /// </summary>
        public int LanguageId { get; set; }
    }
}
