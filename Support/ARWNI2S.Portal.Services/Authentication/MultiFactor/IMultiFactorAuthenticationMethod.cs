using ARWNI2S.Node.Core.Services.Plugins;

namespace ARWNI2S.Portal.Services.Authentication.MultiFactor
{
    /// <summary>
    /// Represents method for the multi-factor authentication
    /// </summary>
    public partial interface IMultiFactorAuthenticationMethod : IModule
    {
        #region Methods

        /// <summary>
        ///  Gets a multi-factor authentication type
        /// </summary>
        MultiFactorAuthenticationType Type { get; }

        /// <summary>
        /// Gets a type of a view component for displaying module in public server
        /// </summary>
        /// <returns>View component type</returns>
        Type GetPublicViewComponent();

        /// <summary>
        /// Gets a type of a view component for displaying verification page
        /// </summary>
        /// <returns>View component type</returns>
        Type GetVerificationViewComponent();

        /// <summary>
        /// Gets a multi-factor authentication method description that will be displayed on user info pages in the public server
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task<string> GetDescriptionAsync();

        #endregion
    }
}
