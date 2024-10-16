using ARWNI2S.Node.Services.ScheduleTasks;
using ARWNI2S.Node.Services.Users;
using ARWNI2S.Portal.Services.Entities.Gdpr;

namespace ARWNI2S.Portal.Services.Gdpr
{
    /// <summary>
    /// Represents a task for deleting inactive users
    /// </summary>
    public partial class DeleteInactiveUsersTask : IScheduleTask
    {
        #region Fields

        private readonly IUserService _userService;
        private readonly IGdprService _gdprService;
        private readonly GdprSettings _gdprSettings;

        #endregion

        #region Ctor

        public DeleteInactiveUsersTask(IUserService userService,
            IGdprService gdprService,
            GdprSettings gdprSettings)
        {
            _userService = userService;
            _gdprService = gdprService;
            _gdprSettings = gdprSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes a task
        /// </summary>
        public async Task ExecuteAsync()
        {
            if (!_gdprSettings.GdprEnabled)
                return;

            var lastActivityToUtc = DateTime.UtcNow.AddMonths(-_gdprSettings.DeleteInactiveUsersAfterMonths);
            var inactiveUsers = await _userService.GetAllUsersAsync(lastActivityToUtc: lastActivityToUtc);

            foreach (var user in inactiveUsers)
                await _gdprService.PermanentDeleteUserAsync(user);
        }

        #endregion
    }
}
