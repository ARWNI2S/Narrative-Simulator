﻿using ARWNI2S.Infrastructure;
using ARWNI2S.Node.Core.Caching;
using ARWNI2S.Node.Core.Configuration;
using ARWNI2S.Node.Data;
using ARWNI2S.Node.Data.Configuration;
using ARWNI2S.Node.Services.Common;
using ARWNI2S.Node.Services.Installation;
using ARWNI2S.Node.Services.Plugins;
using ARWNI2S.Node.Services.Security;
using ARWNI2S.Portal.Framework;
using ARWNI2S.Portal.Framework.Security;
using ARWNI2S.Portal.Infrastructure.Installation;
using ARWNI2S.Portal.Models.Install;
using ARWNI2S.Portal.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;

namespace ARWNI2S.Portal.Controllers
{
    [AutoValidateAntiforgeryToken]
    public partial class InstallController : Controller
    {
        #region Fields

        private readonly NI2SSettings _nI2SSettings;
        private readonly Lazy<IInstallationLocalizationService> _locService;
        private readonly Lazy<IInstallationService> _installationService;
        private readonly IEngineFileProvider _fileProvider;
        private readonly Lazy<IPermissionService> _permissionService;
        private readonly Lazy<IModuleService> _moduleService;
        private readonly Lazy<IStaticCacheManager> _staticCacheManager;
        private readonly Lazy<IUploadService> _uploadService;
        private readonly Lazy<IWebHelper> _webHelper;

        #endregion

        #region Ctor

        public InstallController(NI2SSettings nI2SSettings,
            Lazy<IInstallationLocalizationService> locService,
            Lazy<IInstallationService> installationService,
            IEngineFileProvider fileProvider,
            Lazy<IPermissionService> permissionService,
            Lazy<IModuleService> moduleService,
            Lazy<IStaticCacheManager> staticCacheManager,
            Lazy<IUploadService> uploadService,
            Lazy<IWebHelper> webHelper)
        {
            _nI2SSettings = nI2SSettings;
            _locService = locService;
            _installationService = installationService;
            _fileProvider = fileProvider;
            _permissionService = permissionService;
            _moduleService = moduleService;
            _staticCacheManager = staticCacheManager;
            _uploadService = uploadService;
            _webHelper = webHelper;
        }

        #endregion

        #region Utilites

        private InstallModel PrepareCountryList(InstallModel model)
        {
            if (!model.InstallRegionalResources)
                return model;

            var browserCulture = _locService.Value.GetBrowserCulture();
            var countries = new List<SelectListItem>
            {
                //This item was added in case it was not possible to automatically determine the country by culture
                new() { Value = string.Empty, Text = _locService.Value.GetResource("CountrySelect") }
            };
            countries.AddRange(from country in ISO3166.GetCollection()
                               from localization in ISO3166.GetLocalizationInfo(country.Alpha2)
                               let lang = ISO3166.GetLocalizationInfo(country.Alpha2).Count() > 1 ? $" [{localization.Language} language]" : string.Empty
                               let item = new SelectListItem
                               {
                                   Value = $"{country.Alpha2}-{localization.Culture}",
                                   Text = $"{country.Name}{lang}",
                                   Selected = localization.Culture == browserCulture && browserCulture[^2..] == country.Alpha2
                               }
                               select item);
            model.AvailableCountries.AddRange(countries);

            return model;
        }

        private InstallModel PrepareLanguageList(InstallModel model)
        {
            foreach (var lang in _locService.Value.GetAvailableLanguages())
            {
                model.AvailableLanguages.Add(new SelectListItem
                {
                    Value = Url.Action("ChangeLanguage", "Install", new { language = lang.Code }),
                    Text = lang.Name,
                    Selected = _locService.Value.GetCurrentLanguage().Code == lang.Code
                });
            }

            return model;
        }

        private InstallModel PrepareAvailableDataProviders(InstallModel model)
        {
            model.AvailableDataProviders.AddRange(
                _locService.Value.GetAvailableProviderTypes()
                .OrderBy(v => v.Value)
                .Select(pt => new SelectListItem
                {
                    Value = pt.Key.ToString(),
                    Text = pt.Value
                }));

            return model;
        }

