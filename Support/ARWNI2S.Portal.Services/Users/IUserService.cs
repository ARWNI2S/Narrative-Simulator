//using DragonCorp.Metalink.Core.Domain.Orders;
using ARWNI2S.Infrastructure.Collections.Generic;
using ARWNI2S.Node.Data.Entities.Users;

namespace ARWNI2S.Protal.Services.Users
{
    /// <summary>
    /// User service interface
    /// </summary>
    public partial interface IUserService
    {
        #region Users

        /// <summary>
        /// Gets all users
        /// </summary>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="lastActivityFromUtc">Last activity date from (UTC); null to load all records</param>
        /// <param name="lastActivityToUtc">Last activity date to (UTC); null to load all records</param>
        /// <param name="affiliateId">Affiliate identifier</param>
        /// <param name="partnerId">Partner identifier</param>
        /// <param name="userRoleIds">A list of user role identifiers to filter by (at least one match); pass null or empty list in order to load all users; </param>
        /// <param name="email">Email; null to load all users</param>
        /// <param name="username">Username; null to load all users</param>
        /// <param name="firstName">First name; null to load all users</param>
        /// <param name="lastName">Last name; null to load all users</param>
        /// <param name="dayOfBirth">Day of birth; 0 to load all users</param>
        /// <param name="monthOfBirth">Month of birth; 0 to load all users</param>
        /// <param name="company">Company; null to load all users</param>
        /// <param name="phone">Phone; null to load all users</param>
        /// <param name="zipPostalCode">Phone; null to load all users</param>
        /// <param name="ipAddress">IP address; null to load all users</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="getOnlyTotalCount">A value in indicating whether you want to load only total number of records. Set to "true" if you don't want to load data from database</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the users
        /// </returns>
        Task<IPagedList<User>> GetAllUsersAsync(DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            DateTime? lastActivityFromUtc = null, DateTime? lastActivityToUtc = null,
            int affiliateId = 0, int partnerId = 0, int[] userRoleIds = null,
            string email = null, string username = null, string firstName = null, string lastName = null,
            int dayOfBirth = 0, int monthOfBirth = 0,
            string company = null, string phone = null, string zipPostalCode = null, string ipAddress = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);

        /// <summary>
        /// Gets online users
        /// </summary>
        /// <param name="lastActivityFromUtc">User last activity date (from)</param>
        /// <param name="userRoleIds">A list of user role identifiers to filter by (at least one match); pass null or empty list in order to load all users; </param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the users
        /// </returns>
        Task<IPagedList<User>> GetOnlineUsersAsync(DateTime lastActivityFromUtc,
            int[] userRoleIds, int pageIndex = 0, int pageSize = int.MaxValue);

        ///// <summary>
        ///// Gets users with shopping carts
        ///// </summary>
        ///// <param name="shoppingCartType">Shopping cart type; pass null to load all records</param>
        ///// <param name="nodeId">Node identifier; pass 0 to load all records</param>
        ///// <param name="productId">Product identifier; pass null to load all records</param>
        ///// <param name="createdFromUtc">Created date from (UTC); pass null to load all records</param>
        ///// <param name="createdToUtc">Created date to (UTC); pass null to load all records</param>
        ///// <param name="countryId">Billing country identifier; pass null to load all records</param>
        ///// <param name="pageIndex">Page index</param>
        ///// <param name="pageSize">Page size</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the users
        ///// </returns>
        //Task<IPagedList<User>> GetUsersWithShoppingCartsAsync(ShoppingCartType? shoppingCartType = null,
        //    int nodeId = 0, int? productId = null,
        //    DateTime? createdFromUtc = null, DateTime? createdToUtc = null, int? countryId = null,
        //    int pageIndex = 0, int pageSize = int.MaxValue);

        ///// <summary>
        ///// Gets user for shopping cart
        ///// </summary>
        ///// <param name="shoppingCart">Shopping cart</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the result
        ///// </returns>
        //Task<User> GetShoppingCartUserAsync(IList<ShoppingCartItem> shoppingCart);

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteUserAsync(User user);

        /// <summary>
        /// Gets built-in system record used for background tasks
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a user object
        /// </returns>
        Task<User> GetOrCreateBackgroundTaskUserAsync();

        /// <summary>
        /// Gets built-in system guest record used for requests from search engines
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a user object
        /// </returns>
        Task<User> GetOrCreateSearchEngineUserAsync();

        /// <summary>
        /// Gets a user
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a user
        /// </returns>
        Task<User> GetUserByIdAsync(int userId);

        /// <summary>
        /// Get users by identifiers
        /// </summary>
        /// <param name="userIds">User identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the users
        /// </returns>
        Task<IList<User>> GetUsersByIdsAsync(int[] userIds);

        /// <summary>
        /// Get users by guids
        /// </summary>
        /// <param name="userGuids">User guids</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the users
        /// </returns>
        Task<IList<User>> GetUsersByGuidsAsync(Guid[] userGuids);

