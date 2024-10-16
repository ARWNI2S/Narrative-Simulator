using ARWNI2S.Node.Services.Plugins;

namespace ARWNI2S.Portal.Services.Authentication.External
{
    /// <summary>
    /// Represents method for the external authentication
    /// </summary>
    public partial interface IExternalAuthenticationMethod : IModule
    {
        /// <summary>
        /// Gets a type of a view component for displaying module in public server
        /// </summary>
        /// <returns>View component type</returns>
        Type GetPublicViewComponent();
    }
}
