using ARWNI2S.Node.Core.Entities.Users;
using ARWNI2S.Node.Data.Extensions;
using ARWNI2S.Node.Services.Plugins;
using ARWNI2S.Node.Services.Users;
using ARWNI2S.Portal.Services.Entities.Cms;

namespace ARWNI2S.Portal.Services.Cms
{
    /// <summary>
    /// Represents a widget plugin manager implementation
    /// </summary>
    public partial class WidgetPluginManager : PluginManager<IWidgetPlugin>, IWidgetPluginManager
    {
        #region Fields

        private readonly WidgetSettings _widgetSettings;

        #endregion

        #region Ctor

        public WidgetPluginManager(IUserService userService,
            IPluginService pluginService,
            WidgetSettings widgetSettings) : base(userService, pluginService)
        {
            _widgetSettings = widgetSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load active widgets
        /// </summary>
        /// <param name="user">Filter by user; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <param name="widgetZone">Widget zone; pass null to load all plugins</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of active widget
        /// </returns>
        public virtual async Task<IList<IWidgetPlugin>> LoadActivePluginsAsync(User user = null, int storeId = 0, string widgetZone = null)
        {
            var widgets = await LoadActivePluginsAsync(_widgetSettings.ActiveWidgetSystemNames, user, storeId);

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
        public virtual bool IsPluginActive(IWidgetPlugin widget)
        {
            return IsPluginActive(widget, _widgetSettings.ActiveWidgetSystemNames);
        }

        /// <summary>
        /// Check whether the widget with the passed system name is active
        /// </summary>
        /// <param name="systemName">System name of widget to check</param>
        /// <param name="user">Filter by user; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<bool> IsPluginActiveAsync(string systemName, User user = null, int storeId = 0)
        {
            var widget = await LoadPluginBySystemNameAsync(systemName, user, storeId);

            return IsPluginActive(widget);
        }

        #endregion
    }
}