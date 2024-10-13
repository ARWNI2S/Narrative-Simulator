//using DragonCorp.Metalink.Core.Domain.Catalog;
namespace ARWNI2S.Portal.Services.Authentication.Wallet
{
    public class WalletAuthenticationOptions : AuthenticationSchemeOptions
    {
        private const string CorrelationPrefix = ".Web3.Correlation.";

        private CookieBuilder _correlationCookieBuilder;

        /// <summary>
        /// Initializes a new instance of <see cref="WalletAuthenticationOptions"/>.
        /// </summary>
        public WalletAuthenticationOptions()
        {
            _correlationCookieBuilder = new CorrelationCookieBuilder(this)
            {
                Name = CorrelationPrefix,
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                SecurePolicy = CookieSecurePolicy.SameAsRequest,
                IsEssential = true,
            };
        }

        /// <summary>
        /// Gets or sets the type used to secure data.
        /// </summary>
        public IDataProtectionProvider DataProtectionProvider { get; set; }

        /// <summary>
        /// Gets or sets the optional path the user agent is redirected to if the user
        /// doesn't approve the authorization demand requested by the remote server.
        /// This property is not set by default. In this case, an exception is thrown
        /// if an access_denied response is returned by the remote authorization server.
        /// </summary>
        public PathString AccessDeniedPath { get; set; }

        /// <summary>
        /// Gets or sets the name of the parameter used to convey the original location
        /// of the user before the remote challenge was triggered up to the access denied page.
        /// This property is only used when the <see cref="AccessDeniedPath"/> is explicitly specified.
        /// </summary>
        // Note: this deliberately matches the default parameter name used by the cookie handler.
        public string ReturnUrlParameter { get; set; } = "ReturnUrl";

        /// <summary>
        /// Gets or sets the authentication scheme corresponding to the middleware
        /// responsible of persisting user's identity after a successful authentication.
        /// This value typically corresponds to a cookie middleware registered in the Startup class.
        /// When omitted, <see cref="AuthenticationOptions.DefaultSignInScheme"/> is used as a fallback value.
        /// </summary>
        public string SignInScheme { get; set; }

        /// <summary>
        /// Gets or sets the time limit for completing the authentication flow (15 minutes by default).
        /// </summary>
        public TimeSpan RemoteAuthenticationTimeout { get; set; } = TimeSpan.FromMinutes(15);

        /// <summary>
        /// Gets or sets the URI where the client will be redirected to authenticate.
        /// </summary>
        public string SignatureEndpoint { get; set; } = default!;

        /// <summary>
        /// Defines whether access and refresh tokens should be stored in the
        /// <see cref="AuthenticationProperties"/> after a successful authorization.
        /// This property is set to <c>false</c> by default to reduce
        /// the size of the final authentication cookie.
        /// </summary>
        public bool SaveTokens { get; set; }

        /// <summary>
        /// Gets or sets the type used to secure data handled by the middleware.
        /// </summary>
        public ISecureDataFormat<AuthenticationProperties> StateDataFormat { get; set; } = default!;

        /// <summary>
        /// Determines the settings used to create the correlation cookie before the
        /// cookie gets added to the response.
        /// </summary>
        public CookieBuilder CorrelationCookie
        {
            get => _correlationCookieBuilder;
            set => _correlationCookieBuilder = value ?? throw new ArgumentNullException(nameof(value));
        }

        private sealed class CorrelationCookieBuilder : RequestPathBaseCookieBuilder
        {
            private readonly WalletAuthenticationOptions _options;

            public CorrelationCookieBuilder(WalletAuthenticationOptions remoteAuthenticationOptions)
            {
                _options = remoteAuthenticationOptions;
            }

            protected override string AdditionalPath => _options.SignatureEndpoint;

            public override CookieOptions Build(HttpContext context, DateTimeOffset expiresFrom)
            {
                var cookieOptions = base.Build(context, expiresFrom);

                if (!Expiration.HasValue || !cookieOptions.Expires.HasValue)
                {
                    cookieOptions.Expires = expiresFrom.Add(_options.RemoteAuthenticationTimeout);
                }

                return cookieOptions;
            }
        }
    }
}