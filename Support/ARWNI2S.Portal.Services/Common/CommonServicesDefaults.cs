using ARWNI2S.Node.Core.Caching;

namespace ARWNI2S.Portal.Services.Common
{
    /// <summary>
    /// Represents default values related to common services
    /// </summary>
    public static partial class CommonServicesDefaults
    {
        /// <summary>
        /// Gets a request path to the keep alive URL
        /// </summary>
        public static string KeepAlivePath => "keepalive/index";

        #region Address attributes

        /// <summary>
        /// Gets a name of the custom address attribute control
        /// </summary>
        /// <remarks>
        /// {0} : address attribute id
        /// </remarks>
        public static string AddressAttributeControlName => "address_attribute_{0}";

        #endregion

        #region Maintenance

        /// <summary>
        /// Gets a default timeout (in milliseconds) before restarting the application
        /// </summary>
        public static int RestartTimeout => 3000;

        /// <summary>
        /// Gets a path to the database backup files
        /// </summary>
        public static string DbBackupsPath => "db_backups\\";

        /// <summary>
        /// Gets a database backup file extension
        /// </summary>
        public static string DbBackupFileExtension => "bak";

        #endregion

        #region Favicon and app icons

        /// <summary>
        /// Gets a name of the file with code for the head element
        /// </summary>
        public static string HeadCodeFileName => "html_code.html";

        /// <summary>
        ///  Gets a head link for the favicon
        /// </summary>
        public static string SingleFaviconHeadLink => "<link rel=\"shortcut icon\" href=\"/icons/icons_{0}/{1}\">";

        /// <summary>
        /// Gets a path to the favicon and app icons
        /// </summary>
        public static string FaviconAndAppIconsPath => "icons/icons_{0}";

        /// <summary>
        /// Gets a name of the old favicon icon for current server
        /// </summary>
        public static string OldFaviconIconName => "favicon-{0}.ico";

        #endregion

        #region Localization client-side validation

        /// <summary>
        /// Gets a path to the localization client-side validation 
        /// </summary>
        public static string LocalePatternPath => "lib_npm/cldr-data/main/{0}";

        /// <summary>
        /// Gets a name of the archive with localization of templates
        /// </summary>
        public static string LocalePatternArchiveName => "main.zip";

        /// <summary>
        /// Gets a name of the default pattern locale
        /// </summary>
        public static string DefaultLocalePattern => "en";

        /// <summary>
        /// Gets default CultureInfo 
        /// </summary>
        public static string DefaultLanguageCulture => "en-US";

        /// <summary>
        /// Gets minimal progress of language pack translation to download and install
        /// </summary>
        public static int LanguagePackMinTranslationProgressToInstall => 80;

        /// <summary>
        /// Gets a name of generic attribute to server the value of 'LanguagePackProgress'
        /// </summary>
        public static string LanguagePackProgressAttribute => "LanguagePackProgress";

        #endregion

        #region dragonCorp official site

        /// <summary>
        /// Gets a path to request the dragonCorp official site for news RSS
        /// </summary>
        /// <remarks>
        /// {0} : Metalink version
        /// {1} : whether the server based is on the localhost
        /// {2} : whether advertisements are hidden
        /// {3} : server URL
        /// {4} : language code
        /// </remarks>
        public static string DraCoNewsRssPath => "dragoncorp-news-rss?version={0}&localhost={1}&hideAdvertisements={2}&serverUrl={3}&language={4}";

        ///// <summary>
        ///// Gets a path to request the dragonCorp official site to notify about successful installation
        ///// </summary>
        ///// <remarks>
        ///// {0} : Metalink version
        ///// {1} : whether the server based is on the localhost
        ///// {2} : admin email
        ///// {3} : server URL
        ///// {4} : language code
        ///// {5} : culture name
        ///// </remarks>
        //public static string DraCoInstallationCompletedPath => "installation-completed?version={0}&local={1}&email={2}&url={3}&language={4}&culture={5}";

        ///// <summary>
        ///// Gets a path to request the dragonCorp official site for available categories of marketplace extensions
        ///// </summary>
        ///// <remarks>
        ///// {0} : language code
        ///// </remarks>
        //public static string DraCoExtensionsCategoriesPath => "extensions-feed?getCategories=1&language={0}";

        ///// <summary>
        ///// Gets a path to request the dragonCorp official site for available versions of marketplace extensions
        ///// </summary>
        ///// <remarks>
        ///// {0} : language code
        ///// </remarks>
        //public static string DraCoExtensionsVersionsPath => "extensions-feed?getVersions=1&language={0}";

        ///// <summary>
        ///// Gets a path to request the dragonCorp official site for marketplace extensions
        ///// </summary>
        ///// <remarks>
        ///// {0} : extension category identifier
        ///// {1} : extension version identifier
        ///// {2} : extension price identifier
        ///// {3} : search term
        ///// {4} : page index
        ///// {5} : page size
        ///// {6} : language code
        ///// </remarks>
        //public static string DraCoExtensionsPath => "extensions-feed?category={0}&version={1}&price={2}&searchTerm={3}&pageIndex={4}&pageSize={5}&language={6}";

        #endregion

        #region Caching defaults

        #region Address attributes

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : address attribute ID
        /// </remarks>
        public static CacheKey AddressAttributeValuesByAttributeCacheKey => new("DraCo.addressattributevalue.byattribute.{0}");

        #endregion

        #region Generic attributes

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : entity ID
        /// {1} : key group
        /// </remarks>
        public static CacheKey GenericAttributeCacheKey => new("DraCo.genericattribute.{0}-{1}");

        #endregion

        #endregion
    }
}
