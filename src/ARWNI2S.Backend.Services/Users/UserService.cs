using ARWNI2S.Backend.Services.Users.Entities;
using ARWNI2S.Caching;
using ARWNI2S.Collections.Generic;
using ARWNI2S.Engine;
using ARWNI2S.Engine.Caching;
using ARWNI2S.Engine.Data;
using ARWNI2S.Engine.Data.Extensions;
using ARWNI2S.Engine.Infrastructure;
using ARWNI2S.Events;
using ARWNI2S.Framework.Common;
using ARWNI2S.Framework.Common.Entities;
using ARWNI2S.Framework.Localization;
using LinqToDB;
using System.Xml;

namespace ARWNI2S.Backend.Services.Users
{
    /// <summary>
    /// User service
    /// </summary>
    public partial class UserService : IUserService
    {
        #region Fields

        protected readonly UserSettings _userSettings;
        protected readonly IEventPublisher _eventPublisher;
        protected readonly IGenericAttributeService _genericAttributeService;
        protected readonly INiisDataProvider _dataProvider;
        protected readonly IRepository<Address> _userAddressRepository;
        //protected readonly IRepository<BlogComment> _blogCommentRepository;
        protected readonly IRepository<User> _userRepository;
        protected readonly IRepository<UserAddressMapping> _userAddressMappingRepository;
        protected readonly IRepository<UserUserRoleMapping> _userUserRoleMappingRepository;
        protected readonly IRepository<UserPassword> _userPasswordRepository;
        protected readonly IRepository<UserRole> _userRoleRepository;
        //protected readonly IRepository<ForumPost> _forumPostRepository;
        //protected readonly IRepository<ForumTopic> _forumTopicRepository;
        protected readonly IRepository<GenericAttribute> _gaRepository;
        //protected readonly IRepository<NewsComment> _newsCommentRepository;
        //protected readonly IRepository<Order> _orderRepository;
        //protected readonly IRepository<ProductReview> _productReviewRepository;
        //protected readonly IRepository<ProductReviewHelpfulness> _productReviewHelpfulnessRepository;
        //protected readonly IRepository<PollVotingRecord> _pollVotingRecordRepository;
        //protected readonly IRepository<ShoppingCartItem> _shoppingCartRepository;
        protected readonly IShortTermCacheManager _shortTermCacheManager;
        protected readonly IStaticCacheManager _staticCacheManager;
        //protected readonly IStoreContext _storeContext;
        //protected readonly ShoppingCartSettings _shoppingCartSettings;
        //protected readonly TaxSettings _taxSettings;

        #endregion

        #region Ctor