        /// <summary>
        /// Gets a user by GUID
        /// </summary>
        /// <param name="userGuid">User GUID</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a user
        /// </returns>
        Task<User> GetUserByGuidAsync(Guid userGuid);

        /// <summary>
        /// Get user by email
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the user
        /// </returns>
        Task<User> GetUserByEmailAsync(string email);

        /// <summary>
        /// Get user by system role
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the user
        /// </returns>
        Task<User> GetUserBySystemNameAsync(string systemName);

        /// <summary>
        /// Get user by username
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the user
        /// </returns>
        Task<User> GetUserByUsernameAsync(string username);

        /// <summary>
        /// Get user by public address
        /// </summary>
        /// <param name="publicAddress">Public address</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the user
        /// </returns>
        Task<User> GetUserByPublicAddressAsync(string publicAddress);

        /// <summary>
        /// Insert a guest user
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the user
        /// </returns>
        Task<User> InsertGuestUserAsync();

        /// <summary>
        /// Insert a user
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertUserAsync(User user);

        /// <summary>
        /// Updates the user
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateUserAsync(User user);

        ///// <summary>
        ///// Reset data required for checkout
        ///// </summary>
        ///// <param name="user">User</param>
        ///// <param name="nodeId">Node identifier</param>
        ///// <param name="clearCouponCodes">A value indicating whether to clear coupon code</param>
        ///// <param name="clearCheckoutAttributes">A value indicating whether to clear selected checkout attributes</param>
        ///// <param name="clearRewardPoints">A value indicating whether to clear "Use reward points" flag</param>
        ///// <param name="clearShippingMethod">A value indicating whether to clear selected shipping method</param>
        ///// <param name="clearPaymentMethod">A value indicating whether to clear selected payment method</param>
        ///// <returns>A task that represents the asynchronous operation</returns>
        //Task ResetCheckoutDataAsync(User user, int nodeId,
        //    bool clearCouponCodes = false, bool clearCheckoutAttributes = false,
        //    bool clearRewardPoints = true, bool clearShippingMethod = true,
        //    bool clearPaymentMethod = true);

        /// <summary>
        /// Delete guest user records
        /// </summary>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the number of deleted users
        /// </returns>
        Task<int> DeleteGuestUsersAsync(DateTime? createdFromUtc, DateTime? createdToUtc);

        /// <summary>
        /// Gets a default tax display type (if configured)
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<TaxDisplayType?> GetUserDefaultTaxDisplayTypeAsync(User user);

        /// <summary>
        /// Get full name
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the user full name
        /// </returns>
        Task<string> GetUserFullNameAsync(User user);

        /// <summary>
        /// Formats the user name
        /// </summary>
        /// <param name="user">Source</param>
        /// <param name="stripTooLong">Strip too long user name</param>
        /// <param name="maxLength">Maximum user name length</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the formatted text
        /// </returns>
        Task<string> FormatUsernameAsync(User user, bool stripTooLong = false, int maxLength = 0);

        /// <summary>
        /// Gets coupon codes
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the coupon codes
        /// </returns>
        Task<string[]> ParseAppliedDiscountCouponCodesAsync(User user);

        /// <summary>
        /// Adds a coupon code
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="couponCode">Coupon code</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the new coupon codes document
        /// </returns>
        Task ApplyDiscountCouponCodeAsync(User user, string couponCode);

        /// <summary>
        /// Removes a coupon code
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="couponCode">Coupon code to remove</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the new coupon codes document
        /// </returns>
        Task RemoveDiscountCouponCodeAsync(User user, string couponCode);

        /// <summary>
        /// Gets coupon codes
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the coupon codes
        /// </returns>
        Task<string[]> ParseAppliedGiftCardCouponCodesAsync(User user);

        /// <summary>
        /// Adds a coupon code
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="couponCode">Coupon code</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the new coupon codes document
        /// </returns>
        Task ApplyGiftCardCouponCodeAsync(User user, string couponCode);

        /// <summary>
        /// Removes a coupon code
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="couponCode">Coupon code to remove</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the new coupon codes document
        /// </returns>
        Task RemoveGiftCardCouponCodeAsync(User user, string couponCode);

        /// <summary>
        /// Returns a list of guids of not existing users
        /// </summary>
        /// <param name="guids">The guids of the users to check</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of guids not existing users
        /// </returns>
        Task<Guid[]> GetNotExistingUsersAsync(Guid[] guids);

        #endregion

        #region User roles

        /// <summary>
        /// Add a user-user role mapping
        /// </summary>
        /// <param name="roleMapping">user-user role mapping</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task AddUserRoleMappingAsync(UserUserRoleMapping roleMapping);

        /// <summary>
        /// Remove a user-user role mapping
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="role">User role</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task RemoveUserRoleMappingAsync(User user, UserRole role);

        /// <summary>
        /// Delete a user role
        /// </summary>
        /// <param name="userRole">User role</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteUserRoleAsync(UserRole userRole);

