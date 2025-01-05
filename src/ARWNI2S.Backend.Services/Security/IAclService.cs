using ARWNI2S.Backend.Services.Security.Entities;
using ARWNI2S.Backend.Services.Users.Entities;
using ARWNI2S.Engine.Data.Entities;

namespace ARWNI2S.Backend.Services.Security
{
    /// <summary>
    /// ACL service interface
    /// </summary>
    public partial interface IAclService
    {
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
        Task<IQueryable<TEntity>> ApplyAcl<TEntity>(IQueryable<TEntity> query, User user) where TEntity : DataEntity, IAclSupported;

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
        Task<IQueryable<TEntity>> ApplyAcl<TEntity>(IQueryable<TEntity> query, int[] userRoleIds) where TEntity : DataEntity, IAclSupported;

        /// <summary>
        /// Deletes an ACL record
        /// </summary>
        /// <param name="aclRecord">ACL record</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteAclRecordAsync(AclRecord aclRecord);

        /// <summary>
        /// Gets ACL records
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ACL records
        /// </returns>
        Task<IList<AclRecord>> GetAclRecordsAsync<TEntity>(TEntity entity) where TEntity : DataEntity, IAclSupported;

        /// <summary>
        /// Inserts an ACL record
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="userRoleId">User role id</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertAclRecordAsync<TEntity>(TEntity entity, int userRoleId) where TEntity : DataEntity, IAclSupported;

        /// <summary>
        /// Find user role identifiers with granted access
        /// </summary>
        /// <param name="entityId">Entity ID</param>
        /// <param name="entityName">Entity name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the user role identifiers
        /// </returns>
        Task<int[]> GetUserRoleIdsWithAccessAsync(int entityId, string entityName);

        /// <summary>
        /// Authorize ACL permission
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains true - authorized; otherwise, false
        /// </returns>
        Task<bool> AuthorizeAsync<TEntity>(TEntity entity) where TEntity : DataEntity, IAclSupported;

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
        Task<bool> AuthorizeAsync<TEntity>(TEntity entity, User user) where TEntity : DataEntity, IAclSupported;

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
        Task<bool> AuthorizeAsync(string entityTypeName, int entityId, User user);

        /// <summary>
        /// Authorize ACL permission
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="allowedUserRoleIds">List of allowed user role IDs</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains true - authorized; otherwise, false
        /// </returns>
        Task<bool> AuthorizeAsync(User user, IList<int> allowedUserRoleIds);

        /// <summary>
        /// Save ACL mapping
        /// </summary>
        /// <typeparam name="TEntity">Type of entity</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="selectedUserRoleIds">User roles for mapping</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task SaveAclAsync<TEntity>(TEntity entity, IList<int> selectedUserRoleIds) where TEntity : DataEntity, IAclSupported;
    }
}