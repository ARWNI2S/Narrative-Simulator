using ARWNI2S.Node.Core.Entities;

namespace ARWNI2S.Portal.Services.Entities.Common
{
    /// <summary>
    /// Search term record (for statistics)
    /// </summary>
    public partial class SearchTerm : BaseEntity
    {
        /// <summary>
        /// Gets or sets the keyword
        /// </summary>
        public string Keyword { get; set; }

        /// <summary>
        /// Gets or sets the server identifier
        /// </summary>
        public int NodeId { get; set; }

        /// <summary>
        /// Gets or sets search count
        /// </summary>
        public int Count { get; set; }
    }
}