        public UserService(UserSettings userSettings,
            IEventPublisher eventPublisher,
            IGenericAttributeService genericAttributeService,
            INiisDataProvider dataProvider,
            IRepository<Address> userAddressRepository,
            //IRepository<BlogComment> blogCommentRepository,
            IRepository<User> userRepository,
            IRepository<UserAddressMapping> userAddressMappingRepository,
            IRepository<UserUserRoleMapping> userUserRoleMappingRepository,
            IRepository<UserPassword> userPasswordRepository,
            IRepository<UserRole> userRoleRepository,
            //IRepository<ForumPost> forumPostRepository,
            //IRepository<ForumTopic> forumTopicRepository,
            IRepository<GenericAttribute> gaRepository,
            //IRepository<NewsComment> newsCommentRepository,
            //IRepository<Order> orderRepository,
            //IRepository<ProductReview> productReviewRepository,
            //IRepository<ProductReviewHelpfulness> productReviewHelpfulnessRepository,
            //IRepository<PollVotingRecord> pollVotingRecordRepository,
            //IRepository<ShoppingCartItem> shoppingCartRepository,
            IShortTermCacheManager shortTermCacheManager,
            IStaticCacheManager staticCacheManager)//,
                                                   //IStoreContext storeContext,
                                                   //ShoppingCartSettings shoppingCartSettings,
                                                   //TaxSettings taxSettings)
        {
            _userSettings = userSettings;
            _eventPublisher = eventPublisher;
            _genericAttributeService = genericAttributeService;
            _dataProvider = dataProvider;
            _userAddressRepository = userAddressRepository;
            //_blogCommentRepository = blogCommentRepository;
            _userRepository = userRepository;
            _userAddressMappingRepository = userAddressMappingRepository;
            _userUserRoleMappingRepository = userUserRoleMappingRepository;
            _userPasswordRepository = userPasswordRepository;
            _userRoleRepository = userRoleRepository;
            //_forumPostRepository = forumPostRepository;
            //_forumTopicRepository = forumTopicRepository;
            _gaRepository = gaRepository;
            //_newsCommentRepository = newsCommentRepository;
            //_orderRepository = orderRepository;
            //_productReviewRepository = productReviewRepository;
            //_productReviewHelpfulnessRepository = productReviewHelpfulnessRepository;
            //_pollVotingRecordRepository = pollVotingRecordRepository;
            //_shoppingCartRepository = shoppingCartRepository;
            _shortTermCacheManager = shortTermCacheManager;
            _staticCacheManager = staticCacheManager;
            //    _storeContext = storeContext;
            //    _shoppingCartSettings = shoppingCartSettings;
            //    _taxSettings = taxSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets a dictionary of all user roles mapped by ID.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation and contains a dictionary of all user roles mapped by ID.
        /// </returns>
        protected virtual async Task<IDictionary<int, UserRole>> GetAllUserRolesDictionaryAsync()
        {
            return await _staticCacheManager.GetAsync(
                _staticCacheManager.PrepareKeyForDefaultCache(EntityCacheDefaults<UserRole>.AllCacheKey),
                async () => await _userRoleRepository.Table.ToDictionaryAsync(cr => cr.Id));
        }

        #endregion

        #region Methods

        #region Users

        /// <summary>
        /// Gets all users
        /// </summary>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="lastActivityFromUtc">Last activity date from (UTC); null to load all records</param>
        /// <param name="lastActivityToUtc">Last activity date to (UTC); null to load all records</param>
        /// <param name="affiliateId">Affiliate identifier</param>
        /// <param name="vendorId">Vendor identifier</param>
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
        /// <param name="isActive">User is active; null to load all users</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="getOnlyTotalCount">A value in indicating whether you want to load only total number of records. Set to "true" if you don't want to load data from database</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the users
        /// </returns>
        public virtual async Task<IPagedList<User>> GetAllUsersAsync(DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            DateTime? lastActivityFromUtc = null, DateTime? lastActivityToUtc = null,
            int affiliateId = 0, int vendorId = 0, int[] userRoleIds = null,
            string email = null, string username = null, string firstName = null, string lastName = null,
            int dayOfBirth = 0, int monthOfBirth = 0,
            string company = null, string phone = null, string zipPostalCode = null, string ipAddress = null,
            bool? isActive = null, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var users = await _userRepository.GetAllPagedAsync(query =>
            {
                if (createdFromUtc.HasValue)
                    query = query.Where(c => createdFromUtc.Value <= c.CreatedOnUtc);
                if (createdToUtc.HasValue)
                    query = query.Where(c => createdToUtc.Value >= c.CreatedOnUtc);
                if (lastActivityFromUtc.HasValue)
                    query = query.Where(c => lastActivityFromUtc.Value <= c.LastActivityDateUtc);
                if (lastActivityToUtc.HasValue)
                    query = query.Where(c => lastActivityToUtc.Value >= c.LastActivityDateUtc);
                if (affiliateId > 0)
                    query = query.Where(c => affiliateId == c.AffiliateId);
                if (vendorId > 0)
                    query = query.Where(c => vendorId == c.VendorId);
                if (isActive.HasValue)
                    query = query.Where(c => c.Active == isActive.Value);

                query = query.Where(c => !c.Deleted);

                if (userRoleIds != null && userRoleIds.Length > 0)
                {
                    query = query.Join(_userUserRoleMappingRepository.Table, x => x.Id, y => y.UserId,
                            (x, y) => new { User = x, Mapping = y })
                        .Where(z => userRoleIds.Contains(z.Mapping.UserRoleId))
                        .Select(z => z.User)
                        .Distinct();
                }

                if (!string.IsNullOrWhiteSpace(email))
                    query = query.Where(c => c.Email.Contains(email));
                if (!string.IsNullOrWhiteSpace(username))
                    query = query.Where(c => c.Username.Contains(username));
                if (!string.IsNullOrWhiteSpace(firstName))
                    query = query.Where(c => c.FirstName.Contains(firstName));
                if (!string.IsNullOrWhiteSpace(lastName))
                    query = query.Where(c => c.LastName.Contains(lastName));
                if (!string.IsNullOrWhiteSpace(company))
                    query = query.Where(c => c.Company.Contains(company));
                if (!string.IsNullOrWhiteSpace(phone))
                    query = query.Where(c => c.Phone.Contains(phone));
                if (!string.IsNullOrWhiteSpace(zipPostalCode))
                    query = query.Where(c => c.ZipPostalCode.Contains(zipPostalCode));

                if (dayOfBirth > 0 && monthOfBirth > 0)
                    query = query.Where(c => c.DateOfBirth.HasValue && c.DateOfBirth.Value.Day == dayOfBirth &&
                                             c.DateOfBirth.Value.Month == monthOfBirth);
                else if (dayOfBirth > 0)
                    query = query.Where(c => c.DateOfBirth.HasValue && c.DateOfBirth.Value.Day == dayOfBirth);
                else if (monthOfBirth > 0)
                    query = query.Where(c => c.DateOfBirth.HasValue && c.DateOfBirth.Value.Month == monthOfBirth);

                //search by IpAddress
                if (!string.IsNullOrWhiteSpace(ipAddress) && CommonHelper.IsValidIpAddress(ipAddress))
                {
                    query = query.Where(w => w.LastIpAddress == ipAddress);
                }

                query = query.OrderByDescending(c => c.CreatedOnUtc);

                return query;
            }, pageIndex, pageSize, getOnlyTotalCount);

            return users;
        }

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
        public virtual async Task<IPagedList<User>> GetOnlineUsersAsync(DateTime lastActivityFromUtc,
            int[] userRoleIds, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _userRepository.Table;
            query = query.Where(c => lastActivityFromUtc <= c.LastActivityDateUtc);
            query = query.Where(c => !c.Deleted);

            if (userRoleIds != null && userRoleIds.Length > 0)
                query = query.Where(c => _userUserRoleMappingRepository.Table.Any(ccrm => ccrm.UserId == c.Id && userRoleIds.Contains(ccrm.UserRoleId)));

            query = query.OrderByDescending(c => c.LastActivityDateUtc);
            var users = await query.ToPagedListAsync(pageIndex, pageSize);

            return users;
        }

        ///// <summary>
        ///// Gets users with shopping carts
        ///// </summary>
        ///// <param name="shoppingCartType">Shopping cart type; pass null to load all records</param>
        ///// <param name="storeId">Store identifier; pass 0 to load all records</param>
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
        //public virtual async Task<IPagedList<User>> GetUsersWithShoppingCartsAsync(ShoppingCartType? shoppingCartType = null,
        //    int storeId = 0, int? productId = null,
        //    DateTime? createdFromUtc = null, DateTime? createdToUtc = null, int? countryId = null,
        //    int pageIndex = 0, int pageSize = int.MaxValue)
        //{
        //    //get all shopping cart items
        //    var items = _shoppingCartRepository.Table;

        //    //filter by type
        //    if (shoppingCartType.HasValue)
        //        items = items.Where(item => item.ShoppingCartTypeId == (int)shoppingCartType.Value);

        //    //filter shopping cart items by store
        //    if (storeId > 0 && !_shoppingCartSettings.CartsSharedBetweenStores)
        //        items = items.Where(item => item.StoreId == storeId);

        //    //filter shopping cart items by product
        //    if (productId > 0)
        //        items = items.Where(item => item.ProductId == productId);

        //    //filter shopping cart items by date
        //    if (createdFromUtc.HasValue)
        //        items = items.Where(item => createdFromUtc.Value <= item.CreatedOnUtc);
        //    if (createdToUtc.HasValue)
        //        items = items.Where(item => createdToUtc.Value >= item.CreatedOnUtc);

        //    //get all active users
        //    var users = _userRepository.Table.Where(user => user.Active && !user.Deleted);

        //    //filter users by billing country
        //    if (countryId > 0)
        //        users = from c in users
        //                    join a in _userAddressRepository.Table on c.BillingAddressId equals a.Id
        //                    where a.CountryId == countryId
        //                    select c;

        //    var usersWithCarts = from c in users
        //                             join item in items on c.Id equals item.UserId
        //                             //we change ordering for the MySQL engine to avoid problems with the ONLY_FULL_GROUP_BY server property that is set by default since the 5.7.5 version
        //                             orderby _dataProvider.ConfigurationName == "MySql" ? c.CreatedOnUtc : item.CreatedOnUtc descending
        //                             select c;

        //    return await usersWithCarts.Distinct().ToPagedListAsync(pageIndex, pageSize);
        //}

        ///// <summary>
        ///// Gets user for shopping cart
        ///// </summary>
        ///// <param name="shoppingCart">Shopping cart</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the result
        ///// </returns>
        //public virtual async Task<User> GetShoppingCartUserAsync(IList<ShoppingCartItem> shoppingCart)
        //{
        //    var userId = shoppingCart.FirstOrDefault()?.UserId;

        //    return userId.HasValue && userId != 0 ? await GetUserByIdAsync(userId.Value) : null;
        //}

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteUserAsync(User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            if (user.IsSystemAccount)
                throw new NI2SException($"System user account ({user.SystemName}) could not be deleted");

            user.Deleted = true;

            if (_userSettings.SuffixDeletedUsers)
            {
                if (!string.IsNullOrEmpty(user.Email))
                    user.Email += UserServicesDefaults.UserDeletedSuffix;
                if (!string.IsNullOrEmpty(user.Username))
                    user.Username += UserServicesDefaults.UserDeletedSuffix;
            }

            await _userRepository.UpdateAsync(user, false);
            await _userRepository.DeleteAsync(user);
        }

        /// <summary>
        /// Gets a user
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a user
        /// </returns>
        public virtual async Task<User> GetUserByIdAsync(int userId)
        {
            return await _userRepository.GetByIdAsync(userId, cache => default, useShortTermCache: true);
        }

        /// <summary>
        /// Get users by identifiers
        /// </summary>
        /// <param name="userIds">User identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the users
        /// </returns>
        public virtual async Task<IList<User>> GetUsersByIdsAsync(int[] userIds)
        {
            return await _userRepository.GetByIdsAsync(userIds, includeDeleted: false);
        }

        /// <summary>
        /// Get users by guids
        /// </summary>
        /// <param name="userGuids">User guids</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the users
        /// </returns>
        public virtual async Task<IList<User>> GetUsersByGuidsAsync(Guid[] userGuids)
        {
            if (userGuids == null)
                return null;

            var query = from c in _userRepository.Table
                        where userGuids.Contains(c.UserGuid)
                        select c;
            var users = await query.ToListAsync();

            return users;
        }

        /// <summary>
        /// Gets a user by GUID
        /// </summary>
        /// <param name="userGuid">User GUID</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a user
        /// </returns>
        public virtual async Task<User> GetUserByGuidAsync(Guid userGuid)
        {
            if (userGuid == Guid.Empty)
                return null;

            var query = from c in _userRepository.Table
                        where c.UserGuid == userGuid
                        orderby c.Id
                        select c;

            return await _shortTermCacheManager.GetAsync(async () => await query.FirstOrDefaultAsync(), UserServicesDefaults.UserByGuidCacheKey, userGuid);
        }

        /// <summary>
        /// Get user by email
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the user
        /// </returns>
        public virtual async Task<User> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            var query = from c in _userRepository.Table
                        orderby c.Id
                        where c.Email == email
                        select c;
            var user = await query.FirstOrDefaultAsync();

            return user;
        }

