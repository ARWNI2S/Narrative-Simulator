using ARWNI2S.Backend.Services.Users.Entities;
using ARWNI2S.Engine.Data;

namespace ARWNI2S.Backend.Services.Users
{
    /// <summary>
    /// User report service
    /// </summary>
    public partial class UserReportService : IUserReportService
    {
        #region Fields

        protected readonly IUserService _userService;
        protected readonly IDateTimeHelper _dateTimeHelper;
        protected readonly IRepository<User> _userRepository;
        //protected readonly IRepository<Order> _orderRepository;

        #endregion

        #region Ctor

        public UserReportService(IUserService userService,
            IDateTimeHelper dateTimeHelper,
            IRepository<User> userRepository)//,
            //IRepository<Order> orderRepository)
        {
            _userService = userService;
            _dateTimeHelper = dateTimeHelper;
            _userRepository = userRepository;
            //_orderRepository = orderRepository;
        }

        #endregion

        #region Methods

        ///// <summary>
        ///// Get best users
        ///// </summary>
        ///// <param name="createdFromUtc">Order created date from (UTC); null to load all records</param>
        ///// <param name="createdToUtc">Order created date to (UTC); null to load all records</param>
        ///// <param name="os">Order status; null to load all records</param>
        ///// <param name="ps">Order payment status; null to load all records</param>
        ///// <param name="ss">Order shipment status; null to load all records</param>
        ///// <param name="orderBy">1 - order by order total, 2 - order by number of orders</param>
        ///// <param name="pageIndex">Page index</param>
        ///// <param name="pageSize">Page size</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the report
        ///// </returns>
        //public virtual async Task<IPagedList<BestUserReportLine>> GetBestUsersReportAsync(DateTime? createdFromUtc,
        //    DateTime? createdToUtc, OrderStatus? os, PaymentStatus? ps, ShippingStatus? ss, OrderByEnum orderBy,
        //    int pageIndex = 0, int pageSize = 214748364)
        //{
        //    int? orderStatusId = null;
        //    if (os.HasValue)
        //        orderStatusId = (int)os.Value;

        //    int? paymentStatusId = null;
        //    if (ps.HasValue)
        //        paymentStatusId = (int)ps.Value;

        //    int? shippingStatusId = null;
        //    if (ss.HasValue)
        //        shippingStatusId = (int)ss.Value;
        //    var query1 = from c in _userRepository.Table
        //                 join o in _orderRepository.Table on c.Id equals o.UserId
        //                 where (!createdFromUtc.HasValue || createdFromUtc.Value <= o.CreatedOnUtc) &&
        //                       (!createdToUtc.HasValue || createdToUtc.Value >= o.CreatedOnUtc) &&
        //                       (!orderStatusId.HasValue || orderStatusId == o.OrderStatusId) &&
        //                       (!paymentStatusId.HasValue || paymentStatusId == o.PaymentStatusId) &&
        //                       (!shippingStatusId.HasValue || shippingStatusId == o.ShippingStatusId) &&
        //                       !o.Deleted &&
        //                       !c.Deleted
        //                 select new { c, o };

        //    var query2 = from co in query1
        //                 group co by co.c.Id into g
        //                 select new
        //                 {
        //                     UserId = g.Key,
        //                     OrderTotal = g.Sum(x => x.o.OrderTotal),
        //                     OrderCount = g.Count()
        //                 };
        //    query2 = orderBy switch
        //    {
        //        OrderByEnum.OrderByQuantity => query2.OrderByDescending(x => x.OrderCount),
        //        OrderByEnum.OrderByTotalAmount => query2.OrderByDescending(x => x.OrderTotal),
        //        _ => throw new ArgumentException("Wrong orderBy parameter", nameof(orderBy)),
        //    };
        //    var tmp = await query2.ToPagedListAsync(pageIndex, pageSize);
        //    return new PagedList<BestUserReportLine>(tmp.Select(x => new BestUserReportLine
        //    {
        //        UserId = x.UserId,
        //        OrderTotal = x.OrderTotal,
        //        OrderCount = x.OrderCount
        //    }).ToList(),
        //        tmp.PageIndex, tmp.PageSize, tmp.TotalCount);
        //}

        /// <summary>
        /// Gets a report of users registered in the last days
        /// </summary>
        /// <param name="days">Users registered in the last days</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the number of registered users
        /// </returns>
        public virtual async Task<int> GetRegisteredUsersReportAsync(int days)
        {
            var date = (await _dateTimeHelper.ConvertToUserTimeAsync(DateTime.Now)).AddDays(-days);

            var registeredUserRole = await _userService.GetUserRoleBySystemNameAsync(UserDefaults.RegisteredRoleName);
            if (registeredUserRole == null)
                return 0;

            return (await _userService.GetAllUsersAsync(
                date,
                userRoleIds: new[] { registeredUserRole.Id })).Count;
        }

        #endregion
    }
}