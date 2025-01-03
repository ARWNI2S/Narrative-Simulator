using ARWNI2S.Caching;
using ARWNI2S.Engine.Data;
using ARWNI2S.Environment;
using ARWNI2S.Framework.Localization;
using ARWNI2S.Framework.Users.Entities;
using ARWNI2S.Framework.Users.Security.Entities;

namespace ARWNI2S.Framework.Users.Security
{
    /// <summary>
    /// Permission service
    /// </summary>
    public partial class PermissionService : IPermissionService
    {
        #region Fields

        protected readonly IUserService _userService;
        protected readonly ILocalizationService _localizationService;
        protected readonly IRepository<UserRole> _userRoleRepository;
        protected readonly IRepository<PermissionRecord> _permissionRecordRepository;
        protected readonly IRepository<PermissionRecordUserRoleMapping> _permissionRecordUserRoleMappingRepository;
        protected readonly IStaticCacheManager _staticCacheManager;
        protected readonly ITypeFinder _typeFinder;
        protected readonly IWorkingContext _workingContext;

        #endregion

        #region Ctor

        public PermissionService(IUserService userService,
            ILocalizationService localizationService,
            IRepository<UserRole> userRoleRepository,
            IRepository<PermissionRecord> permissionRecordRepository,
            IRepository<PermissionRecordUserRoleMapping> permissionRecordUserRoleMappingRepository,
            IStaticCacheManager staticCacheManager,
            ITypeFinder typeFinder,
            IWorkingContext workingContext)
        {
            _userService = userService;
            _localizationService = localizationService;
            _userRoleRepository = userRoleRepository;
            _permissionRecordRepository = permissionRecordRepository;
            _permissionRecordUserRoleMappingRepository = permissionRecordUserRoleMappingRepository;
            _staticCacheManager = staticCacheManager;
            _typeFinder = typeFinder;
            _workingContext = workingContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get permission records by user role identifier
        /// </summary>
        /// <param name="userRoleId">User role identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the permissions
        /// </returns>
        protected virtual async Task<IList<PermissionRecord>> GetPermissionRecordsByUserRoleIdAsync(int userRoleId)
        {
            var key = _staticCacheManager.PrepareKeyForDefaultCache(UserSecurityDefaults.PermissionRecordsAllCacheKey, userRoleId);

            var query = from pr in _permissionRecordRepository.Table
                        join prcrm in _permissionRecordUserRoleMappingRepository.Table on pr.Id equals prcrm
                            .PermissionRecordId
                        where prcrm.UserRoleId == userRoleId
                        orderby pr.Id
                        select pr;

            return await _staticCacheManager.GetAsync(key, async () => await query.ToListAsync());
        }

        /// <summary>
        /// Gets a permission
        /// </summary>
        /// <param name="systemName">Permission system name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the permission
        /// </returns>
        protected virtual async Task<PermissionRecord> GetPermissionRecordBySystemNameAsync(string systemName)
        {
            if (string.IsNullOrWhiteSpace(systemName))
                return null;

            var query = from pr in _permissionRecordRepository.Table
                        where pr.SystemName == systemName
                        orderby pr.Id
                        select pr;

            var permissionRecord = await query.FirstOrDefaultAsync();
            return permissionRecord;
        }

        /// <summary>
        /// Insert permissions by list of permission configs
        /// </summary>
        /// <param name="configs">Permission configs</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task InstallPermissionsAsync(IList<PermissionConfig> configs)
        {
            if (!configs?.Any() ?? true)
                return;

            var exists =
                await _permissionRecordUserRoleMappingRepository.GetAllAsync(query => query, getCacheKey: _ => default);

            async Task addPermissionRecordUserRoleMappingIfNotExists(
                PermissionRecordUserRoleMapping permissionRecordUserRoleMapping)
            {
                var mapping = exists.FirstOrDefault(m =>
                    m.UserRoleId == permissionRecordUserRoleMapping.UserRoleId &&
                    m.PermissionRecordId == permissionRecordUserRoleMapping.PermissionRecordId);

                if (mapping != null)
                {
                    permissionRecordUserRoleMapping.Id = mapping.Id;

                    return;
                }

                await _permissionRecordUserRoleMappingRepository.InsertAsync(permissionRecordUserRoleMapping, false);
                exists.Add(permissionRecordUserRoleMapping);
            }

            foreach (var config in configs)
            {
                //new permission (install it)
                var permission = new PermissionRecord
                {
                    Name = config.Name,
                    SystemName = config.SystemName,
                    Category = config.Category
                };

                //save new permission
                await _permissionRecordRepository.InsertAsync(permission);

                foreach (var systemRoleName in config.DefaultUserRoles)
                {
                    var userRole = await GetUserRoleBySystemNameAsync(systemRoleName);

                    if (userRole == null)
                    {
                        //new role (save it)
                        userRole = new UserRole
                        {
                            Name = systemRoleName,
                            Active = true,
                            SystemName = systemRoleName
                        };

                        await _userRoleRepository.InsertAsync(userRole);
                    }

                    await addPermissionRecordUserRoleMappingIfNotExists(new PermissionRecordUserRoleMapping { UserRoleId = userRole.Id, PermissionRecordId = permission.Id });
                }

                //save localization
                await _localizationService.SaveLocalizedPermissionNameAsync(permission);
            }
        }

        /// <summary>
        /// Gets a user role
        /// </summary>
        /// <param name="systemName">User role system name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the user role
        /// </returns>
        protected virtual async Task<UserRole> GetUserRoleBySystemNameAsync(string systemName)
        {
            if (string.IsNullOrWhiteSpace(systemName))
                return null;

            var key = _staticCacheManager.PrepareKeyForDefaultCache(NopUserServicesDefaults.UserRolesBySystemNameCacheKey, systemName);

            var query = from cr in _userRoleRepository.Table
                        orderby cr.Id
                        where cr.SystemName == systemName
                        select cr;

            var userRole = await _staticCacheManager.GetAsync(key, async () => await query.FirstOrDefaultAsync());

            return userRole;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets all permissions
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the permissions
        /// </returns>
        public virtual async Task<IList<PermissionRecord>> GetAllPermissionRecordsAsync()
        {
            var permissions = await _permissionRecordRepository.GetAllAsync(query => from pr in query
                                                                                     orderby pr.Name
                                                                                     select pr);

            return permissions;
        }

        /// <summary>
        /// Inserts a permission
        /// </summary>
        /// <param name="permission">Permission</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertPermissionRecordAsync(PermissionRecord permission)
        {
            await _permissionRecordRepository.InsertAsync(permission);
        }

        /// <summary>
        /// Gets a permission record by identifier
        /// </summary>
        /// <param name="permissionId">Permission identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a permission record
        /// </returns>
        public virtual async Task<PermissionRecord> GetPermissionRecordByIdAsync(int permissionId)
        {
            return await _permissionRecordRepository.GetByIdAsync(permissionId);
        }

        /// <summary>
        /// Updates the permission
        /// </summary>
        /// <param name="permission">Permission</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdatePermissionRecordAsync(PermissionRecord permission)
        {
            await _permissionRecordRepository.UpdateAsync(permission);
        }

        /// <summary>
        /// Delete a permission
        /// </summary>
        /// <param name="permission">Permission</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeletePermissionRecordAsync(PermissionRecord permission)
        {
            await _permissionRecordRepository.DeleteAsync(permission);
        }

        /// <summary>
        /// Delete a permission
        /// </summary>
        /// <param name="permissionSystemName">Permission system name</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeletePermissionAsync(string permissionSystemName)
        {
            var permission = await GetPermissionRecordBySystemNameAsync(permissionSystemName);

            if (permission == null)
                return;

            var mapping = await GetMappingByPermissionRecordIdAsync(permission.Id);

            await _permissionRecordUserRoleMappingRepository.DeleteAsync(mapping);
            await _localizationService.DeleteLocalizedPermissionNameAsync(permission);
            await _permissionRecordRepository.DeleteAsync(permission);
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permission">Permission record</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the true - authorized; otherwise, false
        /// </returns>
        public virtual async Task<bool> AuthorizeAsync(PermissionRecord permission)
        {
            return await AuthorizeAsync(permission, await _workingContext.GetCurrentUserAsync());
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permission">Permission record</param>
        /// <param name="user">User</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the true - authorized; otherwise, false
        /// </returns>
        public virtual async Task<bool> AuthorizeAsync(PermissionRecord permission, User user)
        {
            if (permission == null)
                return false;

            if (user == null)
                return false;

            return await AuthorizeAsync(permission.SystemName, user);
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the true - authorized; otherwise, false
        /// </returns>
        public virtual async Task<bool> AuthorizeAsync(string permissionRecordSystemName)
        {
            return await AuthorizeAsync(permissionRecordSystemName, await _workingContext.GetCurrentUserAsync());
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <param name="user">User</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the true - authorized; otherwise, false
        /// </returns>
        public virtual async Task<bool> AuthorizeAsync(string permissionRecordSystemName, User user)
        {
            if (string.IsNullOrEmpty(permissionRecordSystemName))
                return false;

            var userRoles = await _userService.GetUserRolesAsync(user);
            foreach (var role in userRoles)
                if (await AuthorizeAsync(permissionRecordSystemName, role.Id))
                    //yes, we have such permission
                    return true;

            //no permission found
            return false;
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <param name="userRoleId">User role identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the true - authorized; otherwise, false
        /// </returns>
        public virtual async Task<bool> AuthorizeAsync(string permissionRecordSystemName, int userRoleId)
        {
            if (string.IsNullOrEmpty(permissionRecordSystemName))
                return false;

            var key = _staticCacheManager.PrepareKeyForDefaultCache(UserSecurityDefaults.PermissionAllowedCacheKey, permissionRecordSystemName, userRoleId);

            return await _staticCacheManager.GetAsync(key, async () =>
            {
                var permissions = await GetPermissionRecordsByUserRoleIdAsync(userRoleId);
                foreach (var permission in permissions)
                    if (permission.SystemName.Equals(permissionRecordSystemName, StringComparison.InvariantCultureIgnoreCase))
                        return true;

                return false;
            });
        }

        /// <summary>
        /// Gets a permission record-user role mapping
        /// </summary>
        /// <param name="permissionId">Permission identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a list of mappings
        /// </returns>
        public virtual async Task<IList<PermissionRecordUserRoleMapping>> GetMappingByPermissionRecordIdAsync(int permissionId)
        {
            var query = _permissionRecordUserRoleMappingRepository.Table;

            query = query.Where(x => x.PermissionRecordId == permissionId);

            return await query.ToListAsync();
        }

        /// <summary>
        /// Delete a permission record-user role mapping
        /// </summary>
        /// <param name="permissionId">Permission identifier</param>
        /// <param name="userRoleId">User role identifier</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeletePermissionRecordUserRoleMappingAsync(int permissionId, int userRoleId)
        {
            var mapping = await _permissionRecordUserRoleMappingRepository.Table
                .FirstOrDefaultAsync(prcm => prcm.UserRoleId == userRoleId && prcm.PermissionRecordId == permissionId);
            if (mapping is null)
                return;

            await _permissionRecordUserRoleMappingRepository.DeleteAsync(mapping);
        }

        /// <summary>
        /// Inserts a permission record-user role mapping
        /// </summary>
        /// <param name="permissionRecordUserRoleMapping">Permission record-user role mapping</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertPermissionRecordUserRoleMappingAsync(PermissionRecordUserRoleMapping permissionRecordUserRoleMapping)
        {
            await _permissionRecordUserRoleMappingRepository.InsertAsync(permissionRecordUserRoleMapping);
        }

        /// <summary>
        /// Configure permission manager
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertPermissionsAsync()
        {
            var permissionRecords = (await _permissionRecordRepository.GetAllAsync(query => query, getCacheKey: _ => null)).Distinct().ToHashSet();
            var exists = permissionRecords.Select(p => p.SystemName).ToHashSet();

            var configs = _typeFinder.FindClassesOfType<IPermissionConfigManager>()
                .Select(configType => (IPermissionConfigManager)Activator.CreateInstance(configType))
                .SelectMany(config => config?.AllConfigs ?? new List<PermissionConfig>())
                .Where(c => !exists.Contains(c.SystemName))
                .ToList();

            await InstallPermissionsAsync(configs);
        }

        /// <summary>
        /// Inserts a permission record-user role mappings
        /// </summary>
        /// <param name="userRoleId">User role ID</param>
        /// <param name="permissions">Permissions</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertPermissionMappingAsync(int userRoleId, params string[] permissions)
        {
            var permissionRecords = await GetAllPermissionRecordsAsync();

            foreach (var permissionSystemName in permissions)
            {
                var permission = permissionRecords.FirstOrDefault(p =>
                    p.SystemName.Equals(permissionSystemName, StringComparison.CurrentCultureIgnoreCase));

                if (permission == null)
                    continue;

                await InsertPermissionRecordUserRoleMappingAsync(
                    new PermissionRecordUserRoleMapping
                    {
                        UserRoleId = userRoleId,
                        PermissionRecordId = permission.Id
                    });
            }
        }

        #endregion
    }
}
