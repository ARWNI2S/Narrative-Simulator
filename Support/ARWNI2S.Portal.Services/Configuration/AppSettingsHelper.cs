using ARWNI2S.Infrastructure;
using ARWNI2S.Infrastructure.Configuration;
using ARWNI2S.Node.Core.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace ARWNI2S.Portal.Services.Configuration
{
    /// <summary>
    /// Represents the app settings helper
    /// </summary>
    public partial class AppSettingsHelper
    {
        #region Fields

        private static Dictionary<string, int> _configurationOrder;

        #endregion

        #region Methods

        /// <summary>
        /// Create app settings with the passed configurations and save it to the file
        /// </summary>
        /// <param name="configurations">Configurations to save</param>
        /// <param name="fileProvider">File provider</param>
        /// <param name="overwrite">Whether to overwrite appsettings file</param>
        /// <returns>App settings</returns>
        public static AppSettings SaveAppSettings(IList<IConfig> configurations, IEngineFileProvider fileProvider, bool overwrite = true)
        {
            ArgumentNullException.ThrowIfNull(configurations);

            _configurationOrder ??= configurations.ToDictionary(config => config.Name, config => config.GetOrder());

            //create app settings
            var ni2sSettings = Singleton<AppSettings>.Instance ?? new AppSettings();
            ni2sSettings.Update(configurations);
            Singleton<AppSettings>.Instance = ni2sSettings;

            //create file if not exists
            var filePath = fileProvider.MapPath(WebConfigurationDefaults.NI2SSettingsFilePath);
            var fileExists = fileProvider.FileExists(filePath);
            fileProvider.CreateFile(filePath);

            //get raw configuration parameters
            var configuration = JsonConvert.DeserializeObject<AppSettings>(fileProvider.ReadAllText(filePath, Encoding.UTF8))
                ?.Configuration
                ?? [];
            foreach (var config in configurations)
            {
                configuration[config.Name] = JToken.FromObject(config);
            }

            //sort configurations for display by order (e.g. data configuration with 0 will be the first)
            ni2sSettings.Configuration = configuration
                .SelectMany(outConfig => _configurationOrder.Where(inConfig => inConfig.Key == outConfig.Key).DefaultIfEmpty(),
                    (outConfig, inConfig) => new { OutConfig = outConfig, InConfig = inConfig })
                .OrderBy(config => config.InConfig.Value)
                .Select(config => config.OutConfig)
                .ToDictionary(config => config.Key, config => config.Value);

            //save app settings to the file
            if (!fileExists || overwrite)
            {
                var text = JsonConvert.SerializeObject(ni2sSettings, Formatting.Indented);//, new JsonSerializerSettings { ContractResolver = new SecretContractResolver() });
                fileProvider.WriteAllText(filePath, text, Encoding.UTF8);
            }

            return ni2sSettings;
        }

        #endregion
    }
}