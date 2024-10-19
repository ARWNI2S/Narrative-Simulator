using ARWNI2S.Node.Core.Infrastructure;
using ARWNI2S.Node.Data;
using ARWNI2S.Node.Services.Logging;
using ARWNI2S.Portal.Services.Entities.Users;
using Microsoft.AspNetCore.Authentication;

namespace ARWNI2S.Portal.Services.Authentication
{
    /// <summary>
    /// Represents middleware that enables authentication
    /// </summary>
    public class AuthenticationMiddleware
    {
        #region Fields

        private readonly RequestDelegate _next;

        #endregion

        #region Ctor

        public AuthenticationMiddleware(IAuthenticationSchemeProvider schemes, RequestDelegate next)
        {
            Schemes = schemes ?? throw new ArgumentNullException(nameof(schemes));
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public IAuthenticationSchemeProvider Schemes { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke middleware actions
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            context.Features.Set<IAuthenticationFeature>(new AuthenticationFeature
            {
                OriginalPath = context.Request.Path,
                OriginalPathBase = context.Request.PathBase
            });

            // Give any IAuthenticationRequestHandler schemes a chance to handle the request
            var handlers = NodeEngineContext.Current.Resolve<IAuthenticationHandlerProvider>();
            foreach (var scheme in await Schemes.GetRequestHandlerSchemesAsync())
            {
                try
                {
                    if (await handlers.GetHandlerAsync(context, scheme.Name) is IAuthenticationRequestHandler handler && await handler.HandleRequestAsync())
                        return;
                }
                catch (Exception ex)
                {
                    if (!DataSettingsManager.IsDatabaseInstalled())
                        continue;

                    var externalAuthenticationSettings =
                        NodeEngineContext.Current.Resolve<ExternalAuthenticationSettings>();

                    if (!externalAuthenticationSettings.LogErrors)
                        continue;

                    var logger =
                        NodeEngineContext.Current.Resolve<ILogService>();

                    await logger.ErrorAsync(ex.Message, ex);
                }
            }

            var defaultAuthenticate = await Schemes.GetDefaultAuthenticateSchemeAsync();
            if (defaultAuthenticate != null)
            {
                var result = await context.AuthenticateAsync(defaultAuthenticate.Name);
                if (result?.Principal != null)
                {
                    context.User = result.Principal;
                }
            }

            await _next(context);
        }

        #endregion
    }
}