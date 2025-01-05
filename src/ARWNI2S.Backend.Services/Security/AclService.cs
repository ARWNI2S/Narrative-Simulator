using ARWNI2S.Backend.Services.Security.Entities;
using ARWNI2S.Backend.Services.Users;
using ARWNI2S.Backend.Services.Users.Entities;
using ARWNI2S.Caching;
using ARWNI2S.Engine.Data;
using ARWNI2S.Engine.Data.Entities;
using ARWNI2S.Environment;
using LinqToDB;

namespace ARWNI2S.Backend.Services.Security
{
    /// <summary>
    /// ACL service
    /// </summary>
    public partial class AclService : IAclService
    {
        #region Fields

        protected readonly CatalogSettings _catalogSettings;
        protected readonly IUserService _userService;
        protected readonly INiisDataProvider _dataProvider;
        protected readonly IRepository<AclRecord> _aclRecordRepository;
        protected readonly IStaticCacheManager _staticCacheManager;
        protected readonly Lazy<IWorkingContext> _workingContext;

        #endregion

        #region Ctor

        public AclService(CatalogSettings catalogSettings,
            IUserService userService,
            INiisDataProvider dataProvider,
            IRepository<AclRecord> aclRecordRepository,
            IStaticCacheManager staticCacheManager,
            Lazy<IWorkingContext> workingContext)
        {
            _catalogSettings = catalogSettings;
            _userService = userService;
            _dataProvider = dataProvider;
            _aclRecordRepository = aclRecordRepository;
            _staticCacheManager = staticCacheManager;
            _workingContext = workingContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Inserts an ACL record
        /// </summary>
        /// <param name="aclRecord">ACL record</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task InsertAclRecordAsync(AclRecord aclRecord)
        {
            await _aclRecordRepository.InsertAsync(aclRecord);
        }

        /// <summary>
        /// Get a value indicating whether any ACL records exist for entity type are related to user roles
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains true if exist; otherwise false
        /// </returns>
        protected virtual async Task<bool> IsEntityAclMappingExistAsync<TEntity>() where TEntity : DataEntity, IAclSupported
        {
            var entityName = typeof(TEntity).Name;
            var key = _staticCacheManager.PrepareKeyForDefaultCache(SecurityDefaults.EntityAclRecordExistsCacheKey, entityName);

            var query = from acl in _aclRecordRepository.Table
                        where acl.EntityName == entityName
                        select acl;

            return await _staticCacheManager.GetAsync(key, query.Any);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Apply ACL to the passed query
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
        /// <param name="query">Query to filter</param>
        /// <param name="user">User</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the filtered query
        /// </returns>
        public virtual async Task<IQueryable<TEntity>> ApplyAcl<TEntity>(IQueryable<TEntity> query, User user)
            where TEntity : DataEntity, IAclSupported
        {
            ArgumentNullException.ThrowIfNull(query);

            ArgumentNullException.ThrowIfNull(user);

            var userRoleIds = await _userService.GetUserRoleIdsAsync(user);
            return await ApplyAcl(query, userRoleIds);
        }

        /// <summary>
        /// Apply ACL to the passed query
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
        /// <param name="query">Query to filter</param>
        /// <param name="userRoleIds">Identifiers of user's roles</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the filtered query
        /// </returns>
        public virtual async Task<IQueryable<TEntity>> ApplyAcl<TEntity>(IQueryable<TEntity> query, int[] userRoleIds)
            where TEntity : DataEntity, IAclSupported
        {
            ArgumentNullException.ThrowIfNull(query);

            ArgumentNullException.ThrowIfNull(userRoleIds);

            if (!userRoleIds.Any() || _catalogSettings.IgnoreAcl || !await IsEntityAclMappingExistAsync<TEntity>())
                return query;

            return from entity in query
                   where !entity.SubjectToAcl || _aclRecordRepository.Table.Any(acl =>
                       acl.EntityName == typeof(TEntity).Name && acl.EntityId == entity.Id && userRoleIds.Contains(acl.UserRoleId))
                   select entity;
        }

        /// <summary>
        /// Deletes an ACL record
        /// </summary>
        /// <param name="aclRecord">ACL record</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteAclRecordAsync(AclRecord aclRecord)
        {
            await _aclRecordRepository.DeleteAsync(aclRecord);
        }

        /// <summary>
        /// Gets ACL records
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ACL records
        /// </returns>
        public virtual async Task<IList<AclRecord>> GetAclRecordsAsync<TEntity>(TEntity entity) where TEntity : DataEntity, IAclSupported
        {
            ArgumentNullException.ThrowIfNull(entity);

            var entityId = entity.Id;
            var entityName = entity.GetType().Name;

            var query = from ur in _aclRecordRepository.Table
                        where ur.EntityId == entityId &&
                              ur.EntityName == entityName
                        select ur;
            var aclRecords = await query.ToListAsync();

            return aclRecords;
        }

        /// <summary>
        /// Inserts an ACL record
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="userRoleId">User role id</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertAclRecordAsync<TEntity>(TEntity entity, int userRoleId) where TEntity : DataEntity, IAclSupported
        {
            ArgumentNullException.ThrowIfNull(entity);

            if (userRoleId == 0)
                throw new ArgumentOutOfRangeException(nameof(userRoleId));

            var entityId = entity.Id;
            var entityName = entity.GetType().Name;

            var aclRecord = new AclRecord
            {
                EntityId = entityId,
                EntityName = entityName,
                UserRoleId = userRoleId
            };

            await InsertAclRecordAsync(aclRecord);
        }

        /// <summary>
        /// Find user role identifiers with granted access
        /// </summary>
        /// <param name="entityId">Entity ID</param>
        /// <param name="entityName">Entity name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the user role identifiers
        /// </returns>
        public virtual async Task<int[]> GetUserRoleIdsWithAccessAsync(int entityId, string entityName)
        {
            if (entityId == 0)
                return [];

            var key = _staticCacheManager.PrepareKeyForDefaultCache(SecurityDefaults.AclRecordCacheKey, entityId, entityName);

            var query = from ur in _aclRecordRepository.Table
                        where ur.EntityId == entityId &&
                              ur.EntityName == entityName
                        select ur.UserRoleId;

            return await _staticCacheManager.GetAsync(key, () => query.ToArrayAsync());
        }

        /// <summary>
        /// Authorize ACL permission
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains true - authorized; otherwise, false
        /// </returns>
        public virtual async Task<bool> AuthorizeAsync<TEntity>(TEntity entity) where TEntity : DataEntity, IAclSupported
        {
            return await AuthorizeAsync(entity, await _workingContext.Value.GetCurrentUserAsync());
        }

        /// <summary>
        /// Authorize ACL permission
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="user">User</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains true - authorized; otherwise, false
        /// </returns>
        public virtual async Task<bool> AuthorizeAsync<TEntity>(TEntity entity, User user) where TEntity : DataEntity, IAclSupported
        {
            if (entity == null)
                return false;

            if (!entity.SubjectToAcl)
                return true;

            return await AuthorizeAsync(entity.GetType().Name, entity.Id, user);
        }

        /// <summary>
        /// Authorize ACL permission
        /// </summary>
        /// <param name="entityTypeName">Type name of entity that supports the ACL</param>
        /// <param name="entityId">Entity ID</param>
        /// <param name="user">User</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains true - authorized; otherwise, false
        /// </returns>
        public virtual async Task<bool> AuthorizeAsync(string entityTypeName, int entityId, User user)
        {
            if (string.IsNullOrEmpty(entityTypeName))
                return false;

            if (entityId <= 0)
                return false;

            if (user == null)
                return false;

            if (_catalogSettings.IgnoreAcl)
                return true;

            foreach (var role1 in await _userService.GetUserRolesAsync(user))
                foreach (var role2Id in await GetUserRoleIdsWithAccessAsync(entityId, entityTypeName))
                    if (role1.Id == role2Id)
                        //yes, we have such permission
                        return true;

            //no permission found
            return false;
        }

        /// <summary>
        /// Authorize ACL permission
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="allowedUserRoleIds">List of allowed user role IDs</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains true - authorized; otherwise, false
        /// </returns>
        public virtual async Task<bool> AuthorizeAsync(User user, IList<int> allowedUserRoleIds)
        {
            return _catalogSettings.IgnoreAcl || allowedUserRoleIds.Intersect(await _userService.GetUserRoleIdsAsync(user)).Any();
        }

        /// <summary>
        /// Save ACL mapping
        /// </summary>
        /// <typeparam name="TEntity">Type of entity</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="selectedUserRoleIds">User roles for mapping</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task SaveAclAsync<TEntity>(TEntity entity, IList<int> selectedUserRoleIds) where TEntity : DataEntity, IAclSupported
        {
            if (entity == null)
                return;

            if (entity.SubjectToAcl != selectedUserRoleIds.Any())
            {
                entity.SubjectToAcl = selectedUserRoleIds.Any();
                await _dataProvider.UpdateEntityAsync(entity);
            }

            var existingAclRecords = await GetAclRecordsAsync(entity);
            var allUserRoles = await _userService.GetAllUserRolesAsync(true);
            foreach (var userRole in allUserRoles)
                if (selectedUserRoleIds.Contains(userRole.Id))
                {
                    //new role
                    if (existingAclRecords.All(acl => acl.UserRoleId != userRole.Id))
                        await InsertAclRecordAsync(entity, userRole.Id);
                }
                else
                {
                    //remove role
                    var aclRecordToDelete = existingAclRecords.FirstOrDefault(acl => acl.UserRoleId == userRole.Id);
                    if (aclRecordToDelete != null)
                        await DeleteAclRecordAsync(aclRecordToDelete);
                }
        }

        #endregion
    }
}