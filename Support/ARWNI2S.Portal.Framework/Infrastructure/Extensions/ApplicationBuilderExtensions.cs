using ARWNI2S.Infrastructure;
using ARWNI2S.Node.Core;
using ARWNI2S.Node.Core.Common;
using ARWNI2S.Node.Core.Configuration;
using ARWNI2S.Node.Core.Infrastructure;
using ARWNI2S.Node.Data;
using ARWNI2S.Node.Data.Migrations;
using ARWNI2S.Node.Services.Common;
using ARWNI2S.Node.Services.Localization;
using ARWNI2S.Node.Services.Logging;
using ARWNI2S.Node.Services.Plugins;
using ARWNI2S.Node.Services.ScheduleTasks;
using ARWNI2S.Portal.Framework.Http;
using ARWNI2S.Portal.Services.Configuration;
using ARWNI2S.Portal.Services.Entities.Common;
using Microsoft.Extensions.FileProviders;
using Microsoft.Net.Http.Headers;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace ARWNI2S.Portal.Framework.Infrastructure.Extensions
{
    /// <summary>
    /// Represents extensions of IApplicationBuilder
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Configure the application HTTP request pipeline
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void ConfigureRequestPipeline(this IApplicationBuilder application)
        {
            EngineContext.Current.ConfigureRequestPipeline(application);
        }

        /// <summary>
        /// Starts the engine
        /// </summary>
        /// <param name="_">unused</param>
        /// <returns>async task</returns>
        public static async Task StartEngineAsync(this IApplicationBuilder _)
        {
            var engine = EngineContext.Current;

            //further actions are performed only when the database is installed
            if (DataSettingsManager.IsDatabaseInstalled())
            {
                //log application start
                await engine.Resolve<ILogService>().InformationAsync("Application started");

                //install and update modules
                var moduleService = engine.Resolve<IModuleService>();
                await moduleService.InstallModulesAsync();
                await moduleService.UpdateModulesAsync();

                //update dragonCorp core and db
                var migrationManager = engine.Resolve<IMigrationManager>();
                var assembly = Assembly.GetAssembly(typeof(ApplicationBuilderExtensions));
                migrationManager.ApplyUpMigrations(assembly, MigrationProcessType.Update);
                assembly = Assembly.GetAssembly(typeof(IMigrationManager));
                migrationManager.ApplyUpMigrations(assembly, MigrationProcessType.Update);

                var taskScheduler = engine.Resolve<ITaskScheduler>();
                await taskScheduler.InitializeAsync();
                taskScheduler.StartScheduler();
            }
        }

        /// <summary>
        /// Add exception handling
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UsePortalExceptionHandler(this IApplicationBuilder application)
        {
            var appSettings = EngineContext.Current.Resolve<AppSettings>();
            var webHostEnvironment = EngineContext.Current.Resolve<IWebHostEnvironment>();
            var useDetailedExceptionPage = appSettings.Get<CommonConfig>().DisplayFullErrorStack || webHostEnvironment.IsDevelopment();
            if (useDetailedExceptionPage)
            {
                //get detailed exceptions for developing and testing purposes
                application.UseDeveloperExceptionPage();
            }
            else
            {
                //or use special exception handler
                application.UseExceptionHandler("/Error/Error");
            }

            //log errors
            application.UseExceptionHandler(handler =>
            {
                handler.Run(async context =>
                {
                    var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
                    if (exception == null)
                        return;

                    try
                    {
                        //check whether database is installed
                        if (DataSettingsManager.IsDatabaseInstalled())
                        {
                            //get current user
                            var currentUser = await EngineContext.Current.Resolve<IWorkContext>().GetCurrentUserAsync();

                            //log error
                            await EngineContext.Current.Resolve<ILogService>().ErrorAsync(exception.Message, exception, currentUser);
                        }
                    }
                    finally
                    {
                        //rethrow the exception to show the error page
                        ExceptionDispatchInfo.Throw(exception);
                    }
                });
            });
        }

        /// <summary>
        /// Adds a special handler that checks for responses with the 404 status code that do not have a body
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UsePageNotFound(this IApplicationBuilder application)
        {
            application.UseStatusCodePages(async context =>
            {
                //handle 404 Not Found
                if (context.HttpContext.Response.StatusCode == StatusCodes.Status404NotFound)
                {
                    var webHelper = EngineContext.Current.Resolve<IWebHelper>();
                    if (!webHelper.IsStaticResource())
                    {
                        //get original path and query
                        var originalPath = context.HttpContext.Request.Path;
                        var originalQueryString = context.HttpContext.Request.QueryString;

                        if (DataSettingsManager.IsDatabaseInstalled())
                        {
                            var commonSettings = EngineContext.Current.Resolve<CommonSettings>();

                            if (commonSettings.Log404Errors)
                            {
                                var logger = EngineContext.Current.Resolve<ILogService>();
                                var workContext = EngineContext.Current.Resolve<IWorkContext>();

                                await logger.ErrorAsync($"Error 404. The requested page ({originalPath}) was not found",
                                    user: await workContext.GetCurrentUserAsync());
                            }
                        }

                        try
                        {
                            //get new path
                            var pageNotFoundPath = "/page-not-found";
                            //re-execute request with new path
                            context.HttpContext.Response.Redirect(context.HttpContext.Request.PathBase + pageNotFoundPath);
                        }
                        finally
                        {
                            //return original path to request
                            context.HttpContext.Request.QueryString = originalQueryString;
                            context.HttpContext.Request.Path = originalPath;
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Adds a special handler that checks for responses with the 400 status code (bad request)
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseBadRequestResult(this IApplicationBuilder application)
        {
            application.UseStatusCodePages(async context =>
            {
                //handle 404 (Bad request)
                if (context.HttpContext.Response.StatusCode == StatusCodes.Status400BadRequest)
                {
                    var logger = EngineContext.Current.Resolve<ILogService>();
                    var workContext = EngineContext.Current.Resolve<IWorkContext>();
                    await logger.ErrorAsync("Error 400. Bad request", null, user: await workContext.GetCurrentUserAsync());
                }
            });
        }

        /// <summary>
        /// Configure middleware for dynamically compressing HTTP responses
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseDraCoResponseCompression(this IApplicationBuilder application)
        {
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            //whether to use compression (gzip by default)
            if (EngineContext.Current.Resolve<CommonSettings>().UseResponseCompression)
                application.UseResponseCompression();
        }

        /// <summary>
        /// Adds WebOptimizer to the <see cref="IApplicationBuilder"/> request execution pipeline
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseDraCoWebOptimizer(this IApplicationBuilder application)
        {
            var fileProvider = EngineContext.Current.Resolve<IEngineFileProvider>();
            var webHostEnvironment = EngineContext.Current.Resolve<IWebHostEnvironment>();

            application.UseWebOptimizer(webHostEnvironment, new[]
            {
                new FileProviderOptions
                {
                    RequestPath =  new PathString("/Modules"),
                    FileProvider = new PhysicalFileProvider(fileProvider.MapPath(@"Modules"))
                }
            });
        }

        /// <summary>
        /// Configure static file serving
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseDraCoStaticFiles(this IApplicationBuilder application)
        {
            var fileProvider = EngineContext.Current.Resolve<IEngineFileProvider>();
            var appSettings = EngineContext.Current.Resolve<AppSettings>();

            void staticFileResponse(StaticFileResponseContext context)
            {
                if (!string.IsNullOrEmpty(appSettings.Get<CommonConfig>().StaticFilesCacheControl))
                    context.Context.Response.Headers.Append(HeaderNames.CacheControl, appSettings.Get<CommonConfig>().StaticFilesCacheControl);
            }

            //add handling if sitemaps 
            application.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(fileProvider.GetAbsolutePath(SeoServicesDefaults.SitemapXmlDirectory)),
                RequestPath = new PathString($"/{SeoServicesDefaults.SitemapXmlDirectory}"),
                OnPrepareResponse = context =>
                {
                    if (!DataSettingsManager.IsDatabaseInstalled() ||
                        !EngineContext.Current.Resolve<SitemapXmlSettings>().SitemapXmlEnabled)
                    {
                        context.Context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Context.Response.ContentLength = 0;
                        context.Context.Response.Body = Stream.Null;
                    }
                }
            });

            //common static files
            application.UseStaticFiles(new StaticFileOptions { OnPrepareResponse = staticFileResponse });

            //modules static files
            var staticFileOptions = new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(fileProvider.MapPath(@"Modules")),
                RequestPath = new PathString("/Modules"),
                OnPrepareResponse = staticFileResponse
            };

            //exclude files in blacklist
            if (!string.IsNullOrEmpty(appSettings.Get<CommonConfig>().ModuleStaticFileExtensionsBlacklist))
            {
                var fileExtensionContentTypeProvider = new FileExtensionContentTypeProvider();

                foreach (var ext in appSettings.Get<CommonConfig>().ModuleStaticFileExtensionsBlacklist
                    .Split(';', ',')
                    .Select(e => e.Trim().ToLowerInvariant())
                    .Select(e => $"{(e.StartsWith('.') ? string.Empty : ".")}{e}")
                    .Where(fileExtensionContentTypeProvider.Mappings.ContainsKey))
                {
                    fileExtensionContentTypeProvider.Mappings.Remove(ext);
                }

                staticFileOptions.ContentTypeProvider = fileExtensionContentTypeProvider;
            }

            application.UseStaticFiles(staticFileOptions);

            //add support for backups
            var provider = new FileExtensionContentTypeProvider
            {
                Mappings = { [".bak"] = MimeTypes.ApplicationOctetStream }
            };

            application.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(fileProvider.GetAbsolutePath(CommonServicesDefaults.DbBackupsPath)),
                RequestPath = new PathString("/db_backups"),
                ContentTypeProvider = provider,
                OnPrepareResponse = context =>
                {
                    if (!DataSettingsManager.IsDatabaseInstalled() ||
                        !EngineContext.Current.Resolve<IPermissionService>().AuthorizeAsync(StandardPermissionProvider.ManageMaintenance).Result)
                    {
                        context.Context.Response.StatusCode = StatusCodes.Status404NotFound;
                        context.Context.Response.ContentLength = 0;
                        context.Context.Response.Body = Stream.Null;
                    }
                }
            });

            //add support for webmanifest files
            provider.Mappings[".webmanifest"] = MimeTypes.ApplicationManifestJson;

            application.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(fileProvider.GetAbsolutePath("icons")),
                RequestPath = "/icons",
                ContentTypeProvider = provider
            });

            if (DataSettingsManager.IsDatabaseInstalled())
            {
                application.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = EngineContext.Current.Resolve<IRoxyFilemanFileProvider>(),
                    RequestPath = new PathString(RoxyFilemanServiceDefaults.DefaultRootDirectory),
                    OnPrepareResponse = staticFileResponse
                });
            }

            if (appSettings.Get<CommonConfig>().ServeUnknownFileTypes)
            {
                application.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(fileProvider.GetAbsolutePath(".well-known")),
                    RequestPath = new PathString("/.well-known"),
                    ServeUnknownFileTypes = true,
                });
            }
        }

        /// <summary>
        /// Configure middleware checking whether requested page is keep alive page
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseKeepAlive(this IApplicationBuilder application)
        {
            application.UseMiddleware<KeepAliveMiddleware>();
        }

        /// <summary>
        /// Configure middleware checking whether database is installed
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseInstallUrl(this IApplicationBuilder application)
        {
            application.UseMiddleware<InstallUrlMiddleware>();
        }

        /// <summary>
        /// Adds the authentication middleware, which enables authentication capabilities.
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseDraCoAuthentication(this IApplicationBuilder application)
        {
            //check whether database is installed
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            application.UseMiddleware<AuthenticationMiddleware>();
        }

        /// <summary>
        /// Configure the request localization feature
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseDraCoRequestLocalization(this IApplicationBuilder application)
        {
            application.UseRequestLocalization(options =>
            {
                if (!DataSettingsManager.IsDatabaseInstalled())
                    return;

                //prepare supported cultures
                var cultures = EngineContext.Current.Resolve<ILanguageService>().GetAllLanguages()
#if DEBUG
                    .Where(language => !language.Name.Contains("debug", StringComparison.CurrentCultureIgnoreCase))
#endif
                    .OrderBy(language => language.DisplayOrder)
                    .Select(language => new CultureInfo(language.LanguageCulture)).ToList();
                options.SupportedCultures = cultures;
                options.SupportedUICultures = cultures;
                options.DefaultRequestCulture = new RequestCulture(cultures.FirstOrDefault() ?? new CultureInfo(CommonServicesDefaults.DefaultLanguageCulture));
                options.ApplyCurrentCultureToResponseHeaders = true;

                //configure culture providers
                options.AddInitialRequestCultureProvider(new SeoUrlCultureProvider());
                var cookieRequestCultureProvider = options.RequestCultureProviders.OfType<CookieRequestCultureProvider>().FirstOrDefault();
                if (cookieRequestCultureProvider is not null)
                    cookieRequestCultureProvider.CookieName = $"{CookieDefaults.Prefix}{CookieDefaults.CultureCookie}";
            });
        }

        /// <summary>
        /// Configure Endpoints routing
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseDraCoEndpoints(this IApplicationBuilder application)
        {
            //Execute the endpoint selected by the routing middleware
            application.UseEndpoints(endpoints =>
            {
                //register all routes
                EngineContext.Current.Resolve<IRoutePublisher>().RegisterRoutes(endpoints);
            });
        }

        /// <summary>
        /// Configure applying forwarded headers to their matching fields on the current request.
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseDraCoProxy(this IApplicationBuilder application)
        {
            var appSettings = EngineContext.Current.Resolve<AppSettings>();

            if (appSettings.Get<HostingConfig>().UseProxy)
            {
                var options = new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.All,
                    // IIS already serves as a reverse proxy and will add X-Forwarded headers to all requests,
                    // so we need to increase this limit, otherwise, passed forwarding headers will be ignored.
                    ForwardLimit = 2
                };

                if (!string.IsNullOrEmpty(appSettings.Get<HostingConfig>().ForwardedForHeaderName))
                    options.ForwardedForHeaderName = appSettings.Get<HostingConfig>().ForwardedForHeaderName;

                if (!string.IsNullOrEmpty(appSettings.Get<HostingConfig>().ForwardedProtoHeaderName))
                    options.ForwardedProtoHeaderName = appSettings.Get<HostingConfig>().ForwardedProtoHeaderName;

                if (!string.IsNullOrEmpty(appSettings.Get<HostingConfig>().KnownProxies))
                {
                    foreach (var strIp in appSettings.Get<HostingConfig>().KnownProxies.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
                    {
                        if (IPAddress.TryParse(strIp, out var ip))
                            options.KnownProxies.Add(ip);
                    }

                    if (options.KnownProxies.Count > 1)
                        options.ForwardLimit = null; //disable the limit, because KnownProxies is configured
                }

                //configure forwarding
                application.UseForwardedHeaders(options);
            }
        }

        /// <summary>
        /// Configure WebMarkupMin
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseDraCoWebMarkupMin(this IApplicationBuilder application)
        {
            //check whether database is installed
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            application.UseWebMarkupMin();
        }
    }
}
