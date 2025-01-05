using ARWNI2S.Engine.Plugins;

namespace ARWNI2S.Backend.Services.Authentication.External
{
    /// <summary>
    /// Represents method for the external authentication
    /// </summary>
    public partial interface IExternalAuthenticationMethod : IPlugin
    {
        /// <summary>
        /// Gets a type of a view component for displaying plugin in public store
        /// </summary>
        /// <returns>View component type</returns>
        Type GetPublicViewComponent();
    }
}