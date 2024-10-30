using ARWNI2S.Node.Core.Entities;

namespace ARWNI2S.Portal.Services.Entities.Polls
{
    /// <summary>
    /// Represents a poll voting record
    /// </summary>
    public partial class PollVotingRecord : DataEntity
    {
        /// <summary>
        /// Gets or sets the poll answer identifier
        /// </summary>
        public int PollAnswerId { get; set; }

        /// <summary>
        /// Gets or sets the user identifier
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }
    }
}