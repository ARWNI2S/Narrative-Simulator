using ARWNI2S.Node.Data.Entities;

namespace ARWNI2S.Portal.Services.Entities.Mailing
{
    /// <summary>
    /// Represents a system message
    /// </summary>
    public partial class SystemMessage : BaseDataEntity
    {
        /// <summary>
        /// Gets or sets the node identifier
        /// </summary>
        public int NodeId { get; set; }

        /// <summary>
        /// Gets or sets the user identifier who should receive the message
        /// </summary>
        public int ToUserId { get; set; }

        /// <summary>
        /// Gets or sets the subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the text
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether message is read
        /// </summary>
        public bool IsRead { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

    }
}
