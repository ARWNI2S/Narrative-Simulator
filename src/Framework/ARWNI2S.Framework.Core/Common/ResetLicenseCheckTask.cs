using ARWNI2S.Framework.Configuration;

namespace ARWNI2S.Framework.Common
{
    /// <summary>
    /// Represents a task to reset license check
    /// </summary>
    public partial class ResetLicenseCheckTask : IScheduleTask
    {
        #region Fields

        protected readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public ResetLicenseCheckTask(ISettingService settingService)
        {
            _settingService = settingService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes a task
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task ExecuteAsync()
        {
            await _settingService.SetSettingAsync($"{nameof(AdminAreaSettings)}.{nameof(AdminAreaSettings.CheckLicense)}", true);
        }

        #endregion
    }
}