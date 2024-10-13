//using DragonCorp.Metalink.Core.Domain.Catalog;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Globalization;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Base64UrlTextEncoder = Microsoft.AspNetCore.Authentication.Base64UrlTextEncoder;

namespace ARWNI2S.Portal.Services.Authentication.Wallet
{
    /// <summary>
    /// An opinionated abstraction for an <see cref="AuthenticationHandler{TOptions}"/> that performs authentication using a crypto wallet
    /// provider.
    /// </summary>
    /// <typeparam name="TOptions">The type for the options used to configure the authentication handler.</typeparam>
    public abstract class WalletAuthenticationHandler<TOptions> : AuthenticationHandler<TOptions>, IAuthenticationRequestHandler
        where TOptions : WalletAuthenticationOptions, new()
    {
        //private const string CorrelationProperty = ".3bew";
        private const string CorrelationProperty = ".metalink";
        //private const string CorrelationMarker = "E";
        private const string CorrelationMarker = "DC";
        private const string AuthSchemeKey = ".AuthScheme";

        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// The authentication scheme used by default for signin.
        /// </summary>
        protected string SignInScheme => Options.SignInScheme;

        protected WalletAuthenticationHandler(IHttpContextAccessor httpContextAccessor,
            IOptionsMonitor<TOptions> options, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // ##0002
        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            if (string.IsNullOrEmpty(properties.RedirectUri))
            {
                properties.RedirectUri = OriginalPathBase + OriginalPath + Request.QueryString;
            }

            var httpContext = _httpContextAccessor.HttpContext;
            httpContext.Response.ContentType = "application/json";

            if (!properties.Items.TryGetValue(WalletAuthenticationDefaults.PublicAddressKey, out string value))
            {
                // Configura la respuesta HTTP con el contenido JSON
                httpContext.Response.StatusCode = 401; // Código de estado NONONO
                                                       // Convierte el objeto JSON en una cadena
                var jsonError = new
                {
                    Error = "Invalid request."
                };

                // Convierte el objeto JSON en una caden
                await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(jsonError));

                return;

            }

            var publicAddress = value;
            properties.Items.Remove(WalletAuthenticationDefaults.PublicAddressKey);

            /*  **OAuth2 10.12 CSRF infrastructure is useful as it generates a nonce in base class as:
             *  {...
             *  var bytes = new byte[32];
             *  RandomNumberGenerator.Fill(bytes);
             *  var correlationId = Base64UrlTextEncoder.Encode(bytes);
             *  var cookieOptions = Options.CorrelationCookie.Build(Context, Clock.UtcNow);
             *  properties.Items[CorrelationProperty] = correlationId;
             *  var cookieName = Options.CorrelationCookie.Name + correlationId;
             *  Response.Cookies.Append(cookieName, CorrelationMarker, cookieOptions);
             *  }
             */

            // NONCE
            GenerateCorrelationId(properties);

            var jsonData = new
            {
                PublicAddress = publicAddress,
                AuthorizationEndpoint = BuildChallengeUrl(properties, BuildRedirectUri(properties.RedirectUri)),
                Message = await GenerateWalletMessageAsync(publicAddress, properties.Items[CorrelationProperty], properties.Items[WalletAuthenticationDefaults.DateTimeKey])
            };

            // Convierte el objeto JSON en una cadena
            var json = JsonConvert.SerializeObject(jsonData);

            // Configura la respuesta HTTP con el contenido JSON
            httpContext.Response.StatusCode = 200; // Código de estado OK
            await httpContext.Response.WriteAsync(json);

        }

        protected virtual string BuildChallengeUrl(AuthenticationProperties properties, string redirectUri)
        {
            //var scopeParameter = properties.GetParameter<ICollection<string>>(WAuthChallengeProperties.ScopeKey);
            //var scope = scopeParameter != null ? FormatScope(scopeParameter) : FormatScope();

            var state = Options.StateDataFormat.Protect(properties);
            var parameters = new Dictionary<string, string>
            {
                { "response_type", "signature" },
                { "redirect_uri", redirectUri },
                { "state", state },
            };
            return QueryHelpers.AddQueryString(Options.SignatureEndpoint, parameters);
        }

        /// <summary>
        /// Gets a value that determines if the current authentication request should be handled by <see cref="HandleRequestAsync" />.
        /// </summary>
        /// <returns><see langword="true"/> to handle the operation, otherwise <see langword="false"/>.</returns>
        public virtual Task<bool> ShouldHandleRequestAsync()
            => Task.FromResult(Options.SignatureEndpoint == Request.Path /*&&
                Request.Method == HttpMethods.Post &&
                Request.ContentLength > 0*/);

