using ARWNI2S.Engine;
using ARWNI2S.Environment;
using System.Globalization;

namespace ARWNI2S.Framework.Localization
{
    public static class LocalizationExtensions
    {
        private static ILanguageService languageService;

        public static async Task<Language> GetWorkingLanguageAsync(this IWorkingContext workingContext)
        {
            languageService ??= EngineContext.Current.Resolve<ILanguageService>();

            var culture = workingContext.GetWorkingCulture();

            var languages = await languageService.GetAllLanguagesAsync();
            var found = languages.FirstOrDefault(l => AreEqual(l, culture));
            if (found == null)
            {
                //TODO: Implement this method
                //found = languages.FirstOrDefault(l => );
            }
            return found;
        }


        private static bool AreEqual(Language l, CultureInfo culture)
        {
            //TODO: Implement this method
            return l.UniqueSeoCode.Equals(culture.Name, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
