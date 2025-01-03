using ARWNI2S.Framework.Users.Entities;
using ARWNI2S.Framework.Users.Security.Entities;

namespace ARWNI2S.Framework.Users.Security
{
    /// <summary>
    /// Permission service interface
    /// </summary>
    public partial interface IPermissionService
    {
        /// <summary>
        /// Gets all permissions
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the permissions
        /// </returns>
        Task<IList<PermissionRecord>> GetAllPermissionRecordsAsync();

        /// <summary>
        /// Inserts a permission
        /// </summary>
        /// <param name="permission">Permission</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertPermissionRecordAsync(PermissionRecord permission);

        /// <summary>
        /// Gets a permission record by identifier
        /// </summary>
        /// <param name="permissionId">Permission identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a permission record
        /// </returns>
        Task<PermissionRecord> GetPermissionRecordByIdAsync(int permissionId);

        /// <summary>
        /// Updates the permission
        /// </summary>
        /// <param name="permission">Permission</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdatePermissionRecordAsync(PermissionRecord permission);

        /// <summary>
        /// Deletes the permission
        /// </summary>
        /// <param name="permission">Permission</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeletePermissionRecordAsync(PermissionRecord permission);

        /// <summary>
        /// Delete a permission
        /// </summary>
        /// <param name="permissionSystemName">Permission system name</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeletePermissionAsync(string permissionSystemName);

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permission">Permission record</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains true - authorized; otherwise, false
        /// </returns>
        Task<bool> AuthorizeAsync(PermissionRecord permission);

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permission">Permission record</param>
        /// <param name="user">User</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains true - authorized; otherwise, false
        /// </returns>
        Task<bool> AuthorizeAsync(PermissionRecord permission, User user);

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains true - authorized; otherwise, false
        /// </returns>
        Task<bool> AuthorizeAsync(string permissionRecordSystemName);

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <param name="user">User</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains true - authorized; otherwise, false
        /// </returns>
        Task<bool> AuthorizeAsync(string permissionRecordSystemName, User user);

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <param name="userRoleId">User role identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains true - authorized; otherwise, false
        /// </returns>
        Task<bool> AuthorizeAsync(string permissionRecordSystemName, int userRoleId);

        /// <summary>
        /// Gets a permission record-user role mapping
        /// </summary>
        /// <param name="permissionId">Permission identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a list of mappings
        /// </returns>
        Task<IList<PermissionRecordUserRoleMapping>> GetMappingByPermissionRecordIdAsync(int permissionId);

        /// <summary>
        /// Delete a permission record-user role mapping
        /// </summary>
        /// <param name="permissionId">Permission identifier</param>
        /// <param name="userRoleId">User role identifier</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeletePermissionRecordUserRoleMappingAsync(int permissionId, int userRoleId);

        /// <summary>
        /// Inserts a permission record-user role mapping
        /// </summary>
        /// <param name="permissionRecordUserRoleMapping">Permission record-user role mapping</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertPermissionRecordUserRoleMappingAsync(PermissionRecordUserRoleMapping permissionRecordUserRoleMapping);

        /// <summary>
        /// Insert permissions
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertPermissionsAsync();

        /// <summary>
        /// Inserts a permission record-user role mappings
        /// </summary>
        /// <param name="userRoleId">User role ID</param>
        /// <param name="permissions">Permissions</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertPermissionMappingAsync(int userRoleId, params string[] permissions);
    }
}