using ARWNI2S.Node.Services.Network;
using ARWNI2S.Node.Services.ScheduleTasks;

namespace ARWNI2S.Portal.Services.Common
{
    /// <summary>
    /// Represents a task for keeping the site alive
    /// </summary>
    public partial class KeepAliveTask : IScheduleTask
    {
        #region Fields

        private readonly NodeHttpClient _nodeHttpClient;

        #endregion

        #region Ctor

        public KeepAliveTask(NodeHttpClient serverHttpClient)
        {
            _nodeHttpClient = serverHttpClient;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes a task
        /// </summary>
        public async Task ExecuteAsync()
        {
            await _nodeHttpClient.KeepAliveAsync();
        }

        #endregion
    }
}