        // ##0001
        /// <summary>
        /// Handles the current authentication request.
        /// </summary>
        /// <returns><see langword="true"/> if authentication was handled, otherwise <see langword="false"/>.</returns>
        public virtual async Task<bool> HandleRequestAsync()
        {
            if (!await ShouldHandleRequestAsync())
            {
                return false;
            }

            AuthenticationTicket ticket = null;
            Exception exception = null;
            AuthenticationProperties properties = null;
            try
            {
                var authResult = await HandleWalletAuthenticateAsync();
                if (authResult == null)
                {
                    exception = new InvalidOperationException("Invalid return state, unable to redirect.");
                }
                else if (authResult.Handled)
                {
                    return true;
                }
                else if (authResult.Skipped || authResult.None)
                {
                    return false;
                }
                else if (!authResult.Succeeded)
                {
                    exception = authResult.Failure ?? new InvalidOperationException("Invalid return state, unable to redirect.");
                    properties = authResult.Properties;
                }

                ticket = authResult?.Ticket;
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            if (exception != null)
            {
                Logger.WalletAuthenticationError(exception.Message);
                var errorContext = new WalletAuthenticationFailureContext(Context, Scheme, Options, exception)
                {
                    Properties = properties
                };
                await OnRemoteFailure(errorContext);

                if (errorContext.Result != null)
                {
                    if (errorContext.Result.Handled)
                    {
                        return true;
                    }
                    else if (errorContext.Result.Skipped)
                    {
                        return false;
                    }
                    else if (errorContext.Result.Failure != null)
                    {
                        throw new MetalinkException("An error was returned from the RemoteFailure event.", errorContext.Result.Failure);
                    }
                }

                if (errorContext.Failure != null)
                {
                    throw new MetalinkException("An error was encountered while handling the remote login.", errorContext.Failure);
                }
            }

            // We have a ticket if we get here
            Debug.Assert(ticket != null);
            var ticketContext = new WalletAuthenticationSuccededContext(Context, Scheme, Options, ticket)
            {
                ReturnUri = ticket.Properties.RedirectUri
            };

            ticket.Properties.RedirectUri = null;

            // Mark which provider produced this identity so we can cross-check later in HandleAuthenticateAsync
            ticketContext.Properties.Items[AuthSchemeKey] = Scheme.Name;

            await OnTicketReceived(ticketContext);

            if (ticketContext.Result != null)
            {
                if (ticketContext.Result.Handled)
                {
                    Logger.SignInHandled();
                    return true;
                }
                else if (ticketContext.Result.Skipped)
                {
                    Logger.SignInSkipped();
                    return false;
                }
            }

            await Context.SignInAsync(SignInScheme, ticketContext.Principal, ticketContext.Properties);

            // Default redirect path is the base path
            if (string.IsNullOrEmpty(ticketContext.ReturnUri))
            {
                ticketContext.ReturnUri = "/";
            }

            Response.Redirect(ticketContext.ReturnUri);
            return true;
        }

        /// <summary>
        /// Authenticate the user identity with the identity provider.
        ///
        /// The method process the request on the endpoint defined by CallbackPath.
        /// </summary>
        protected virtual async Task<HandleRequestResult> HandleWalletAuthenticateAsync()
        {
            var query = Request.Query;

            var state = query["state"];
            var properties = Options.StateDataFormat.Unprotect(state);

            if (properties == null)
            {
                return HandleRequestResult.Fail("The wauth state was missing or invalid.");
            }

            // WAuth2 10.12 CSRF
            if (!properties.Items.TryGetValue(CorrelationProperty, out var nonce) ||
                !properties.Items.TryGetValue(WalletAuthenticationDefaults.DateTimeKey, out var issuedAtUtc) ||
                !ValidateCorrelationId(properties))
            {
                return HandleRequestResult.Fail("Correlation failed.", properties);
            }

            var error = query["error"];
            if (!StringValues.IsNullOrEmpty(error))
            {
                // Note: access_denied errors are special protocol errors indicating the user didn't
                // approve the authorization demand requested by the remote authorization server.
                // Since it's a frequent scenario (that is not caused by incorrect configuration),
                // denied errors are handled differently using HandleAccessDeniedErrorAsync().
                // Visit https://tools.ietf.org/html/rfc6749#section-4.1.2.1 for more information.
                if (StringValues.Equals(error, "access_denied"))
                {
                    return await HandleAccessDeniedErrorAsync(properties);
                }

                var failureMessage = new StringBuilder();
                failureMessage.Append(error);
                var errorDescription = query["error_description"];
                if (!StringValues.IsNullOrEmpty(errorDescription))
                {
                    failureMessage.Append(";Description=").Append(errorDescription);
                }
                var errorUri = query["error_uri"];
                if (!StringValues.IsNullOrEmpty(errorUri))
                {
                    failureMessage.Append(";Uri=").Append(errorUri);
                }

                return HandleRequestResult.Fail(failureMessage.ToString(), properties);
            }

            //Request.EnableBuffering();
            //Stream req = Request.Body;
            //using var reader = new StreamReader(req);
            //string data = await reader.ReadToEndAsync();

            //var signature = JsonConvert.DeserializeObject<SignatureDTO>(data);

            var signature = query["signature"];
            var publicAddress = query["publicAddress"];

            if (string.IsNullOrEmpty(signature) || string.IsNullOrEmpty(publicAddress))
                //if(signature == null || string.IsNullOrEmpty(signature.Signature) || string.IsNullOrEmpty(signature.PublicAddress) || string.IsNullOrEmpty(signature.AntiForgeryToken))
                return HandleRequestResult.Fail("Invalid signature post.", properties);

            //HACK: CHECK anti forgery token ...?

            var tokens = await ValidateSignatureAsync(signature, publicAddress, nonce, issuedAtUtc);

            if (tokens.Error != null)
            {
                return HandleRequestResult.Fail(tokens.Error, properties);
            }
            if (string.IsNullOrEmpty(tokens.AccessToken))
            {
                return HandleRequestResult.Fail("Failed to retrieve access token.", properties);
            }

            try
            {
                //create identity
                var identity = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.NameIdentifier, publicAddress),
                    new Claim(ClaimTypes.Role, WalletAuthenticationDefaults.DisplayName)
                }, AuthenticationServicesDefaults.WalletAuthenticationScheme);

                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, publicAddress, ClaimValueTypes.String));
                identity.AddClaim(new Claim(ClaimTypes.Expiration, tokens.ExpiresIn, ClaimValueTypes.String));
                var principal = new ClaimsPrincipal(identity);

                if (Options.SaveTokens)
                {
                    var authTokens = new List<AuthenticationToken>
                    {
                        new() { Name = "access_token", Value = tokens.AccessToken }
                    };

                    if (!string.IsNullOrEmpty(tokens.ExpiresIn))
                    {
                        if (int.TryParse(tokens.ExpiresIn, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value))
                        {
                            // https://www.w3.org/TR/xmlschema-2/#dateTime
                            // https://msdn.microsoft.com/en-us/library/az4se3k1(v=vs.110).aspx
                            var expiresAt = TimeProvider.GetUtcNow() + TimeSpan.FromSeconds(value);
                            authTokens.Add(new AuthenticationToken
                            {
                                Name = "expires_at",
                                Value = expiresAt.ToString("o", CultureInfo.InvariantCulture)
                            });
                        }
                    }

                    properties.StoreTokens(authTokens);
                }

                var ticket = new AuthenticationTicket(principal, properties, Options.ClaimsIssuer ?? Scheme.Name);

                if (ticket != null)
                    return HandleRequestResult.Success(ticket);

            }
            catch { }

            return HandleRequestResult.Fail("Failed to retrieve user information from blockchain data.", properties);
        }


        /// <inheritdoc />
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var result = await Context.AuthenticateAsync(SignInScheme);
            if (result != null)
            {
                if (result.Failure != null)
                {
                    return result;
                }

                // The SignInScheme may be shared with multiple providers, make sure this provider issued the identity.
                var ticket = result.Ticket;
                if (ticket != null && ticket.Principal != null && ticket.Properties != null
                    && ticket.Properties.Items.TryGetValue(AuthSchemeKey, out var authenticatedScheme)
                    && string.Equals(Scheme.Name, authenticatedScheme, StringComparison.Ordinal))
                {
                    return AuthenticateResult.Success(new AuthenticationTicket(ticket.Principal,
                        ticket.Properties, Scheme.Name));
                }

                return AuthenticateResult.NoResult();
            }

            return AuthenticateResult.Fail("Remote authentication does not directly support AuthenticateAsync");
        }

        /// <inheritdoc />
        protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
            => Context.ForbidAsync(SignInScheme);


        /// <summary>
        /// Produces a cookie containing a nonce used to correlate the current remote authentication request.
        /// </summary>
        /// <param name="properties"></param>
        protected virtual void GenerateCorrelationId(AuthenticationProperties properties)
        {
            ArgumentNullException.ThrowIfNull(properties);

            var bytes = new byte[32];
            RandomNumberGenerator.Fill(bytes);
            var correlationId = Base64UrlTextEncoder.Encode(bytes);

            var cookieOptions = Options.CorrelationCookie.Build(Context, TimeProvider.GetUtcNow());

            properties.Items[CorrelationProperty] = correlationId;
            properties.Items[WalletAuthenticationDefaults.DateTimeKey] = DateTime.UtcNow.ToString();

            var cookieName = Options.CorrelationCookie.Name + correlationId;

            Response.Cookies.Append(cookieName, CorrelationMarker, cookieOptions);
        }

        /// <summary>
        /// Validates that the current request correlates with the current remote authentication request.
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        protected virtual bool ValidateCorrelationId(AuthenticationProperties properties)
        {
            ArgumentNullException.ThrowIfNull(properties);

            if (!properties.Items.TryGetValue(CorrelationProperty, out var correlationId))
            {
                Logger.CorrelationPropertyNotFound(Options.CorrelationCookie.Name!);
                return false;
            }

            properties.Items.Remove(CorrelationProperty);

            var cookieName = Options.CorrelationCookie.Name + correlationId;

            var correlationCookie = Request.Cookies[cookieName];
            if (string.IsNullOrEmpty(correlationCookie))
            {
                Logger.CorrelationCookieNotFound(cookieName);
                return false;
            }

            var cookieOptions = Options.CorrelationCookie.Build(Context, TimeProvider.GetUtcNow());

            Response.Cookies.Delete(cookieName, cookieOptions);

            if (!string.Equals(correlationCookie, CorrelationMarker, StringComparison.Ordinal))
            {
                Logger.UnexpectedCorrelationCookieValue(cookieName, correlationCookie);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Derived types may override this method to handle access denied errors.
        /// </summary>
        /// <param name="properties">The <see cref="AuthenticationProperties"/>.</param>
        /// <returns>The <see cref="HandleRequestResult"/>.</returns>
        protected virtual async Task<HandleRequestResult> HandleAccessDeniedErrorAsync(AuthenticationProperties properties)
        {
            Logger.AccessDeniedError();
            //var context = new Events.AccessDeniedContext(Context, Scheme, Options)
            //{
            //    AccessDeniedPath = Options.AccessDeniedPath,
            //    Properties = properties,
            //    ReturnUrl = properties?.RedirectUri,
            //    ReturnUrlParameter = Options.ReturnUrlParameter
            //};
            //await Events.AccessDenied(context);

            //if (context.Result != null)
            //{
            //    if (context.Result.Handled)
            //    {
            //        Logger.AccessDeniedContextHandled();
            //    }
            //    else if (context.Result.Skipped)
            //    {
            //        Logger.AccessDeniedContextSkipped();
            //    }

            //    return context.Result;
            //}

            // If an access denied endpoint was specified, redirect the user agent.
            // Otherwise, invoke the RemoteFailure event for further processing.
            //if (context.AccessDeniedPath.HasValue)
            //{
            //    string uri = context.AccessDeniedPath;
            //    if (!string.IsNullOrEmpty(context.ReturnUrlParameter) && !string.IsNullOrEmpty(context.ReturnUrl))
            //    {
            //        uri = QueryHelpers.AddQueryString(uri, context.ReturnUrlParameter, context.ReturnUrl);
            //    }
            //    Response.Redirect(BuildRedirectUri(uri));

            //    return HandleRequestResult.Handle();
            //}

            return await Task.FromResult(HandleRequestResult.NoResult());
        }

        protected abstract Task<string> GenerateWalletMessageAsync(string publicAddress, string nonce, string dateTimeUtc);

        protected abstract Task<WalletAuthenticationResult> ValidateSignatureAsync(string signature, string publicAddress, string nonce, string issuedAtUtc);


        /// <summary>
        /// Invoked when there is a remote failure.
        /// </summary>
        private Func<WalletAuthenticationFailureContext, Task> OnRemoteFailure { get; set; } = context => Task.CompletedTask;

        /// <summary>
        /// Invoked after the remote ticket has been received.
        /// </summary>
        private Func<WalletAuthenticationSuccededContext, Task> OnTicketReceived { get; set; } = context => Task.CompletedTask;

    }

}
