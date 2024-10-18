using ARWNI2S.Node.Core.Entities.Localization;
using ARWNI2S.Node.Core.Entities;

namespace ARWNI2S.Portal.Services.Entities.Gdpr
{
    /// <summary>
    /// Represents a GDPR consent
    /// </summary>
    public partial class GdprConsent : BaseEntity, ILocalizedEntity
    {
        /// <summary>
        /// Gets or sets the message displayed to users
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or setsa value indicating whether the consent is required
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Gets or sets the message displayed to users when he/she doesn't agree to this consent
        /// </summary>
        public string RequiredMessage { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether this consent is displayed during registrations
        /// </summary>
        public bool DisplayDuringRegistration { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether this consent is displayed on my account page (user info)
        /// </summary>
        public bool DisplayOnUserInfoPage { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }
    }
}