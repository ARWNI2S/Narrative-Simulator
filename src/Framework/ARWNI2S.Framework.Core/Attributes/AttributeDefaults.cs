using ARWNI2S.Caching;

namespace ARWNI2S.Framework.Attributes
{
    /// <summary>
    /// Represents default values related to attributes services
    /// </summary>
    public static partial class AttributeDefaults
    {
        #region Caching defaults

        /// <summary>
        /// Gets a key for caching attribute values of the attribute
        /// </summary>
        /// <remarks>
        /// {0} : attribute type
        /// {1} : attribute ID
        /// </remarks>
        public static CacheKey AttributeValuesByAttributeCacheKey => new("NI2S.attributevalue.byattribute.{0}.{1}");

        #endregion
    }
}