        /// <summary>
        /// Gets a user role
        /// </summary>
        /// <param name="userRoleId">User role identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the user role
        /// </returns>
        Task<UserRole> GetUserRoleByIdAsync(int userRoleId);

        /// <summary>
        /// Gets a user role
        /// </summary>
        /// <param name="systemName">User role system name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the user role
        /// </returns>
        Task<UserRole> GetUserRoleBySystemNameAsync(string systemName);

        /// <summary>
        /// Get user role identifiers
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="showHidden">A value indicating whether to load hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the user role identifiers
        /// </returns>
        Task<int[]> GetUserRoleIdsAsync(User user, bool showHidden = false);

        /// <summary>
        /// Gets list of user roles
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="showHidden">A value indicating whether to load hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<IList<UserRole>> GetUserRolesAsync(User user, bool showHidden = false);

        /// <summary>
        /// Gets all user roles
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the user roles
        /// </returns>
        Task<IList<UserRole>> GetAllUserRolesAsync(bool showHidden = false);

        /// <summary>
        /// Inserts a user role
        /// </summary>
        /// <param name="userRole">User role</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertUserRoleAsync(UserRole userRole);

        /// <summary>
        /// Gets a value indicating whether user is in a certain user role
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="userRoleSystemName">User role system name</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active user roles</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<bool> IsInUserRoleAsync(User user, string userRoleSystemName, bool onlyActiveUserRoles = true);

        /// <summary>
        /// Gets a value indicating whether user is administrator
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active user roles</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<bool> IsAdminAsync(User user, bool onlyActiveUserRoles = true);

        /// <summary>
        /// Gets a value indicating whether user is a moderator
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active user roles</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<bool> IsModeratorAsync(User user, bool onlyActiveUserRoles = true);

        /// <summary>
        /// Gets a value indicating whether user is registered
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active user roles</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<bool> IsRegisteredAsync(User user, bool onlyActiveUserRoles = true);

        /// <summary>
        /// Gets a value indicating whether user is guest
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active user roles</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<bool> IsGuestAsync(User user, bool onlyActiveUserRoles = true);

        /// <summary>
        /// Gets a value indicating whether user is partner
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active user roles</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<bool> IsPartnerAsync(User user, bool onlyActiveUserRoles = true);

        /// <summary>
        /// Gets a value indicating whether user is player
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active user roles</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<bool> IsPlayerAsync(User user, bool onlyActiveUserRoles = true);

        /// <summary>
        /// Updates the user role
        /// </summary>
        /// <param name="userRole">User role</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateUserRoleAsync(UserRole userRole);

        #endregion

        #region User passwords

        /// <summary>
        /// Gets user passwords
        /// </summary>
        /// <param name="userId">User identifier; pass null to load all records</param>
        /// <param name="passwordFormat">Password format; pass null to load all records</param>
        /// <param name="passwordsToReturn">Number of returning passwords; pass null to load all records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of user passwords
        /// </returns>
        Task<IList<UserPassword>> GetUserPasswordsAsync(int? userId = null,
            PasswordFormat? passwordFormat = null, int? passwordsToReturn = null);

        /// <summary>
        /// Get current user password
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the user password
        /// </returns>
        Task<UserPassword> GetCurrentPasswordAsync(int userId);

        /// <summary>
        /// Insert a user password
        /// </summary>
        /// <param name="userPassword">User password</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertUserPasswordAsync(UserPassword userPassword);

        /// <summary>
        /// Update a user password
        /// </summary>
        /// <param name="userPassword">User password</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateUserPasswordAsync(UserPassword userPassword);

        /// <summary>
        /// Check whether password recovery token is valid
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="token">Token to validate</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<bool> IsPasswordRecoveryTokenValidAsync(User user, string token);

        /// <summary>
        /// Check whether password recovery link is expired
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<bool> IsPasswordRecoveryLinkExpiredAsync(User user);

        /// <summary>
        /// Check whether user password is expired 
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue if password is expired; otherwise false
        /// </returns>
        Task<bool> IsPasswordExpiredAsync(User user);

        #endregion

        #region User address mapping

        /// <summary>
        /// Gets a list of addresses mapped to user
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the 
        /// </returns>
        Task<IList<Address>> GetAddressesByUserIdAsync(int userId);

        /// <summary>
        /// Gets a address mapped to user
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <param name="addressId">Address identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<Address> GetUserAddressAsync(int userId, int addressId);

        /// <summary>
        /// Gets a user billing address
        /// </summary>
        /// <param name="user">User identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<Address> GetUserBillingAddressAsync(User user);

        /// <summary>
        /// Gets a user shipping address
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<Address> GetUserShippingAddressAsync(User user);

        /// <summary>
        /// Remove a user-address mapping record
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="address">Address</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task RemoveUserAddressAsync(User user, Address address);

        /// <summary>
        /// Inserts a user-address mapping record
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="address">Address</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertUserAddressAsync(User user, Address address);

        #endregion
    }
}