        /// <summary>
        /// Get user by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the user
        /// </returns>
        public virtual async Task<User> GetUserBySystemNameAsync(string systemName)
        {
            if (string.IsNullOrWhiteSpace(systemName))
                return null;

            var query = from c in _userRepository.Table
                        orderby c.Id
                        where c.SystemName == systemName
                        select c;

            var user = await _shortTermCacheManager.GetAsync(async () => await query.FirstOrDefaultAsync(), UserServicesDefaults.UserBySystemNameCacheKey, systemName);

            return user;
        }

        /// <summary>
        /// Gets built-in system record used for background tasks
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a user object
        /// </returns>
        public virtual async Task<User> GetOrCreateBackgroundTaskUserAsync()
        {
            var backgroundTaskUser = await GetUserBySystemNameAsync(UserDefaults.BackgroundTaskUserName);

            if (backgroundTaskUser is null)
            {
                //var store = await _storeContext.GetCurrentStoreAsync();
                //If for any reason the system user isn't in the database, then we add it
                backgroundTaskUser = new User
                {
                    Email = "builtin@background-task-record.com",
                    UserGuid = Guid.NewGuid(),
                    AdminComment = "Built-in system record used for background tasks.",
                    Active = true,
                    IsSystemAccount = true,
                    SystemName = UserDefaults.BackgroundTaskUserName,
                    CreatedOnUtc = DateTime.UtcNow,
                    LastActivityDateUtc = DateTime.UtcNow,
                    //RegisteredInStoreId = store.Id
                };

                await InsertUserAsync(backgroundTaskUser);

                var guestRole = await GetUserRoleBySystemNameAsync(UserDefaults.GuestsRoleName) ?? throw new NI2SException("'Guests' role could not be loaded");

                await AddUserRoleMappingAsync(new UserUserRoleMapping { UserRoleId = guestRole.Id, UserId = backgroundTaskUser.Id });
            }

            return backgroundTaskUser;
        }

        /// <summary>
        /// Gets built-in system guest record used for requests from search engines
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a user object
        /// </returns>
        public virtual async Task<User> GetOrCreateSearchEngineUserAsync()
        {
            var searchEngineUser = await GetUserBySystemNameAsync(UserDefaults.SearchEngineUserName);

            if (searchEngineUser is null)
            {
                //var store = await _storeContext.GetCurrentStoreAsync();
                //If for any reason the system user isn't in the database, then we add it
                searchEngineUser = new User
                {
                    Email = "builtin@search_engine_record.com",
                    UserGuid = Guid.NewGuid(),
                    AdminComment = "Built-in system guest record used for requests from search engines.",
                    Active = true,
                    IsSystemAccount = true,
                    SystemName = UserDefaults.SearchEngineUserName,
                    CreatedOnUtc = DateTime.UtcNow,
                    LastActivityDateUtc = DateTime.UtcNow,
                    //RegisteredInStoreId = store.Id
                };

                await InsertUserAsync(searchEngineUser);

                var guestRole = await GetUserRoleBySystemNameAsync(UserDefaults.GuestsRoleName) ?? throw new NI2SException("'Guests' role could not be loaded");

                await AddUserRoleMappingAsync(new UserUserRoleMapping { UserRoleId = guestRole.Id, UserId = searchEngineUser.Id });
            }

            return searchEngineUser;
        }

