using ARWNI2S.Engine.Data.Entities;

namespace ARWNI2S.Backend.Services.Users.Entities
{
    /// <summary>
    /// Represents a reward point history entry
    /// </summary>
    public partial class RewardPointsHistory : DataEntity
    {
        /// <summary>
        /// Gets or sets the user identifier
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the store identifier in which these reward points were awarded or redeemed
        /// </summary>
        public int StoreId { get; set; }

        /// <summary>
        /// Gets or sets the points redeemed/added
        /// </summary>
        public int Points { get; set; }

        /// <summary>
        /// Gets or sets the points balance
        /// </summary>
        public int? PointsBalance { get; set; }

        /// <summary>
        /// Gets or sets the used amount
        /// </summary>
        public decimal UsedAmount { get; set; }

        /// <summary>
        /// Gets or sets the message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the points will no longer be valid
        /// </summary>
        public DateTime? EndDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the number of valid points that have not yet spent (only for positive amount of points)
        /// </summary>
        public int? ValidPoints { get; set; }

        /// <summary>
        /// Used with order
        /// </summary>
        public Guid? UsedWithOrder { get; set; }
    }
}