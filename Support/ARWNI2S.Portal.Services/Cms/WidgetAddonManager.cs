using ARWNI2S.Node.Data.Entities.Users;
using ARWNI2S.Node.Data.Extensions;
using ARWNI2S.Node.Services.Plugins;
using ARWNI2S.Node.Services.Users;
using ARWNI2S.Portal.Services.Entities.Cms;

namespace ARWNI2S.Portal.Services.Cms
{
    /// <summary>
    /// Represents a widget module manager implementation
    /// </summary>
    public partial class WidgetModuleManager : ModuleManager<IWidgetModule>, IWidgetModuleManager
    {
        #region Fields

        private readonly WidgetSettings _widgetSettings;

        #endregion

        #region Ctor

        public WidgetModuleManager(IUserService userService,
            IModuleService moduleService,
            WidgetSettings widgetSettings) : base(userService, moduleService)
        {
            _widgetSettings = widgetSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load active widgets
        /// </summary>
        /// <param name="user">Filter by user; pass null to load all modules</param>
        /// <param name="storeId">Filter by store; pass 0 to load all modules</param>
        /// <param name="widgetZone">Widget zone; pass null to load all modules</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of active widget
        /// </returns>
        public virtual async Task<IList<IWidgetModule>> LoadActiveModulesAsync(User user = null, int storeId = 0, string widgetZone = null)
        {
            var widgets = await LoadActiveModulesAsync(_widgetSettings.ActiveWidgetSystemNames, user, storeId);

            //filter by widget zone
            if (!string.IsNullOrEmpty(widgetZone))
                widgets = await widgets.WhereAwait(async widget =>
                    (await widget.GetWidgetZonesAsync()).Contains(widgetZone, StringComparer.InvariantCultureIgnoreCase)).ToListAsync();

            return widgets;
        }

        /// <summary>
        /// Check whether the passed widget is active
        /// </summary>
        /// <param name="widget">Widget to check</param>
        /// <returns>Result</returns>
        public virtual bool IsModuleActive(IWidgetModule widget)
        {
            return IsModuleActive(widget, _widgetSettings.ActiveWidgetSystemNames);
        }

        /// <summary>
        /// Check whether the widget with the passed system name is active
        /// </summary>
        /// <param name="systemName">System name of widget to check</param>
        /// <param name="user">Filter by user; pass null to load all modules</param>
        /// <param name="storeId">Filter by store; pass 0 to load all modules</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<bool> IsModuleActiveAsync(string systemName, User user = null, int storeId = 0)
        {
            var widget = await LoadModuleBySystemNameAsync(systemName, user, storeId);

            return IsModuleActive(widget);
        }

        #endregion
    }
}