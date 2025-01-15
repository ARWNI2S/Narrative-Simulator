using ARWNI2S.Caching;
using ARWNI2S.Engine.Caching;

namespace ARWNI2S.Framework.Localization
{
    /// <summary>
    /// Represents default values related to localization services
    /// </summary>
    public static partial class LocalizationDefaults
    {
        #region Locales

        /// <summary>
        /// Gets a prefix of locale resources for the admin area
        /// </summary>
        public static string AdminLocaleStringResourcesPrefix => "Admin.";

        /// <summary>
        /// Gets a prefix of locale resources for enumerations 
        /// </summary>
        public static string EnumLocaleStringResourcesPrefix => "Enums.";

        /// <summary>
        /// Gets a prefix of locale resources for permissions 
        /// </summary>
        public static string PermissionLocaleStringResourcesPrefix => "Security.Permission.";

        /// <summary>
        /// Gets a prefix of locale resources for plugin friendly names 
        /// </summary>
        public static string PluginNameLocaleStringResourcesPrefix => "Plugins.FriendlyName.";

        #endregion

        #region Caching defaults

        #region Languages

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : node ID
        /// {1} : show hidden records?
        /// </remarks>
        public static CacheKey LanguagesAllCacheKey => new("NI2S.language.all.{0}-{1}", LanguagesByNodePrefix, EntityCacheDefaults<Language>.AllPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : node ID
        /// </remarks>
        public static string LanguagesByNodePrefix => "NI2S.language.all.{0}";

        #endregion

        #region Locales

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        public static CacheKey LocaleStringResourcesAllPublicCacheKey => new("NI2S.localestringresource.bylanguage.public.{0}", EntityCacheDefaults<LocaleStringResource>.Prefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        public static CacheKey LocaleStringResourcesAllAdminCacheKey => new("NI2S.localestringresource.bylanguage.admin.{0}", EntityCacheDefaults<LocaleStringResource>.Prefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        public static CacheKey LocaleStringResourcesAllCacheKey => new("NI2S.localestringresource.bylanguage.{0}", EntityCacheDefaults<LocaleStringResource>.Prefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : resource key
        /// </remarks>
        public static CacheKey LocaleStringResourcesByNameCacheKey => new("NI2S.localestringresource.byname.{0}-{1}", LocaleStringResourcesByNamePrefix, EntityCacheDefaults<LocaleStringResource>.Prefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        public static string LocaleStringResourcesByNamePrefix => "NI2S.localestringresource.byname.{0}";

        #endregion

        #region Localized properties

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : entity ID
        /// {2} : locale key group
        /// {3} : locale key
        /// </remarks>
        public static CacheKey LocalizedPropertyCacheKey => new("NI2S.localizedproperty.value.{0}-{1}-{2}-{3}");

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : entity ID
        /// {1} : locale key group
        /// {2} : locale key
        /// </remarks>
        public static CacheKey LocalizedPropertiesCacheKey => new("NI2S.localizedproperty.all.{0}-{1}-{2}");

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        public static CacheKey LocalizedPropertyLookupCacheKey => new("NI2S.localizedproperty.value.{0}");

        #endregion

        #endregion
    }
}