using ARWNI2S.Node.Data.Entities.Users;
using ARWNI2S.Node.Services.Plugins;

namespace ARWNI2S.Portal.Services.Cms
{
    /// <summary>
    /// Represents a widget module manager
    /// </summary>
    public partial interface IWidgetModuleManager : IModuleManager<IWidgetModule>
    {
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
        Task<IList<IWidgetModule>> LoadActiveModulesAsync(User user = null, int storeId = 0, string widgetZone = null);

        /// <summary>
        /// Check whether the passed widget is active
        /// </summary>
        /// <param name="widget">Widget to check</param>
        /// <returns>Result</returns>
        bool IsModuleActive(IWidgetModule widget);

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
        Task<bool> IsModuleActiveAsync(string systemName, User user = null, int storeId = 0);
    }
}