namespace ARWNI2S.Backend.Services.Users.Entities
{
    /// <summary>
    /// Represents a best user report line
    /// </summary>
    public partial class BestUserReportLine
    {
        /// <summary>
        /// Gets or sets the user identifier
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the order total
        /// </summary>
        public decimal OrderTotal { get; set; }

        /// <summary>
        /// Gets or sets the order count
        /// </summary>
        public int OrderCount { get; set; }
    }
}