using ARWNI2S.Node.Data.Entities;

namespace ARWNI2S.Portal.Services.Entities.Gdpr
{
    /// <summary>
    /// Represents a GDPR log
    /// </summary>
    public partial class GdprLog : BaseDataEntity
    {
        /// <summary>
        /// Gets or sets the user identifier
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the consent identifier (0 if not related to any consent)
        /// </summary>
        public int ConsentId { get; set; }

        /// <summary>
        /// Gets or sets the user info (when a user records is already deleted)
        /// </summary>
        public string UserInfo { get; set; }

        /// <summary>
        /// Gets or sets the request type identifier
        /// </summary>
        public int RequestTypeId { get; set; }

        /// <summary>
        /// Gets or sets the request details
        /// </summary>
        public string RequestDetails { get; set; }

        /// <summary>
        /// Gets or sets the date and time of entity creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the request type
        /// </summary>
        public GdprRequestType RequestType
        {
            get => (GdprRequestType)RequestTypeId;
            set => RequestTypeId = (int)value;
        }
    }
}
