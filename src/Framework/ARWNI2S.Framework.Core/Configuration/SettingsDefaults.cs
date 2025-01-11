using ARWNI2S.Caching;
using ARWNI2S.Engine.Caching;

namespace ARWNI2S.Framework.Configuration
{
    /// <summary>
    /// Represents default values related to settings
    /// </summary>
    public static partial class SettingsDefaults
    {
        #region Caching defaults

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey SettingsAllAsDictionaryCacheKey => new("NI2S.setting.all.dictionary.", EntityCacheDefaults<Setting>.Prefix);

        #endregion
    }
}