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
        public static string NI2SSettingsFilePath => "Node_Data/nodesettings.json";

        /// <summary>
        /// Gets the path to file that contains app settings for specific hosting environment
        /// </summary>
        /// <remarks>0 - Environment name</remarks>
        public static string NI2SSettingsEnvironmentFilePath => "Node_Data/nodesettings.{0}.json";
    }
}
