using ARWNI2S.Node.Services.Authentication;

namespace ARWNI2S.Portal.Services.Authentication
{
    /// <summary>
    /// Represents default values related to authentication services
    /// </summary>
    public partial class WebAuthenticationServicesDefaults : AuthenticationServicesDefaults
    { 
        /// <summary>
        /// The default value for the login path
        /// </summary>
        public static PathString LoginPath => new("/login");

        /// <summary>
        /// The default value for the access denied path
        /// </summary>
        public static PathString AccessDeniedPath => new("/page-not-found");
    }
}