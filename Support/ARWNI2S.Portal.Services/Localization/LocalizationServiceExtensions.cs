using ARWNI2S.Node.Core.Infrastructure;
using ARWNI2S.Node.Core.Entities.Localization;
using ARWNI2S.Node.Services.Localization;
using ARWNI2S.Portal.Services.Entities.Security;

namespace ARWNI2S.Portal.Services.Localization
{
    internal static class LocalizationServiceExtensions
    {
        /// <summary>
        /// Get localized value of enum
        /// We don't have UI to manage permission localizable name. That's why we're using this method
        /// </summary>
        /// <param name="permissionRecord">Permission record</param>
        /// <param name="languageId">Language identifier; pass null to use the current working language</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the localized value
        /// </returns>
        public static async Task<string> GetLocalizedPermissionNameAsync(this ILocalizationService localizationService, PermissionRecord permissionRecord, int? languageId = null)
        {
            ArgumentNullException.ThrowIfNull(permissionRecord);

            var _workContext = EngineContext.Current.Resolve<PortalWorkContext>();


            //localized value
            var workingLanguage = await _workContext.GetWorkingLanguageAsync();
            var resourceName = $"{LocalizationServicesDefaults.PermissionLocaleStringResourcesPrefix}{permissionRecord.SystemName}";
            var result = await localizationService.GetResourceAsync(resourceName, languageId ?? workingLanguage.Id, false, string.Empty, true);

            //set default value if required
            if (string.IsNullOrEmpty(result))
                result = permissionRecord.Name;

            return result;

        }

        /// <summary>
        /// Save localized name of a permission
        /// </summary>
        /// <param name="permissionRecord">Permission record</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public static async Task SaveLocalizedPermissionNameAsync(this ILocalizationService localizationService, PermissionRecord permissionRecord)
        {
            ArgumentNullException.ThrowIfNull(permissionRecord);

            var _languageService = EngineContext.Current.Resolve<ILanguageService>();

            var resourceName = $"{LocalizationServicesDefaults.PermissionLocaleStringResourcesPrefix}{permissionRecord.SystemName}";
            var resourceValue = permissionRecord.Name;

            foreach (var lang in await _languageService.GetAllLanguagesAsync(true))
            {
                var lsr = await localizationService.GetLocaleStringResourceByNameAsync(resourceName, lang.Id, false);
                if (lsr == null)
                {
                    lsr = new LocaleStringResource
                    {
                        LanguageId = lang.Id,
                        ResourceName = resourceName,
                        ResourceValue = resourceValue
                    };
                    await localizationService.InsertLocaleStringResourceAsync(lsr);
                }
                else
                {
                    lsr.ResourceValue = resourceValue;
                    await localizationService.UpdateLocaleStringResourceAsync(lsr);
                }
            }

        }

        /// <summary>
        /// Delete a localized name of a permission
        /// </summary>
        /// <param name="permissionRecord">Permission record</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public static async Task DeleteLocalizedPermissionNameAsync(this ILocalizationService localizationService, PermissionRecord permissionRecord)
        {
            ArgumentNullException.ThrowIfNull(permissionRecord);

            var _languageService = EngineContext.Current.Resolve<ILanguageService>();

            var resourceName = $"{LocalizationServicesDefaults.PermissionLocaleStringResourcesPrefix}{permissionRecord.SystemName}";
            foreach (var lang in await _languageService.GetAllLanguagesAsync(true))
            {
                var lsr = await localizationService.GetLocaleStringResourceByNameAsync(resourceName, lang.Id, false);
                if (lsr != null)
                    await localizationService.DeleteLocaleStringResourceAsync(lsr);
            }
        }
    }
}
