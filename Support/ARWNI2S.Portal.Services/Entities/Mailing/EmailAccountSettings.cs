using ARWNI2S.Infrastructure.Configuration;

namespace ARWNI2S.Portal.Services.Entities.Mailing
{
    /// <summary>
    /// Email account settings
    /// </summary>
    public partial class EmailAccountSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a node default email account identifier
        /// </summary>
        public int DefaultEmailAccountId { get; set; }
    }
}