        /// <summary>
        /// Get user by username
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the user
        /// </returns>
        public virtual async Task<User> GetUserByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return null;

            var query = from c in _userRepository.Table
                        orderby c.Id
                        where c.Username == username
                        select c;
            var user = await query.FirstOrDefaultAsync();

            return user;
        }

        /// <summary>
        /// Insert a guest user
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the user
        /// </returns>
        public virtual async Task<User> InsertGuestUserAsync()
        {
            var user = new User
            {
                UserGuid = Guid.NewGuid(),
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow
            };

            //add to 'Guests' role
            var guestRole = await GetUserRoleBySystemNameAsync(UserDefaults.GuestsRoleName) ?? throw new NI2SException("'Guests' role could not be loaded");

            await _userRepository.InsertAsync(user);

            await AddUserRoleMappingAsync(new UserUserRoleMapping { UserId = user.Id, UserRoleId = guestRole.Id });

            return user;
        }

        /// <summary>
        /// Insert a user
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertUserAsync(User user)
        {
            await _userRepository.InsertAsync(user);
        }

        /// <summary>
        /// Updates the user
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateUserAsync(User user)
        {
            await _userRepository.UpdateAsync(user);
        }

        ///// <summary>
        ///// Reset data required for checkout
        ///// </summary>
        ///// <param name="user">User</param>
        ///// <param name="storeId">Store identifier</param>
        ///// <param name="clearCouponCodes">A value indicating whether to clear coupon code</param>
        ///// <param name="clearCheckoutAttributes">A value indicating whether to clear selected checkout attributes</param>
        ///// <param name="clearRewardPoints">A value indicating whether to clear "Use reward points" flag</param>
        ///// <param name="clearShippingMethod">A value indicating whether to clear selected shipping method</param>
        ///// <param name="clearPaymentMethod">A value indicating whether to clear selected payment method</param>
        ///// <returns>A task that represents the asynchronous operation</returns>
        //public virtual async Task ResetCheckoutDataAsync(User user, int storeId,
        //    bool clearCouponCodes = false, bool clearCheckoutAttributes = false,
        //    bool clearRewardPoints = true, bool clearShippingMethod = true,
        //    bool clearPaymentMethod = true)
        //{
        //    ArgumentNullException.ThrowIfNull(user);

        //    //clear entered coupon codes
        //    if (clearCouponCodes)
        //    {
        //        await _genericAttributeService.SaveAttributeAsync<string>(user, UserDefaults.DiscountCouponCodeAttribute, null);
        //        await _genericAttributeService.SaveAttributeAsync<string>(user, UserDefaults.GiftCardCouponCodesAttribute, null);
        //    }

        //    //clear checkout attributes
        //    if (clearCheckoutAttributes)
        //        await _genericAttributeService.SaveAttributeAsync<string>(user, UserDefaults.CheckoutAttributes, null, storeId);

        //    //clear reward points flag
        //    if (clearRewardPoints)
        //        await _genericAttributeService.SaveAttributeAsync(user, UserDefaults.UseRewardPointsDuringCheckoutAttribute, false, storeId);

        //    //clear selected shipping method
        //    if (clearShippingMethod)
        //    {
        //        await _genericAttributeService.SaveAttributeAsync<ShippingOption>(user, UserDefaults.SelectedShippingOptionAttribute, null, storeId);
        //        await _genericAttributeService.SaveAttributeAsync<ShippingOption>(user, UserDefaults.OfferedShippingOptionsAttribute, null, storeId);
        //        await _genericAttributeService.SaveAttributeAsync<PickupPoint>(user, UserDefaults.SelectedPickupPointAttribute, null, storeId);
        //    }

        //    //clear selected payment method
        //    if (clearPaymentMethod)
        //        await _genericAttributeService.SaveAttributeAsync<string>(user, UserDefaults.SelectedPaymentMethodAttribute, null, storeId);

        //    await _eventPublisher.PublishAsync(new ResetCheckoutDataEvent(user, storeId));
        //}

        /// <summary>
        /// Delete guest user records
        /// </summary>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="onlyWithoutShoppingCart">A value indicating whether to delete users only without shopping cart</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the number of deleted users
        /// </returns>
        public virtual async Task<int> DeleteGuestUsersAsync(DateTime? createdFromUtc, DateTime? createdToUtc, bool onlyWithoutShoppingCart)
        {
            var guestRole = await GetUserRoleBySystemNameAsync(UserDefaults.GuestsRoleName);

            var allGuestUsers = from guest in _userRepository.Table
                                join ccm in _userUserRoleMappingRepository.Table on guest.Id equals ccm.UserId
                                where ccm.UserRoleId == guestRole.Id
                                select guest;

            var guestsToDelete = from guest in _userRepository.Table
                                 join g in allGuestUsers on guest.Id equals g.Id
                                 //from sCart in _shoppingCartRepository.Table.Where(sci => sci.UserId == guest.Id).DefaultIfEmpty()
                                 //from order in _orderRepository.Table.Where(o => o.UserId == guest.Id).DefaultIfEmpty()
                                 //from blogComment in _blogCommentRepository.Table.Where(o => o.UserId == guest.Id).DefaultIfEmpty()
                                 //from newsComment in _newsCommentRepository.Table.Where(o => o.UserId == guest.Id).DefaultIfEmpty()
                                 //from productReview in _productReviewRepository.Table.Where(o => o.UserId == guest.Id).DefaultIfEmpty()
                                 //from productReviewHelpfulness in _productReviewHelpfulnessRepository.Table.Where(o => o.UserId == guest.Id).DefaultIfEmpty()
                                 //from pollVotingRecord in _pollVotingRecordRepository.Table.Where(o => o.UserId == guest.Id).DefaultIfEmpty()
                                 //from forumTopic in _forumTopicRepository.Table.Where(o => o.UserId == guest.Id).DefaultIfEmpty()
                                 //from forumPost in _forumPostRepository.Table.Where(o => o.UserId == guest.Id).DefaultIfEmpty()
                                 //where (!onlyWithoutShoppingCart || sCart == null) &&
                                 //      order == null && blogComment == null && newsComment == null && productReview == null && productReviewHelpfulness == null &&
                                 //      pollVotingRecord == null && forumTopic == null && forumPost == null &&
                                 //      !guest.IsSystemAccount &&
                                 //      (createdFromUtc == null || guest.CreatedOnUtc > createdFromUtc) &&
                                 //      (createdToUtc == null || guest.CreatedOnUtc < createdToUtc)
                                 select new { UserId = guest.Id };

            await using var tmpGuests = await _dataProvider.CreateTempDataStorageAsync("tmp_guestsToDelete", guestsToDelete);
            await using var tmpAddresses = await _dataProvider.CreateTempDataStorageAsync("tmp_guestsAddressesToDelete",
                _userAddressMappingRepository.Table
                    .Where(ca => tmpGuests.Any(c => c.UserId == ca.UserId))
                    .Select(ca => new { ca.AddressId }));

            //delete guests
            var totalRecordsDeleted = await _userRepository.DeleteAsync(c => tmpGuests.Any(tmp => tmp.UserId == c.Id));

            //delete attributes
            await _gaRepository.DeleteAsync(ga => tmpGuests.Any(c => c.UserId == ga.EntityId) && ga.KeyGroup == nameof(User));

            //delete m -> m addresses
            await _userAddressRepository.DeleteAsync(a => tmpAddresses.Any(tmp => tmp.AddressId == a.Id));

            return totalRecordsDeleted;
        }

