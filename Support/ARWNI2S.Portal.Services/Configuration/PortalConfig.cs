using ARWNI2S.Infrastructure.Configuration;

namespace ARWNI2S.Portal.Services.Configuration
{
    /// <summary>
    /// Represents common configuration parameters
    /// </summary>
    public partial class PortalConfig : IConfig
    {
        /// <summary>
        /// Gets or sets path to database with user agent strings
        /// </summary>
        public string UserAgentStringsPath { get; private set; } = "~/App_Data/browscap.xml";

        /// <summary>
        /// Gets or sets path to database with crawler only user agent strings
        /// </summary>
        public string CrawlerOnlyUserAgentStringsPath { get; private set; } = "~/App_Data/browscap.crawlersonly.xml";

        /// <summary>
        /// Gets or sets path to additional database with crawler only user agent strings
        /// </summary>
        public string CrawlerOnlyAdditionalUserAgentStringsPath { get; private set; } = "~/App_Data/additional.crawlers.xml";

        /// <summary>
        /// Gets or sets a value indicating whether to store TempData in the session state. By default the cookie-based TempData provider is used to store TempData in cookies.
        /// </summary>
        public bool UseSessionStateTempDataProvider { get; private set; } = false;

        /// <summary>
        /// Gets or sets a value that indicates whether to use MiniProfiler services
        /// </summary>
        public bool MiniProfilerEnabled { get; private set; } = false;

        /// <summary>
        /// Get or set a value indicating whether to serve files that don't have a recognized content-type
        /// </summary>
        public bool ServeUnknownFileTypes { get; private set; } = false;

    }
}