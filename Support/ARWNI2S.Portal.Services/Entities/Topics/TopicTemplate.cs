using ARWNI2S.Node.Core.Entities;

namespace ARWNI2S.Portal.Services.Entities.Topics
{
    /// <summary>
    /// Represents a topic template
    /// </summary>
    public partial class TopicTemplate : DataEntity
    {
        /// <summary>
        /// Gets or sets the template name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the view path
        /// </summary>
        public string ViewPath { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }
    }
}
