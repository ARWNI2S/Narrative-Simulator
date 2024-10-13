using ARWNI2S.Node.Core.Caching;
using ARWNI2S.Node.Data.Entities.Users;

namespace ARWNI2S.Portal.Services.Users
{
    /// <summary>
    /// Represents default values related to user services
    /// </summary>
    public static partial class UserServicesDefaults
    {
        /// <summary>
        /// Gets a password salt key size
        /// </summary>
        public static int PasswordSaltKeySize => 5;

        /// <summary>
        /// Gets a max username length
        /// </summary>
        public static int UserUsernameLength => 100;

        /// <summary>
        /// Gets a default hash format for user password
        /// </summary>
        public static string DefaultHashedPasswordFormat => "SHA512";

        /// <summary>
        /// Gets default prefix for user
        /// </summary>
        public static string UserAttributePrefix => "user_attribute_";

        #region Caching defaults

        #region User

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : system name
        /// </remarks>
        public static CacheKey UserBySystemNameCacheKey => new("DraCo.user.bysystemname.{0}");

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : user GUID
        /// </remarks>
        public static CacheKey UserByGuidCacheKey => new("DraCo.user.byguid.{0}");

        #endregion

        #region User attributes

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : user attribute ID
        /// </remarks>
        public static CacheKey UserAttributeValuesByAttributeCacheKey => new("DraCo.userattributevalue.byattribute.{0}");

        #endregion

        #region User roles

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        public static CacheKey UserRolesAllCacheKey => new("DraCo.userrole.all.{0}", EntityCacheDefaults<UserRole>.AllPrefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : system name
        /// </remarks>
        public static CacheKey UserRolesBySystemNameCacheKey => new("DraCo.userrole.bysystemname.{0}", UserRolesBySystemNamePrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string UserRolesBySystemNamePrefix => "DraCo.userrole.bysystemname.";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : user identifier
        /// {1} : show hidden
        /// </remarks>
        public static CacheKey UserRoleIdsCacheKey => new("DraCo.user.userrole.ids.{0}-{1}", UserUserRolesPrefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : user identifier
        /// {1} : show hidden
        /// </remarks>
        public static CacheKey UserRolesCacheKey => new("DraCo.user.userrole.{0}-{1}", UserUserRolesByUserPrefix, UserUserRolesPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string UserUserRolesPrefix => "DraCo.user.userrole.";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : user identifier
        /// </remarks>
        public static string UserUserRolesByUserPrefix => "DraCo.user.userrole.{0}";

        #endregion

        #region Addresses

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : user identifier
        /// </remarks>
        public static CacheKey UserAddressesCacheKey => new("DraCo.user.addresses.{0}", UserAddressesPrefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : user identifier
        /// {1} : address identifier
        /// </remarks>
        public static CacheKey UserAddressCacheKey => new("DraCo.user.addresses.{0}-{1}", UserAddressesByUserPrefix, UserAddressesPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string UserAddressesPrefix => "DraCo.user.addresses.";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : user identifier
        /// </remarks>
        public static string UserAddressesByUserPrefix => "DraCo.user.addresses.{0}";

        #endregion

        #region User password

        /// <summary>
        /// Gets a key for caching current user password lifetime
        /// </summary>
        /// <remarks>
        /// {0} : user identifier
        /// </remarks>
        public static CacheKey UserPasswordLifetimeCacheKey => new("DraCo.userpassword.lifetime.{0}");

        #endregion

        #endregion

    }
}