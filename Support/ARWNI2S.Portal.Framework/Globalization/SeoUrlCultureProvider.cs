using ARWNI2S.Node.Core.Entities.Localization;
using ARWNI2S.Node.Core.Infrastructure;
using ARWNI2S.Portal.Services.Localization;
using Microsoft.AspNetCore.Localization;

namespace ARWNI2S.Portal.Framework.Globalization
{
    /// <summary>
    /// Determines the culture information for a request via the URL
    /// </summary>
    public partial class SeoUrlCultureProvider : RequestCultureProvider
    {
        /// <summary>
        /// Implements the provider to determine the culture of the given request
        /// </summary>
        /// <param name="httpContext">HttpContext for the request</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains ProviderCultureResult
        /// </returns>
        public override async Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            var localizationSettings = EngineContext.Current.Resolve<LocalizationSettings>();

            if (!localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                return await NullProviderCultureResult;

            //localized URLs are enabled, so try to get language from the requested page URL
            var (isLocalized, language) = await httpContext.Request.Path.Value.IsLocalizedUrlAsync(httpContext.Request.PathBase, false);
            if (!isLocalized || language is null)
                return await NullProviderCultureResult;

            return new ProviderCultureResult(language.LanguageCulture);
        }
    }
}