        ///// <summary>
        ///// Gets a tax display type for the user
        ///// </summary>
        ///// <param name="user">User</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the tax display type
        ///// </returns>
        //public virtual async Task<TaxDisplayType> GetUserTaxDisplayTypeAsync(User user)
        //{
        //    ArgumentNullException.ThrowIfNull(user);

        //    //default tax display type
        //    var taxDisplayType = _taxSettings.TaxDisplayType;

        //    //whether users are allowed to select tax display type and the user has previously saved one
        //    if (_taxSettings.AllowUsersToSelectTaxDisplayType && user.TaxDisplayTypeId.HasValue)
        //        taxDisplayType = (TaxDisplayType)user.TaxDisplayTypeId.Value;
        //    else
        //    {
        //        //default tax type by user roles
        //        var defaultRoleTaxDisplayType = await GetUserDefaultTaxDisplayTypeAsync(user);
        //        if (defaultRoleTaxDisplayType.HasValue)
        //            taxDisplayType = defaultRoleTaxDisplayType.Value;
        //    }

        //    return taxDisplayType;
        //}

        ///// <summary>
        ///// Gets a default tax display type (if configured)
        ///// </summary>
        ///// <param name="user">User</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the result
        ///// </returns>
        //public virtual async Task<TaxDisplayType?> GetUserDefaultTaxDisplayTypeAsync(User user)
        //{
        //    ArgumentNullException.ThrowIfNull(user);

        //    var roleWithOverriddenTaxType = (await GetUserRolesAsync(user)).FirstOrDefault(cr => cr.Active && cr.OverrideTaxDisplayType);
        //    if (roleWithOverriddenTaxType == null)
        //        return null;

        //    return (TaxDisplayType)roleWithOverriddenTaxType.DefaultTaxDisplayTypeId;
        //}

        /// <summary>
        /// Get full name
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the user full name
        /// </returns>
        public virtual async Task<string> GetUserFullNameAsync(User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            var firstName = user.FirstName;
            var lastName = user.LastName;

            var fullName = string.Empty;
            if (!string.IsNullOrWhiteSpace(firstName) && !string.IsNullOrWhiteSpace(lastName))
            {
                //do not inject ILocalizationService via constructor because it'll cause circular references
                var format = await EngineContext.Current.Resolve<ILocalizationService>().GetResourceAsync("User.FullNameFormat");

                fullName = string.Format(format, firstName, lastName);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(firstName))
                    fullName = firstName;

                if (!string.IsNullOrWhiteSpace(lastName))
                    fullName = lastName;
            }

