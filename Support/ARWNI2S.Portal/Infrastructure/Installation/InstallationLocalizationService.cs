﻿using ARWNI2S.Infrastructure;
using ARWNI2S.Node.Data;
using ARWNI2S.Node.Services.Common;
using ARWNI2S.Portal.Framework;
using ARWNI2S.Portal.Services;
using ARWNI2S.Portal.Services.Http;
using Microsoft.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Xml;

namespace ARWNI2S.Portal.Infrastructure.Installation
{
    /// <summary>
    /// Localization service for installation process
    /// </summary>
    public partial class InstallationLocalizationService : IInstallationLocalizationService
    {
        #region Fields

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEngineFileProvider _fileProvider;
        private readonly IWebHelper _webHelper;

        private List<InstallationLanguage> _availableLanguages;

        #endregion

        #region Ctor

        public InstallationLocalizationService(IHttpContextAccessor httpContextAccessor,
            IEngineFileProvider fileProvider,
            IWebHelper webHelper)
        {
            _httpContextAccessor = httpContextAccessor;
            _fileProvider = fileProvider;
            _webHelper = webHelper;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get locale resource value
        /// </summary>
        /// <param name="resourceName">Resource name</param>
        /// <returns>Resource value</returns>
        public string GetResource(string resourceName)
        {
            var language = GetCurrentLanguage();
            if (language == null)
                return resourceName;

            var resourceValue = language.Resources
                .Where(r => r.Name.Equals(resourceName, StringComparison.InvariantCultureIgnoreCase))
                .Select(r => r.Value).FirstOrDefault();

            if (string.IsNullOrEmpty(resourceValue))
                return resourceName;

            return resourceValue;
        }

        /// <summary>
        /// Get current browser culture
        /// </summary>
        /// <returns>Current culture</returns>
        public string GetBrowserCulture()
        {
            _httpContextAccessor.HttpContext.Request.Headers.TryGetValue(HeaderNames.AcceptLanguage, out var userLanguages);
            return userLanguages.FirstOrDefault()?.Split(',').FirstOrDefault() ?? CommonServicesDefaults.DefaultLanguageCulture;
        }

        /// <summary>
        /// Get current language for the installation page
        /// </summary>
        /// <returns>Current language</returns>
        public virtual InstallationLanguage GetCurrentLanguage()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            //try to get cookie
            var cookieName = $"{CookieDefaults.Prefix}{CookieDefaults.InstallationLanguageCookie}";
            httpContext.Request.Cookies.TryGetValue(cookieName, out var cookieLanguageCode);

            //ensure it's available (it could be delete since the previous installation)
            var availableLanguages = GetAvailableLanguages();

            var language = availableLanguages
                .FirstOrDefault(l => l.Code.Equals(cookieLanguageCode, StringComparison.InvariantCultureIgnoreCase));
            if (language != null)
                return language;

            //let's find by current browser culture
            if (httpContext.Request.Headers.TryGetValue(HeaderNames.AcceptLanguage, out var userLanguages))
            {
                var userLanguage = userLanguages.FirstOrDefault()?.Split(',').FirstOrDefault() ?? string.Empty;
                if (!string.IsNullOrEmpty(userLanguage))
                {
                    //right. we do "StartsWith" (not "Equals") because we have shorten codes (not full culture names)
                    language = availableLanguages.FirstOrDefault(l => userLanguage.StartsWith(l.Code, StringComparison.InvariantCultureIgnoreCase));
                }
            }

            if (language != null)
                return language;

            //let's return the default one
            language = availableLanguages.FirstOrDefault(l => l.IsDefault);
            if (language != null)
                return language;

            //return any available language
            language = availableLanguages.FirstOrDefault();

            return language;
        }

