namespace ARWNI2S.Portal.Services.Installation
{
    /// <summary>
    /// Represents default values related to installation services
    /// </summary>
    public static partial class InstallationServiceDefaults
    {
        /// <summary>
        /// Gets a request path to the install URL
        /// </summary>
        public static string InstallPath => "install";

        /// <summary>
        /// Gets a path to the localization resources file
        /// </summary>
        public static string LocalizationResourcesPath => "~/Node_Data/Localization/";

        /// <summary>
        /// Gets a localization resources file extension
        /// </summary>
        public static string LocalizationResourcesFileExtension => "metalink.xml";

        /// <summary>
        /// Gets a path to the installation default images
        /// </summary>
        public static string DefaultImagesPath => "images\\default\\";
    }
}