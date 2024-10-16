namespace ARWNI2S.Portal.Services.Common
{
    /// <summary>
    /// Represents the HTTP client to request current node
    /// </summary>
    public partial class NodeHttpClient
    {
        #region Fields

        private readonly HttpClient _httpClient;

        #endregion

        #region Ctor

        public NodeHttpClient(HttpClient client,
            IWebHelper webHelper)
        {
            //configure client
            client.BaseAddress = new Uri(webHelper.GetNodeLocation());

            _httpClient = client;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Keep the current node site alive
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the asynchronous task whose result determines that request completed
        /// </returns>
        public virtual async Task KeepAliveAsync()
        {
            await _httpClient.GetStringAsync(CommonServicesDefaults.KeepAlivePath);
        }

        #endregion
    }
}