        /// <summary>
        /// Save a language for the installation page
        /// </summary>
        /// <param name="languageCode">Language code</param>
        public virtual void SaveCurrentLanguage(string languageCode)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddHours(24),
                HttpOnly = true,
                Secure = _webHelper.IsCurrentConnectionSecured()
            };
            var cookieName = $"{CookieDefaults.Prefix}{CookieDefaults.InstallationLanguageCookie}";
            httpContext.Response.Cookies.Delete(cookieName);
            httpContext.Response.Cookies.Append(cookieName, languageCode, cookieOptions);
        }

        /// <summary>
        /// Get a list of available languages
        /// </summary>
        /// <returns>Available installation languages</returns>
        public virtual IList<InstallationLanguage> GetAvailableLanguages()
        {
            if (_availableLanguages != null)
                return _availableLanguages;

            _availableLanguages = [];
            foreach (var filePath in _fileProvider.EnumerateFiles(_fileProvider.MapPath("~/Node_Data/Localization/Installation/"), "*.xml"))
            {
                var xmlDocument = new XmlDocument();
                xmlDocument.Load(filePath);

                //get language code
                var languageCode = "";
                //we file name format: installation.{languagecode}.xml
                var r = new Regex(Regex.Escape("installation.") + "(.*?)" + Regex.Escape(".xml"));
                var matches = r.Matches(_fileProvider.GetFileName(filePath));
                foreach (Match match in matches.Cast<Match>())
                    languageCode = match.Groups[1].Value;

                //at now we use language codes only (not full culture names)
                languageCode = languageCode[..2];

                var languageNode = xmlDocument.SelectSingleNode(@"//Language");

                if (languageNode == null || languageNode.Attributes == null)
                    continue;

                //get language friendly name
                var languageName = languageNode.Attributes["Name"].InnerText.Trim();

                //is default
                var isDefaultAttribute = languageNode.Attributes["IsDefault"];
                var isDefault = isDefaultAttribute != null && Convert.ToBoolean(isDefaultAttribute.InnerText.Trim());

                //is default
                var isRightToLeftAttribute = languageNode.Attributes["IsRightToLeft"];
                var isRightToLeft = isRightToLeftAttribute != null && Convert.ToBoolean(isRightToLeftAttribute.InnerText.Trim());

                //create language
                var language = new InstallationLanguage
                {
                    Code = languageCode,
                    Name = languageName,
                    IsDefault = isDefault,
                    IsRightToLeft = isRightToLeft,
                };

                //load resources
                var resources = xmlDocument.SelectNodes(@"//Language/LocaleResource");
                if (resources == null)
                    continue;
                foreach (XmlNode resNode in resources)
                {
                    if (resNode.Attributes == null)
                        continue;

                    var resNameAttribute = resNode.Attributes["Name"];
                    var resValueNode = resNode.SelectSingleNode("Value");

                    if (resNameAttribute == null)
                        throw new PortalException("All installation resources must have an attribute Name=\"Value\".");
                    var resourceName = resNameAttribute.Value.Trim();
                    if (string.IsNullOrEmpty(resourceName))
                        throw new PortalException("All installation resource attributes 'Name' must have a value.'");

                    if (resValueNode == null)
                        throw new PortalException("All installation resources must have an element \"Value\".");
                    var resourceValue = resValueNode.InnerText.Trim();

                    language.Resources.Add(new InstallationLocaleResource
                    {
                        Name = resourceName,
                        Value = resourceValue
                    });
                }

                _availableLanguages.Add(language);
                _availableLanguages = [.. _availableLanguages.OrderBy(l => l.Name)];

            }
            return _availableLanguages;
        }

        /// <summary>
        /// Get a dictionary of available data provider types
        /// </summary>
        /// <param name="valuesToExclude">Values to exclude</param>
        /// <param name="useLocalization">Localize</param>
        /// <returns>Key-value pairs of available data providers types</returns>
        public Dictionary<int, string> GetAvailableProviderTypes(int[] valuesToExclude = null, bool useLocalization = true)
        {
            return Enum.GetValues(typeof(DataProviderType))
                .Cast<DataProviderType>()
                .Where(enumValue => enumValue != DataProviderType.Unknown && (valuesToExclude == null || !valuesToExclude.Contains(Convert.ToInt32(enumValue))))
                .ToDictionary(
                    enumValue => Convert.ToInt32(enumValue),
                    enumValue => useLocalization ? GetResource(enumValue.ToString()) : CommonHelper.ConvertEnum(enumValue.ToString()));
        }

        #endregion
    }
}