            return fullName;
        }

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
        public virtual async Task<string> FormatUsernameAsync(User user, bool stripTooLong = false, int maxLength = 0)
        {
            if (user == null)
                return string.Empty;

            if (await IsGuestAsync(user))
                //do not inject ILocalizationService via constructor because it'll cause circular references
                return await EngineContext.Current.Resolve<ILocalizationService>().GetResourceAsync("User.Guest");

            var result = string.Empty;
            switch (_userSettings.UserNameFormat)
            {
                case UserNameFormat.ShowEmails:
                    result = user.Email;
                    break;
                case UserNameFormat.ShowUsernames:
                    result = user.Username;
                    break;
                case UserNameFormat.ShowFullNames:
                    result = await GetUserFullNameAsync(user);
                    break;
                case UserNameFormat.ShowFirstName:
                    result = user.FirstName;
                    break;
                default:
                    break;
            }

            if (stripTooLong && maxLength > 0)
                result = CommonHelper.EnsureMaximumLength(result, maxLength);

            return result ?? string.Empty;
        }

        /// <summary>
        /// Gets coupon codes
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the coupon codes
        /// </returns>
        public virtual async Task<string[]> ParseAppliedDiscountCouponCodesAsync(User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            var existingCouponCodes = await _genericAttributeService.GetAttributeAsync<string>(user, UserDefaults.DiscountCouponCodeAttribute);

            var couponCodes = new List<string>();
            if (string.IsNullOrEmpty(existingCouponCodes))
                return couponCodes.ToArray();

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(existingCouponCodes);

                var nodeList1 = xmlDoc.SelectNodes(@"//DiscountCouponCodes/CouponCode");
                foreach (XmlNode node1 in nodeList1)
                {
                    if (node1.Attributes?["Code"] == null)
                        continue;
                    var code = node1.Attributes["Code"].InnerText.Trim();
                    couponCodes.Add(code);
                }
            }
            catch
            {
                // ignored
            }

            return couponCodes.ToArray();
        }

        /// <summary>
        /// Adds a coupon code
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="couponCode">Coupon code</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the new coupon codes document
        /// </returns>
        public virtual async Task ApplyDiscountCouponCodeAsync(User user, string couponCode)
        {
            ArgumentNullException.ThrowIfNull(user);

            var result = string.Empty;
            try
            {
                var existingCouponCodes = await _genericAttributeService.GetAttributeAsync<string>(user, UserDefaults.DiscountCouponCodeAttribute);

                couponCode = couponCode.Trim().ToLowerInvariant();

                var xmlDoc = new XmlDocument();
                if (string.IsNullOrEmpty(existingCouponCodes))
                {
                    var element1 = xmlDoc.CreateElement("DiscountCouponCodes");
                    xmlDoc.AppendChild(element1);
                }
                else
                    xmlDoc.LoadXml(existingCouponCodes);

                var rootElement = (XmlElement)xmlDoc.SelectSingleNode(@"//DiscountCouponCodes");

                XmlElement gcElement = null;
                //find existing
                var nodeList1 = xmlDoc.SelectNodes(@"//DiscountCouponCodes/CouponCode");
                foreach (XmlNode node1 in nodeList1)
                {
                    if (node1.Attributes?["Code"] == null)
                        continue;

                    var couponCodeAttribute = node1.Attributes["Code"].InnerText.Trim();

                    if (couponCodeAttribute.ToLowerInvariant() != couponCode.ToLowerInvariant())
                        continue;

                    gcElement = (XmlElement)node1;
                    break;
                }

                //create new one if not found
                if (gcElement == null)
                {
                    gcElement = xmlDoc.CreateElement("CouponCode");
                    gcElement.SetAttribute("Code", couponCode);
                    rootElement.AppendChild(gcElement);
                }

                result = xmlDoc.OuterXml;
            }
            catch
            {
                // ignored
            }

            //apply new value
            await _genericAttributeService.SaveAttributeAsync(user, UserDefaults.DiscountCouponCodeAttribute, result);
        }

        /// <summary>
        /// Removes a coupon code
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="couponCode">Coupon code to remove</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the new coupon codes document
        /// </returns>
        public virtual async Task RemoveDiscountCouponCodeAsync(User user, string couponCode)
        {
            ArgumentNullException.ThrowIfNull(user);

            //get applied coupon codes
            var existingCouponCodes = await ParseAppliedDiscountCouponCodesAsync(user);

            //clear them
            await _genericAttributeService.SaveAttributeAsync<string>(user, UserDefaults.DiscountCouponCodeAttribute, null);

            //save again except removed one
            foreach (var existingCouponCode in existingCouponCodes)
                if (!existingCouponCode.Equals(couponCode, StringComparison.InvariantCultureIgnoreCase))
                    await ApplyDiscountCouponCodeAsync(user, existingCouponCode);
        }

        /// <summary>
        /// Gets coupon codes
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the coupon codes
        /// </returns>
        public virtual async Task<string[]> ParseAppliedGiftCardCouponCodesAsync(User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            var existingCouponCodes = await _genericAttributeService.GetAttributeAsync<string>(user, UserDefaults.GiftCardCouponCodesAttribute);

            var couponCodes = new List<string>();
            if (string.IsNullOrEmpty(existingCouponCodes))
                return couponCodes.ToArray();

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(existingCouponCodes);

                var nodeList1 = xmlDoc.SelectNodes(@"//GiftCardCouponCodes/CouponCode");
                foreach (XmlNode node1 in nodeList1)
                {
                    if (node1.Attributes?["Code"] == null)
                        continue;

                    var code = node1.Attributes["Code"].InnerText.Trim();
                    couponCodes.Add(code);
                }
            }
            catch
            {
                // ignored
            }

            return couponCodes.ToArray();
        }

        /// <summary>
        /// Adds a coupon code
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="couponCode">Coupon code</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the new coupon codes document
        /// </returns>
        public virtual async Task ApplyGiftCardCouponCodeAsync(User user, string couponCode)
        {
            ArgumentNullException.ThrowIfNull(user);

            var result = string.Empty;
            try
            {
                var existingCouponCodes = await _genericAttributeService.GetAttributeAsync<string>(user, UserDefaults.GiftCardCouponCodesAttribute);

                couponCode = couponCode.Trim().ToLowerInvariant();

                var xmlDoc = new XmlDocument();
                if (string.IsNullOrEmpty(existingCouponCodes))
                {
                    var element1 = xmlDoc.CreateElement("GiftCardCouponCodes");
                    xmlDoc.AppendChild(element1);
                }
                else
                    xmlDoc.LoadXml(existingCouponCodes);

                var rootElement = (XmlElement)xmlDoc.SelectSingleNode(@"//GiftCardCouponCodes");

                XmlElement gcElement = null;
                //find existing
                var nodeList1 = xmlDoc.SelectNodes(@"//GiftCardCouponCodes/CouponCode");
                foreach (XmlNode node1 in nodeList1)
                {
                    if (node1.Attributes?["Code"] == null)
                        continue;

                    var couponCodeAttribute = node1.Attributes["Code"].InnerText.Trim();
                    if (couponCodeAttribute.ToLowerInvariant() != couponCode.ToLowerInvariant())
                        continue;

                    gcElement = (XmlElement)node1;
                    break;
                }

                //create new one if not found
                if (gcElement == null)
                {
                    gcElement = xmlDoc.CreateElement("CouponCode");
                    gcElement.SetAttribute("Code", couponCode);
                    rootElement.AppendChild(gcElement);
                }

                result = xmlDoc.OuterXml;
            }
            catch
            {
                // ignored
            }

            //apply new value
            await _genericAttributeService.SaveAttributeAsync(user, UserDefaults.GiftCardCouponCodesAttribute, result);
        }

        /// <summary>
        /// Removes a coupon code
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="couponCode">Coupon code to remove</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the new coupon codes document
        /// </returns>
        public virtual async Task RemoveGiftCardCouponCodeAsync(User user, string couponCode)
        {
            ArgumentNullException.ThrowIfNull(user);

            //get applied coupon codes
            var existingCouponCodes = await ParseAppliedGiftCardCouponCodesAsync(user);

            //clear them
            await _genericAttributeService.SaveAttributeAsync<string>(user, UserDefaults.GiftCardCouponCodesAttribute, null);

            //save again except removed one
            foreach (var existingCouponCode in existingCouponCodes)
                if (!existingCouponCode.Equals(couponCode, StringComparison.InvariantCultureIgnoreCase))
                    await ApplyGiftCardCouponCodeAsync(user, existingCouponCode);
        }

        /// <summary>
        /// Returns a list of guids of not existing users
        /// </summary>
        /// <param name="guids">The guids of the users to check</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of guids not existing users
        /// </returns>
        public virtual async Task<Guid[]> GetNotExistingUsersAsync(Guid[] guids)
        {
            ArgumentNullException.ThrowIfNull(guids);

            var query = _userRepository.Table;
            var queryFilter = guids.Distinct().ToArray();
            //filtering by guid
            var filter = await query.Select(c => c.UserGuid)
                .Where(c => queryFilter.Contains(c))
                .ToListAsync();

            return queryFilter.Except(filter).ToArray();
        }

        #endregion

        #region User roles

        /// <summary>
        /// Add a user-user role mapping
        /// </summary>
        /// <param name="roleMapping">User-user role mapping</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task AddUserRoleMappingAsync(UserUserRoleMapping roleMapping)
        {
            await _userUserRoleMappingRepository.InsertAsync(roleMapping);
        }

        /// <summary>
        /// Remove a user-user role mapping
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="role">User role</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task RemoveUserRoleMappingAsync(User user, UserRole role)
        {
            ArgumentNullException.ThrowIfNull(user);

            ArgumentNullException.ThrowIfNull(role);

            var mapping = await _userUserRoleMappingRepository.Table
                .SingleOrDefaultAsync(ccrm => ccrm.UserId == user.Id && ccrm.UserRoleId == role.Id);

            if (mapping != null)
                await _userUserRoleMappingRepository.DeleteAsync(mapping);
        }

        /// <summary>
        /// Delete a user role
        /// </summary>
        /// <param name="userRole">User role</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteUserRoleAsync(UserRole userRole)
        {
            ArgumentNullException.ThrowIfNull(userRole);

            if (userRole.IsSystemRole)
                throw new NI2SException("System role could not be deleted");

            await _userRoleRepository.DeleteAsync(userRole);
        }

        /// <summary>
        /// Gets a user role
        /// </summary>
        /// <param name="userRoleId">User role identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the user role
        /// </returns>
        public virtual async Task<UserRole> GetUserRoleByIdAsync(int userRoleId)
        {
            var allRolesById = await GetAllUserRolesDictionaryAsync();

            return allRolesById.TryGetValue(userRoleId, out var role) ? role : null;
        }

        /// <summary>
        /// Gets a user role
        /// </summary>
        /// <param name="systemName">User role system name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the user role
        /// </returns>
        public virtual async Task<UserRole> GetUserRoleBySystemNameAsync(string systemName)
        {
            if (string.IsNullOrWhiteSpace(systemName))
                return null;

            var key = _staticCacheManager.PrepareKeyForDefaultCache(UserServicesDefaults.UserRolesBySystemNameCacheKey, systemName);

            var query = from cr in _userRoleRepository.Table
                        orderby cr.Id
                        where cr.SystemName == systemName
                        select cr;

            var userRole = await _staticCacheManager.GetAsync(key, async () => await query.FirstOrDefaultAsync());

            return userRole;
        }

        /// <summary>
        /// Get user role identifiers
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="showHidden">A value indicating whether to load hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the user role identifiers
        /// </returns>
        public virtual async Task<int[]> GetUserRoleIdsAsync(User user, bool showHidden = false)
        {
            ArgumentNullException.ThrowIfNull(user);

            return (await GetUserRolesAsync(user, showHidden: showHidden))
                .Select(cr => cr.Id)
                .ToArray();
        }

        /// <summary>
        /// Gets list of user roles
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="showHidden">A value indicating whether to load hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<IList<UserRole>> GetUserRolesAsync(User user, bool showHidden = false)
        {
            ArgumentNullException.ThrowIfNull(user);

            var allRolesById = await GetAllUserRolesDictionaryAsync();

            var mappings = await _shortTermCacheManager.GetAsync(
                async () => await _userUserRoleMappingRepository.GetAllAsync(query => query.Where(crm => crm.UserId == user.Id)), UserServicesDefaults.UserRolesCacheKey, user);

            return mappings.Select(mapping => allRolesById.TryGetValue(mapping.UserRoleId, out var role) ? role : null)
                .Where(cr => cr != null && (showHidden || cr.Active))
                .ToList();
        }

        /// <summary>
        /// Gets all user roles
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the user roles
        /// </returns>
        public virtual async Task<IList<UserRole>> GetAllUserRolesAsync(bool showHidden = false)
        {
            var allRolesById = await GetAllUserRolesDictionaryAsync();

            return allRolesById.Values
                .Where(cr => showHidden || cr.Active)
                .ToList();
        }

        /// <summary>
        /// Inserts a user role
        /// </summary>
        /// <param name="userRole">User role</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertUserRoleAsync(UserRole userRole)
        {
            await _userRoleRepository.InsertAsync(userRole);
        }

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
        public virtual async Task<bool> IsInUserRoleAsync(User user,
            string userRoleSystemName, bool onlyActiveUserRoles = true)
        {
            ArgumentNullException.ThrowIfNull(user);
            ArgumentException.ThrowIfNullOrEmpty(userRoleSystemName);

            var userRoles = await GetUserRolesAsync(user, !onlyActiveUserRoles);

            return userRoles?.Any(cr => cr.SystemName == userRoleSystemName) ?? false;
        }

        /// <summary>
        /// Gets a value indicating whether user is administrator
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active user roles</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<bool> IsAdminAsync(User user, bool onlyActiveUserRoles = true)
        {
            return await IsInUserRoleAsync(user, UserDefaults.AdministratorsRoleName, onlyActiveUserRoles);
        }

        /// <summary>
        /// Gets a value indicating whether user is a forum moderator
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active user roles</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<bool> IsForumModeratorAsync(User user, bool onlyActiveUserRoles = true)
        {
            return await IsInUserRoleAsync(user, UserDefaults.ForumModeratorsRoleName, onlyActiveUserRoles);
        }

        /// <summary>
        /// Gets a value indicating whether user is registered
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active user roles</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<bool> IsRegisteredAsync(User user, bool onlyActiveUserRoles = true)
        {
            return await IsInUserRoleAsync(user, UserDefaults.RegisteredRoleName, onlyActiveUserRoles);
        }

        /// <summary>
        /// Gets a value indicating whether user is guest
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active user roles</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<bool> IsGuestAsync(User user, bool onlyActiveUserRoles = true)
        {
            return await IsInUserRoleAsync(user, UserDefaults.GuestsRoleName, onlyActiveUserRoles);
        }

        /// <summary>
        /// Gets a value indicating whether user is vendor
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active user roles</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<bool> IsVendorAsync(User user, bool onlyActiveUserRoles = true)
        {
            return await IsInUserRoleAsync(user, UserDefaults.VendorsRoleName, onlyActiveUserRoles);
        }

        /// <summary>
        /// Updates the user role
        /// </summary>
        /// <param name="userRole">User role</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateUserRoleAsync(UserRole userRole)
        {
            await _userRoleRepository.UpdateAsync(userRole);
        }

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
        public virtual async Task<IList<UserPassword>> GetUserPasswordsAsync(int? userId = null,
            PasswordFormat? passwordFormat = null, int? passwordsToReturn = null)
        {
            var query = _userPasswordRepository.Table;

            //filter by user
            if (userId.HasValue)
                query = query.Where(password => password.UserId == userId.Value);

            //filter by password format
            if (passwordFormat.HasValue)
                query = query.Where(password => password.PasswordFormatId == (int)passwordFormat.Value);

            //get the latest passwords
            if (passwordsToReturn.HasValue)
                query = query.OrderByDescending(password => password.CreatedOnUtc).Take(passwordsToReturn.Value);

            return await query.ToListAsync();
        }

        /// <summary>
        /// Get current user password
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the user password
        /// </returns>
        public virtual async Task<UserPassword> GetCurrentPasswordAsync(int userId)
        {
            if (userId == 0)
                return null;

            //return the latest password
            return (await GetUserPasswordsAsync(userId, passwordsToReturn: 1)).FirstOrDefault();
        }

        /// <summary>
        /// Insert a user password
        /// </summary>
        /// <param name="userPassword">User password</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertUserPasswordAsync(UserPassword userPassword)
        {
            await _userPasswordRepository.InsertAsync(userPassword);
        }

        /// <summary>
        /// Update a user password
        /// </summary>
        /// <param name="userPassword">User password</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateUserPasswordAsync(UserPassword userPassword)
        {
            await _userPasswordRepository.UpdateAsync(userPassword);
        }

        /// <summary>
        /// Check whether password recovery token is valid
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="token">Token to validate</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<bool> IsPasswordRecoveryTokenValidAsync(User user, string token)
        {
            ArgumentNullException.ThrowIfNull(user);

            var cPrt = await _genericAttributeService.GetAttributeAsync<string>(user, UserDefaults.PasswordRecoveryTokenAttribute);
            if (string.IsNullOrEmpty(cPrt))
                return false;

            if (!cPrt.Equals(token, StringComparison.InvariantCultureIgnoreCase))
                return false;

            return true;
        }

        /// <summary>
        /// Check whether password recovery link is expired
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<bool> IsPasswordRecoveryLinkExpiredAsync(User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            if (_userSettings.PasswordRecoveryLinkDaysValid == 0)
                return false;

            var generatedDate = await _genericAttributeService.GetAttributeAsync<DateTime?>(user, UserDefaults.PasswordRecoveryTokenDateGeneratedAttribute);
            if (!generatedDate.HasValue)
                return false;

            var daysPassed = (DateTime.UtcNow - generatedDate.Value).TotalDays;
            if (daysPassed > _userSettings.PasswordRecoveryLinkDaysValid)
                return true;

            return false;
        }

        /// <summary>
        /// Check whether user password is expired 
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains true if password is expired; otherwise false
        /// </returns>
        public virtual async Task<bool> IsPasswordExpiredAsync(User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            //the guests don't have a password
            if (await IsGuestAsync(user))
                return false;

            //password lifetime is disabled for user
            if (!(await GetUserRolesAsync(user)).Any(role => role.Active && role.EnablePasswordLifetime))
                return false;

            //setting disabled for all
            if (_userSettings.PasswordLifetime == 0)
                return false;

            //get current password usage time
            var currentLifetime = await _shortTermCacheManager.GetAsync(async () =>
            {
                var userPassword = await GetCurrentPasswordAsync(user.Id);
                //password is not found, so return max value to force user to change password
                if (userPassword == null)
                    return int.MaxValue;

                return (DateTime.UtcNow - userPassword.CreatedOnUtc).Days;
            }, UserServicesDefaults.UserPasswordLifetimeCacheKey, user);

            return currentLifetime >= _userSettings.PasswordLifetime;
        }

        #endregion

        #region User address mapping

        /// <summary>
        /// Remove a user-address mapping record
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="address">Address</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task RemoveUserAddressAsync(User user, Address address)
        {
            ArgumentNullException.ThrowIfNull(user);

            if (await _userAddressMappingRepository.Table
                    .FirstOrDefaultAsync(m => m.AddressId == address.Id && m.UserId == user.Id)
                is UserAddressMapping mapping)
            {
                if (user.BillingAddressId == address.Id)
                    user.BillingAddressId = null;
                if (user.ShippingAddressId == address.Id)
                    user.ShippingAddressId = null;

                await _userAddressMappingRepository.DeleteAsync(mapping);
            }
        }

        /// <summary>
        /// Inserts a user-address mapping record
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="address">Address</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertUserAddressAsync(User user, Address address)
        {
            ArgumentNullException.ThrowIfNull(user);

            ArgumentNullException.ThrowIfNull(address);

            if (await _userAddressMappingRepository.Table
                    .FirstOrDefaultAsync(m => m.AddressId == address.Id && m.UserId == user.Id)
                is null)
            {
                var mapping = new UserAddressMapping
                {
                    AddressId = address.Id,
                    UserId = user.Id
                };

                await _userAddressMappingRepository.InsertAsync(mapping);
            }
        }

        /// <summary>
        /// Gets a list of addresses mapped to user
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<IList<Address>> GetAddressesByUserIdAsync(int userId)
        {
            var query = from address in _userAddressRepository.Table
                        join cam in _userAddressMappingRepository.Table on address.Id equals cam.AddressId
                        where cam.UserId == userId
                        select address;

            return await _shortTermCacheManager.GetAsync(async () => await query.ToListAsync(), UserServicesDefaults.UserAddressesCacheKey, userId);
        }

        /// <summary>
        /// Gets a address mapped to user
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <param name="addressId">Address identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<Address> GetUserAddressAsync(int userId, int addressId)
        {
            if (userId == 0 || addressId == 0)
                return null;

            var query = from address in _userAddressRepository.Table
                        join cam in _userAddressMappingRepository.Table on address.Id equals cam.AddressId
                        where cam.UserId == userId && address.Id == addressId
                        select address;

            return await _shortTermCacheManager.GetAsync(async () => await query.FirstOrDefaultAsync(), UserServicesDefaults.UserAddressCacheKey, userId, addressId);
        }

        /// <summary>
        /// Gets a user billing address
        /// </summary>
        /// <param name="user">User identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<Address> GetUserBillingAddressAsync(User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            return await GetUserAddressAsync(user.Id, user.BillingAddressId ?? 0);
        }

        /// <summary>
        /// Gets a user shipping address
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<Address> GetUserShippingAddressAsync(User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            return await GetUserAddressAsync(user.Id, user.ShippingAddressId ?? 0);
        }

        #endregion

        #endregion
    }
}