        #endregion

        #region Methods

        public virtual IActionResult Index()
        {
            if (DataSettingsManager.IsDatabaseInstalled())
                return RedirectToRoute("Homepage");

            var model = new InstallModel
            {
                AdminEmail = "admin@dragoncorp.org",
                InstallRegionalResources = _nI2SSettings.Get<InstallationConfig>().InstallRegionalResources,
                CreateDatabaseIfNotExists = false,
                ConnectionStringRaw = false,
                DataProvider = DataProviderType.SqlServer
            };

            PrepareAvailableDataProviders(model);
            PrepareLanguageList(model);
            PrepareCountryList(model);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Index(InstallModel model)
        {
            if (DataSettingsManager.IsDatabaseInstalled())
                return RedirectToRoute("Homepage");

            model.InstallRegionalResources = _nI2SSettings.Get<InstallationConfig>().InstallRegionalResources;

            PrepareAvailableDataProviders(model);
            PrepareLanguageList(model);
            PrepareCountryList(model);

            //Consider granting access rights to the resource to the ASP.NET request identity. 
            //ASP.NET has a base process identity 
            //(typically {MACHINE}\ASPNET on IIS 5 or Network Service on IIS 6 and IIS 7, 
            //and the configured application pool identity on IIS 7.5) that is used if the application is not impersonating.
            //If the application is impersonating via <identity impersonate="true"/>, 
            //the identity will be the anonymous user (typically IUSR_MACHINENAME) or the authenticated request user.

            //validate permissions
            var dirsToCheck = _fileProvider.GetDirectoriesWrite();
            foreach (var dir in dirsToCheck)
                if (!_fileProvider.CheckPermissions(dir, false, true, true, false))
                    ModelState.AddModelError(string.Empty, string.Format(_locService.Value.GetResource("ConfigureDirectoryPermissions"), CurrentOSUser.FullName, dir));

            var filesToCheck = _fileProvider.GetFilesWrite();
            foreach (var file in filesToCheck)
            {
                if (!_fileProvider.FileExists(file))
                    continue;

                if (!_fileProvider.CheckPermissions(file, false, true, true, true))
                    ModelState.AddModelError(string.Empty, string.Format(_locService.Value.GetResource("ConfigureFilePermissions"), CurrentOSUser.FullName, file));
            }

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var dataProvider = DataProviderManager.GetDataProvider(model.DataProvider);

                var connectionString = model.ConnectionStringRaw ? model.ConnectionString : dataProvider.BuildConnectionString(model);

                if (string.IsNullOrEmpty(connectionString))
                    throw new PortalException(_locService.Value.GetResource("ConnectionStringWrongFormat"));

                DataSettingsManager.SaveSettings(new DataConfig
                {
                    DataProvider = model.DataProvider,
                    ConnectionString = connectionString
                }, _fileProvider);

                if (model.CreateDatabaseIfNotExists)
                {
                    try
                    {
                        dataProvider.CreateDatabase(model.Collation);
                    }
                    catch (Exception ex)
                    {
                        throw new PortalException(string.Format(_locService.Value.GetResource("DatabaseCreationError"), ex.Message));
                    }
                }
                else
                {
                    //check whether database exists
                    if (!await dataProvider.DatabaseExistsAsync())
                        throw new PortalException(_locService.Value.GetResource("DatabaseNotExists"));
                }

                dataProvider.InitializeDatabase();

                var cultureInfo = new CultureInfo(CommonServicesDefaults.DefaultLanguageCulture);
                var regionInfo = new RegionInfo(CommonServicesDefaults.DefaultLanguageCulture);

                var languagePackInfo = (DownloadUrl: string.Empty, Progress: 0);
                if (model.InstallRegionalResources)
                {
                    //try to get CultureInfo and RegionInfo
                    if (model.Country != null)
                        try
                        {
                            cultureInfo = new CultureInfo(model.Country[3..]);
                            regionInfo = new RegionInfo(model.Country[3..]);
                        }
                        catch
                        {
                            // ignored
                        }

                    //get URL to download language pack
                    //if (cultureInfo.Name != CommonServicesDefaults.DefaultLanguageCulture)
                    //{
                    //    try
                    //    {
                    //        var languageCode = _locService.Value.GetCurrentLanguage().Code[0..2];
                    //        var resultString = await _draCoHttpClient.Value.InstallationCompletedAsync(model.AdminEmail, languageCode, cultureInfo.Name);
                    //        var result = JsonConvert.DeserializeAnonymousType(resultString,
                    //            new { Message = string.Empty, LanguagePack = new { Culture = string.Empty, Progress = 0, DownloadLink = string.Empty } });

                    //        if (result != null && result.LanguagePack.Progress > CommonServicesDefaults.LanguagePackMinTranslationProgressToInstall)
                    //        {
                    //            languagePackInfo.DownloadUrl = result.LanguagePack.DownloadLink;
                    //            languagePackInfo.Progress = result.LanguagePack.Progress;
                    //        }
                    //    }
                    //    catch
                    //    {
                    //        // ignored
                    //    }
                    //}

                    //upload CLDR
                    await _uploadService.Value.UploadLocalePatternAsync(cultureInfo);
                }

                //now resolve installation service
                await _installationService.Value.InstallRequiredDataAsync(model.AdminEmail, model.AdminPassword, languagePackInfo, regionInfo, cultureInfo);

                //prepare modules to install
                _moduleService.Value.ClearInstalledModulesList();

                var modulesIgnoredDuringInstallation = new List<string>();
                if (!string.IsNullOrEmpty(_nI2SSettings.Get<InstallationConfig>().DisabledModules))
                {
                    modulesIgnoredDuringInstallation = _nI2SSettings.Get<InstallationConfig>().DisabledModules
                        .Split(',', StringSplitOptions.RemoveEmptyEntries).Select(moduleName => moduleName.Trim()).ToList();
                }

                var modules = (await _moduleService.Value.GetModuleDescriptorsAsync<IModule>(LoadModulesMode.All))
                    .Where(moduleDescriptor => !modulesIgnoredDuringInstallation.Contains(moduleDescriptor.SystemName))
                    .OrderBy(moduleDescriptor => moduleDescriptor.Group).ThenBy(moduleDescriptor => moduleDescriptor.DisplayOrder)
                    .ToList();

                if (modulesIgnoredDuringInstallation.Count == 1 && modulesIgnoredDuringInstallation[0] == "*")
                    modules.Clear();

                foreach (var module in modules)
                {
                    await _moduleService.Value.PrepareModuleToInstallAsync(module.SystemName, checkDependencies: false);
                }

                //register default permissions
                var permissionProviders = new List<Type> { typeof(StandardPermissionProvider) };
                foreach (var providerType in permissionProviders)
                {
                    var provider = (IPermissionProvider)Activator.CreateInstance(providerType);
                    await _permissionService.Value.InstallPermissionsAsync(provider);
                }

                return View(new InstallModel { RestartUrl = Url.RouteUrl("Homepage") });

            }
            catch (Exception exception)
            {
                await _staticCacheManager.Value.ClearAsync();

                //clear provider settings if something got wrong
                DataSettingsManager.SaveSettings(new DataConfig(), _fileProvider);

                ModelState.AddModelError(string.Empty, string.Format(_locService.Value.GetResource("SetupFailed"), exception.Message));
            }

            return View(model);
        }

        public virtual IActionResult ChangeLanguage(string language)
        {
            if (DataSettingsManager.IsDatabaseInstalled())
                return RedirectToRoute("Homepage");

            _locService.Value.SaveCurrentLanguage(language);

            //Reload the page
            return RedirectToAction("Index", "Install");
        }

        [HttpPost]
        public virtual IActionResult RestartInstall()
        {
            if (DataSettingsManager.IsDatabaseInstalled())
                return RedirectToRoute("Homepage");

            return View("Index", new InstallModel { RestartUrl = Url.Action("Index", "Install") });
        }

        public virtual IActionResult RestartApplication()
        {
            if (DataSettingsManager.IsDatabaseInstalled())
                return RedirectToRoute("Homepage");

            //restart application
            _webHelper.Value.RestartAppDomain();

            return new EmptyResult();
        }

        #endregion
    }
}