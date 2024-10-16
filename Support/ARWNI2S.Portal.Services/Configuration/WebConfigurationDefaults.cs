namespace ARWNI2S.Portal.Services.Configuration
{
    /// <summary>
    /// Represents default values related to configuration services
    /// </summary>
    public static partial class WebConfigurationDefaults
    {
        /// <summary>
        /// Gets the path to file that contains app settings
        /// </summary>
        public static string NI2SSettingsFilePath => "App_Data/appsettings.json";

        /// <summary>
        /// Gets the path to file that contains app settings for specific hosting environment
        /// </summary>
        /// <remarks>0 - Environment name</remarks>
        public static string NI2SSettingsEnvironmentFilePath => "App_Data/appsettings.{0}.json";
    }
}
