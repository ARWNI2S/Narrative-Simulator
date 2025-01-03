namespace ARWNI2S.Framework.Common.Entities
{
    /// <summary>
    /// Search term record (for statistics)
    /// </summary>
    public partial class SearchTermReportLine
    {
        /// <summary>
        /// Gets or sets the keyword
        /// </summary>
        public string Keyword { get; set; }

        /// <summary>
        /// Gets or sets search count
        /// </summary>
        public int Count { get; set; }
    }
}