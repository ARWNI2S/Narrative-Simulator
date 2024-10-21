using ARWNI2S.Node.Core;
using ARWNI2S.Node.Core.Caching;
using ARWNI2S.Node.Core.Entities.Common;
using ARWNI2S.Node.Core.Entities.Users;
using ARWNI2S.Node.Data;
using ARWNI2S.Node.Data.Extensions;
using ARWNI2S.Node.Services.Common;
using ARWNI2S.Node.Services.Users;
using ARWNI2S.Portal.Services.Entities.Blogs;
using ARWNI2S.Portal.Services.Entities.Common;
using ARWNI2S.Portal.Services.Entities.News;
using ARWNI2S.Portal.Services.Entities.Polls;
using ARWNI2S.Portal.Services.Entities.Tax;
using ARWNI2S.Portal.Services.Entities.Users;
using System.Xml;

namespace ARWNI2S.Portal.Services.Users
{
    /// <summary>
    /// User service
    /// </summary>
    public partial class PortalUserService : UserService
    {
        #region Fields

        private readonly IRepository<Address> _userAddressRepository;
        private readonly IRepository<BlogComment> _blogCommentRepository;
        private readonly IRepository<UserAddressMapping> _userAddressMappingRepository;
        //private readonly IRepository<CryptoAddress> _cryptoAddressRepository;
        private readonly IRepository<NewsComment> _newsCommentRepository;
        private readonly IRepository<PollVotingRecord> _pollVotingRecordRepository;

        #endregion

        #region Ctor

        public PortalUserService(
            WebUserSettings userSettings,
            IGenericAttributeService genericAttributeService,
            INI2SDataProvider dataProvider,
            IRepository<Address> userAddressRepository,
            IRepository<BlogComment> blogCommentRepository,
            IRepository<User> userRepository,
            IRepository<UserAddressMapping> userAddressMappingRepository,
            IRepository<UserUserRoleMapping> userUserRoleMappingRepository,
            IRepository<UserPassword> userPasswordRepository,
            IRepository<UserRole> userRoleRepository,
            IRepository<GenericAttribute> gaRepository,
            IRepository<NewsComment> newsCommentRepository,
            IRepository<PollVotingRecord> pollVotingRecordRepository,
            IShortTermCacheManager shortTermCacheManager,
            IStaticCacheManager staticCacheManager,
            IClusteringContext nodeContext
            ) : base(userSettings, genericAttributeService, dataProvider, userRepository, userUserRoleMappingRepository, userPasswordRepository, userRoleRepository, gaRepository, shortTermCacheManager, staticCacheManager, nodeContext)
        {
            _userAddressRepository = userAddressRepository;
            _blogCommentRepository = blogCommentRepository;
            _userAddressMappingRepository = userAddressMappingRepository;
            _newsCommentRepository = newsCommentRepository;
            _pollVotingRecordRepository = pollVotingRecordRepository;
        }

        #endregion

        #region Methods

        #region Users

        /// <summary>
        /// Gets a default tax display type (if configured)
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<TaxDisplayType?> GetUserDefaultTaxDisplayTypeAsync(User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            var roleWithOverriddenTaxType = (await GetUserRolesAsync(user)).FirstOrDefault(cr => cr.Active && cr.OverrideTaxDisplayType);
            if (roleWithOverriddenTaxType == null)
                return null;

            return (TaxDisplayType)roleWithOverriddenTaxType.DefaultTaxDisplayTypeId;
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
                return [.. couponCodes];

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

            return [.. couponCodes];
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

                    if (!couponCodeAttribute.Equals(couponCode, StringComparison.InvariantCultureIgnoreCase))
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
                return [.. couponCodes];

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

            return [.. couponCodes];
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
                    if (!couponCodeAttribute.Equals(couponCode, StringComparison.InvariantCultureIgnoreCase))
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

            var key = _staticCacheManager.PrepareKeyForShortTermCache(UserServicesDefaults.UserAddressesCacheKey, userId);

            return await _staticCacheManager.GetAsync(key, async () => await query.ToListAsync());
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

            var key = _staticCacheManager.PrepareKeyForShortTermCache(UserServicesDefaults.UserAddressCacheKey, userId, addressId);

            return await _staticCacheManager.GetAsync(key, async () => await query.FirstOrDefaultAsync());
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