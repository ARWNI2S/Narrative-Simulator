using ARWNI2S.Node.Core.Entities.Localization;

namespace ARWNI2S.Portal.Services.Localization
{
    public partial class PortalLocalizationSettings : LocalizationSettings
    {
        /// <summary>
        /// A value indicating whether to load all search engine friendly names (slugs) on application startup
        /// </summary>
        public bool LoadAllUrlRecordsOnStartup { get; set; }

    }
}
