//using DragonCorp.Metalink.Core.Domain.Catalog;
using System.Security.Claims;

namespace ARWNI2S.Portal.Services.Authentication.Wallet
{
    /// <summary>
    /// Provides failure context information to handler providers.
    /// </summary>
    public class WalletAuthenticationFailureContext : HandleRequestContext<WalletAuthenticationOptions>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="WalletAuthenticationFailureContext"/>.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <param name="scheme">The <see cref="AuthenticationScheme"/>.</param>
        /// <param name="options">The <see cref="WalletAuthenticationOptions"/>.</param>
        /// <param name="failure">User friendly error message for the error.</param>
        public WalletAuthenticationFailureContext(
            HttpContext context,
            AuthenticationScheme scheme,
            WalletAuthenticationOptions options,
            Exception failure)
            : base(context, scheme, options)
        {
            Failure = failure;
        }

        /// <summary>
        /// User friendly error message for the error.
        /// </summary>
        public Exception Failure { get; set; }

        /// <summary>
        /// Additional state values for the authentication session.
        /// </summary>
        public AuthenticationProperties Properties { get; set; }
    }

    /// <summary>
    /// Provides context information to handler providers.
    /// </summary>
    public class WalletAuthenticationSuccededContext : WalletAuthenticationContext<WalletAuthenticationOptions>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="WalletAuthenticationSuccededContext"/>.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <param name="scheme">The <see cref="AuthenticationScheme"/>.</param>
        /// <param name="options">The <see cref="WalletAuthenticationOptions"/>.</param>
        /// <param name="ticket">The received ticket.</param>
        public WalletAuthenticationSuccededContext(
            HttpContext context,
            AuthenticationScheme scheme,
            WalletAuthenticationOptions options,
            AuthenticationTicket ticket)
            : base(context, scheme, options, ticket?.Properties)
            => Principal = ticket?.Principal;

        /// <summary>
        /// Gets or sets the URL to redirect to after signin.
        /// </summary>
        public string ReturnUri { get; set; }
    }

    /// <summary>
    /// Base context for crypto wallet authentication.
    /// </summary>
    public abstract class WalletAuthenticationContext<TOptions> : HandleRequestContext<TOptions> where TOptions : AuthenticationSchemeOptions
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="scheme">The authentication scheme.</param>
        /// <param name="options">The authentication options associated with the scheme.</param>
        /// <param name="properties">The authentication properties.</param>
        protected WalletAuthenticationContext(
            HttpContext context,
            AuthenticationScheme scheme,
            TOptions options,
            AuthenticationProperties properties)
            : base(context, scheme, options)
            => Properties = properties ?? new AuthenticationProperties();

        /// <summary>
        /// Gets the <see cref="ClaimsPrincipal"/> containing the user claims.
        /// </summary>
        public ClaimsPrincipal Principal { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="AuthenticationProperties"/>.
        /// </summary>
        public virtual AuthenticationProperties Properties { get; set; }

        /// <summary>
        /// Calls success creating a ticket with the <see cref="Principal"/> and <see cref="Properties"/>.
        /// </summary>
        public void Success() => Result = HandleRequestResult.Success(new AuthenticationTicket(Principal!, Properties, Scheme.Name));

        /// <summary>
        /// Indicates that authentication failed.
        /// </summary>
        /// <param name="failure">The exception associated with the failure.</param>
        public void Fail(Exception failure) => Result = HandleRequestResult.Fail(failure);

        /// <summary>
        /// Indicates that authentication failed.
        /// </summary>
        /// <param name="failureMessage">The exception associated with the failure.</param>
        public void Fail(string failureMessage) => Result = HandleRequestResult.Fail(failureMessage);
    }
}
