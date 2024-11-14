using ARWNI2S.Infrastructure.Engine.Builder;
using ARWNI2S.Node.Core.Configuration;
using ARWNI2S.Node.Core.Infrastructure;
using ARWNI2S.Node.Data;
using ARWNI2S.Node.Data.Migrations;
using ARWNI2S.Node.Services.Logging;
using ARWNI2S.Node.Services.Plugins;
using ARWNI2S.Node.Services.ScheduleTasks;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace ARWNI2S.Engine.Hosting.Extensions
{
    /// <summary>
    /// Represents extensions of IEngineBuilder
    /// </summary>
    public static class EngineBuilderExtensions
    {
        /// <summary>
        /// Configure the node NI2S engine
        /// </summary>
        /// <param name="engine">Builder for configuring a node's NI2S engine</param>
        public static void ConfigureEngine(this IEngineBuilder engine)
        {
            //engine.ConfigureSimulation();
            NodeEngineContext.Current.ConfigureEngine(engine);
        }

        /// <summary>
        /// Starts the engine
        /// </summary>
        /// <param name="_">unused</param>
        /// <returns>async task</returns>
        public static async Task StartEngineAsync(this IEngineBuilder _)
        {
            var engine = NodeEngineContext.Current;

            //further actions are performed only when the database is installed
            if (DataSettingsManager.IsDatabaseInstalled())
            {
                //install and update modules
                var moduleService = engine.Resolve<IModuleService>();
                await moduleService.InstallModulesAsync();
                await moduleService.UpdateModulesAsync();

                //update dragonCorp core and db
                var migrationManager = engine.Resolve<IMigrationManager>();
                var assembly = Assembly.GetAssembly(typeof(EngineBuilderExtensions));
                migrationManager.ApplyUpMigrations(assembly, MigrationProcessType.Update);
                assembly = Assembly.GetAssembly(typeof(IMigrationManager));
                migrationManager.ApplyUpMigrations(assembly, MigrationProcessType.Update);

                ////start engine lifecycle
                //var engineLifecycle = engine.Resolve<EngineLifecycle>();

                //log node start
                await engine.Resolve<ILogService>().InformationAsync("Node started");

                var taskScheduler = engine.Resolve<IClusterTaskScheduler>();
                await taskScheduler.InitializeAsync();
                taskScheduler.StartScheduler();
            }
        }

        /// <summary>
        /// Add exception handling
        /// </summary>
        /// <param name="engine">Builder for configuring an engine's request pipeline</param>
        public static void UseNI2SExceptionHandler(this IEngineBuilder engine)
        {
            //TODO: EXCEPTION HANDLER!!!

            var ni2sSettings = NodeEngineContext.Current.Resolve<NI2SSettings>();
            var hostEnvironment = NodeEngineContext.Current.Resolve<IHostEnvironment>();
            var useDetailedExceptions = ni2sSettings.Get<CommonConfig>().DisplayFullErrorStack || hostEnvironment.IsDevelopment();
            if (useDetailedExceptions)
            {
                //get detailed exceptions for developing and testing purposes
                //engine.UseDeveloperExceptionPage();
            }
            else
            {
                //or use special exception handler
                //engine.UseExceptionHandler("/Error/Error");
            }

            //log errors
            //engine.UseExceptionHandler(handler =>
            //{
            //    handler.Run(async context =>
            //    {
            //        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
            //        if (exception == null)
            //            return;

            //        try
            //        {
            //            //check whether database is installed
            //            if (DataSettingsManager.IsDatabaseInstalled())
            //            {
            //                //get current user
            //                var currentUser = await EngineContext.Current.Resolve<IWorkContext>().GetCurrentUserAsync();

            //                //log error
            //                await EngineContext.Current.Resolve<ILogService>().ErrorAsync(exception.Message, exception, currentUser);
            //            }
            //        }
            //        finally
            //        {
            //            //rethrow the exception to show the error page
            //            ExceptionDispatchInfo.Throw(exception);
            //        }
            //    });
            //});
        }

        ///// <summary>
        ///// Configure applying forwarded headers to their matching fields on the current request.
        ///// </summary>
        ///// <param name="engine">Builder for configuring an engine's request pipeline</param>
        //public static void UseNodeProxy(this IEngineBuilder engine)
        //{
        //    //TODO: NODE PROXY/VPN/ETC!!!

        //    var ni2sSettings = NodeEngineContext.Current.Resolve<NI2SSettings>();

        //    if (ni2sSettings.Get<HostingConfig>().UseProxy)
        //    {
        //        //var options = new ForwardedHeadersOptions
        //        //{
        //        //    ForwardedHeaders = ForwardedHeaders.All,
        //        //    // IIS already serves as a reverse proxy and will add X-Forwarded headers to all requests,
        //        //    // so we need to increase this limit, otherwise, passed forwarding headers will be ignored.
        //        //    ForwardLimit = 2
        //        //};

        //        //if (!string.IsNullOrEmpty(ni2sSettings.Get<HostingConfig>().ForwardedForHeaderName))
        //        //    options.ForwardedForHeaderName = ni2sSettings.Get<HostingConfig>().ForwardedForHeaderName;

        //        //if (!string.IsNullOrEmpty(ni2sSettings.Get<HostingConfig>().ForwardedProtoHeaderName))
        //        //    options.ForwardedProtoHeaderName = ni2sSettings.Get<HostingConfig>().ForwardedProtoHeaderName;

        //        //if (!string.IsNullOrEmpty(ni2sSettings.Get<HostingConfig>().KnownProxies))
        //        //{
        //        //    foreach (var strIp in ni2sSettings.Get<HostingConfig>().KnownProxies.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
        //        //    {
        //        //        if (IPAddress.TryParse(strIp, out var ip))
        //        //            options.KnownProxies.Add(ip);
        //        //    }

        //        //    if (options.KnownProxies.Count > 1)
        //        //        options.ForwardLimit = null; //disable the limit, because KnownProxies is configured
        //        //}

        //        ////configure forwarding
        //        //engine.UseForwardedHeaders(options);
        //    }
        //}

        public static void UseClustering(this IEngineBuilder _)
        {
        }

        ///// <summary>
        ///// Add exception handling
        ///// </summary>
        ///// <param name="engine">Builder for configuring a engine's request pipeline</param>
        //public static void UseNopExceptionHandler(this IEngineBuilder engine)
        //{
        //    var appSettings = EngineContext.Current.Resolve<AppSettings>();
        //    var webHostEnvironment = EngineContext.Current.Resolve<IWebHostEnvironment>();
        //    var useDetailedExceptionPage = appSettings.Get<CommonConfig>().DisplayFullErrorStack || webHostEnvironment.IsDevelopment();
        //    if (useDetailedExceptionPage)
        //    {
        //        //get detailed exceptions for developing and testing purposes
        //        engine.UseDeveloperExceptionPage();
        //    }
        //    else
        //    {
        //        //or use special exception handler
        //        engine.UseExceptionHandler("/Error/Error");
        //    }

        //    //log errors
        //    engine.UseExceptionHandler(handler =>
        //    {
        //        handler.Run(async context =>
        //        {
        //            var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        //            if (exception == null)
        //                return;

        //            try
        //            {
        //                //check whether database is installed
        //                if (DataSettingsManager.IsDatabaseInstalled())
        //                {
        //                    //get current customer
        //                    var currentCustomer = await EngineContext.Current.Resolve<IWorkContext>().GetCurrentCustomerAsync();

        //                    //log error
        //                    await EngineContext.Current.Resolve<ILogger>().ErrorAsync(exception.Message, exception, currentCustomer);
        //                }
        //            }
        //            finally
        //            {
        //                //rethrow the exception to show the error page
        //                ExceptionDispatchInfo.Throw(exception);
        //            }
        //        });
        //    });
        //}

        ///// <summary>
        ///// Adds a special handler that checks for responses with the 404 status code that do not have a body
        ///// </summary>
        ///// <param name="engine">Builder for configuring a engine's request pipeline</param>
        //public static void UsePageNotFound(this IEngineBuilder engine)
        //{
        //    engine.UseStatusCodePages(async context =>
        //    {
        //        //handle 404 Not Found
        //        if (context.HttpContext.Response.StatusCode == StatusCodes.Status404NotFound)
        //        {
        //            var webHelper = EngineContext.Current.Resolve<IWebHelper>();
        //            if (!webHelper.IsStaticResource())
        //            {
        //                //get original path and query
        //                var originalPath = context.HttpContext.Request.Path;
        //                var originalQueryString = context.HttpContext.Request.QueryString;

        //                if (DataSettingsManager.IsDatabaseInstalled())
        //                {
        //                    var commonSettings = EngineContext.Current.Resolve<CommonSettings>();

        //                    if (commonSettings.Log404Errors)
        //                    {
        //                        var logger = EngineContext.Current.Resolve<ILogger>();
        //                        var workContext = EngineContext.Current.Resolve<IWorkContext>();

        //                        await logger.ErrorAsync($"Error 404. The requested page ({originalPath}) was not found",
        //                            customer: await workContext.GetCurrentCustomerAsync());
        //                    }
        //                }

        //                try
        //                {
        //                    //get new path
        //                    var pageNotFoundPath = "/page-not-found";
        //                    //re-execute request with new path
        //                    context.HttpContext.Response.Redirect(context.HttpContext.Request.PathBase + pageNotFoundPath);
        //                }
        //                finally
        //                {
        //                    //return original path to request
        //                    context.HttpContext.Request.QueryString = originalQueryString;
        //                    context.HttpContext.Request.Path = originalPath;
        //                }
        //            }
        //        }
        //    });
        //}

        ///// <summary>
        ///// Adds a special handler that checks for responses with the 400 status code (bad request)
        ///// </summary>
        ///// <param name="engine">Builder for configuring a engine's request pipeline</param>
        //public static void UseBadRequestResult(this IEngineBuilder engine)
        //{
        //    engine.UseStatusCodePages(async context =>
        //    {
        //        //handle 404 (Bad request)
        //        if (context.HttpContext.Response.StatusCode == StatusCodes.Status400BadRequest)
        //        {
        //            var logger = EngineContext.Current.Resolve<ILogger>();
        //            var workContext = EngineContext.Current.Resolve<IWorkContext>();
        //            await logger.ErrorAsync("Error 400. Bad request", null, customer: await workContext.GetCurrentCustomerAsync());
        //        }
        //    });
        //}

        ///// <summary>
        ///// Configure middleware for dynamically compressing HTTP responses
        ///// </summary>
        ///// <param name="engine">Builder for configuring a engine's request pipeline</param>
        //public static void UseNopResponseCompression(this IEngineBuilder engine)
        //{
        //    if (!DataSettingsManager.IsDatabaseInstalled())
        //        return;

        //    //whether to use compression (gzip by default)
        //    if (EngineContext.Current.Resolve<CommonSettings>().UseResponseCompression)
        //        engine.UseResponseCompression();
        //}

        ///// <summary>
        ///// Adds WebOptimizer to the <see cref="IEngineBuilder"/> request execution pipeline
        ///// </summary>
        ///// <param name="engine">Builder for configuring a engine's request pipeline</param>
        //public static void UseNopWebOptimizer(this IEngineBuilder engine)
        //{
        //    var appSettings = Singleton<AppSettings>.Instance;
        //    var woConfig = appSettings.Get<WebOptimizerConfig>();

        //    if (!woConfig.EnableCssBundling && !woConfig.EnableJavaScriptBundling)
        //        return;

        //    var fileProvider = EngineContext.Current.Resolve<INopFileProvider>();
        //    var webHostEnvironment = EngineContext.Current.Resolve<IWebHostEnvironment>();

        //    engine.UseWebOptimizer(webHostEnvironment,
        //    [
        //        new FileProviderOptions
        //    {
        //        RequestPath =  new PathString("/Plugins"),
        //        FileProvider = new PhysicalFileProvider(fileProvider.MapPath(@"Plugins"))
        //    },
        //    new FileProviderOptions
        //    {
        //        RequestPath =  new PathString("/Themes"),
        //        FileProvider = new PhysicalFileProvider(fileProvider.MapPath(@"Themes"))
        //    }
        //    ]);
        //}

        ///// <summary>
        ///// Configure static file serving
        ///// </summary>
        ///// <param name="engine">Builder for configuring a engine's request pipeline</param>
        //public static void UseNopStaticFiles(this IEngineBuilder engine)
        //{
        //    var fileProvider = EngineContext.Current.Resolve<INopFileProvider>();
        //    var appSettings = EngineContext.Current.Resolve<AppSettings>();

        //    void staticFileResponse(StaticFileResponseContext context)
        //    {
        //        if (!string.IsNullOrEmpty(appSettings.Get<CommonConfig>().StaticFilesCacheControl))
        //            context.Context.Response.Headers.Append(HeaderNames.CacheControl, appSettings.Get<CommonConfig>().StaticFilesCacheControl);
        //    }

        //    //add handling if sitemaps 
        //    engine.UseStaticFiles(new StaticFileOptions
        //    {
        //        FileProvider = new PhysicalFileProvider(fileProvider.GetAbsolutePath(NopSeoDefaults.SitemapXmlDirectory)),
        //        RequestPath = new PathString($"/{NopSeoDefaults.SitemapXmlDirectory}"),
        //        OnPrepareResponse = context =>
        //        {
        //            if (!DataSettingsManager.IsDatabaseInstalled() ||
        //                !EngineContext.Current.Resolve<SitemapXmlSettings>().SitemapXmlEnabled)
        //            {
        //                context.Context.Response.StatusCode = StatusCodes.Status403Forbidden;
        //                context.Context.Response.ContentLength = 0;
        //                context.Context.Response.Body = Stream.Null;
        //            }
        //        }
        //    });

        //    //common static files
        //    engine.UseStaticFiles(new StaticFileOptions { OnPrepareResponse = staticFileResponse });

        //    //themes static files
        //    engine.UseStaticFiles(new StaticFileOptions
        //    {
        //        FileProvider = new PhysicalFileProvider(fileProvider.MapPath(@"Themes")),
        //        RequestPath = new PathString("/Themes"),
        //        OnPrepareResponse = staticFileResponse
        //    });

        //    //plugins static files
        //    var staticFileOptions = new StaticFileOptions
        //    {
        //        FileProvider = new PhysicalFileProvider(fileProvider.MapPath(@"Plugins")),
        //        RequestPath = new PathString("/Plugins"),
        //        OnPrepareResponse = staticFileResponse
        //    };

        //    //exclude files in blacklist
        //    if (!string.IsNullOrEmpty(appSettings.Get<CommonConfig>().PluginStaticFileExtensionsBlacklist))
        //    {
        //        var fileExtensionContentTypeProvider = new FileExtensionContentTypeProvider();

        //        foreach (var ext in appSettings.Get<CommonConfig>().PluginStaticFileExtensionsBlacklist
        //                     .Split(';', ',')
        //                     .Select(e => e.Trim().ToLowerInvariant())
        //                     .Select(e => $"{(e.StartsWith(".") ? string.Empty : ".")}{e}")
        //                     .Where(fileExtensionContentTypeProvider.Mappings.ContainsKey))
        //        {
        //            fileExtensionContentTypeProvider.Mappings.Remove(ext);
        //        }

        //        staticFileOptions.ContentTypeProvider = fileExtensionContentTypeProvider;
        //    }

        //    engine.UseStaticFiles(staticFileOptions);

        //    //add support for backups
        //    var provider = new FileExtensionContentTypeProvider
        //    {
        //        Mappings = { [".bak"] = MimeTypes.EngineOctetStream }
        //    };

        //    engine.UseStaticFiles(new StaticFileOptions
        //    {
        //        FileProvider = new PhysicalFileProvider(fileProvider.GetAbsolutePath(NopCommonDefaults.DbBackupsPath)),
        //        RequestPath = new PathString("/db_backups"),
        //        ContentTypeProvider = provider,
        //        OnPrepareResponse = context =>
        //        {
        //            if (!DataSettingsManager.IsDatabaseInstalled() ||
        //                !EngineContext.Current.Resolve<IPermissionService>().AuthorizeAsync(StandardPermissionProvider.ManageMaintenance).Result)
        //            {
        //                context.Context.Response.StatusCode = StatusCodes.Status404NotFound;
        //                context.Context.Response.ContentLength = 0;
        //                context.Context.Response.Body = Stream.Null;
        //            }
        //        }
        //    });

        //    //add support for webmanifest files
        //    provider.Mappings[".webmanifest"] = MimeTypes.EngineManifestJson;

        //    engine.UseStaticFiles(new StaticFileOptions
        //    {
        //        FileProvider = new PhysicalFileProvider(fileProvider.GetAbsolutePath("icons")),
        //        RequestPath = "/icons",
        //        ContentTypeProvider = provider
        //    });

        //    if (DataSettingsManager.IsDatabaseInstalled())
        //    {
        //        engine.UseStaticFiles(new StaticFileOptions
        //        {
        //            FileProvider = EngineContext.Current.Resolve<IRoxyFilemanFileProvider>(),
        //            RequestPath = new PathString(NopRoxyFilemanDefaults.DefaultRootDirectory),
        //            OnPrepareResponse = staticFileResponse
        //        });
        //    }

        //    if (appSettings.Get<CommonConfig>().ServeUnknownFileTypes)
        //    {
        //        engine.UseStaticFiles(new StaticFileOptions
        //        {
        //            FileProvider = new PhysicalFileProvider(fileProvider.GetAbsolutePath(".well-known")),
        //            RequestPath = new PathString("/.well-known"),
        //            ServeUnknownFileTypes = true,
        //        });
        //    }
        //}

        ///// <summary>
        ///// Configure middleware checking whether requested page is keep alive page
        ///// </summary>
        ///// <param name="engine">Builder for configuring a engine's request pipeline</param>
        //public static void UseKeepAlive(this IEngineBuilder engine)
        //{
        //    engine.UseMiddleware<KeepAliveMiddleware>();
        //}

        ///// <summary>
        ///// Configure middleware checking whether database is installed
        ///// </summary>
        ///// <param name="engine">Builder for configuring a engine's request pipeline</param>
        //public static void UseInstallUrl(this IEngineBuilder engine)
        //{
        //    engine.UseMiddleware<InstallUrlMiddleware>();
        //}

        ///// <summary>
        ///// Adds the authentication middleware, which enables authentication capabilities.
        ///// </summary>
        ///// <param name="engine">Builder for configuring a engine's request pipeline</param>
        //public static void UseNopAuthentication(this IEngineBuilder engine)
        //{
        //    //check whether database is installed
        //    if (!DataSettingsManager.IsDatabaseInstalled())
        //        return;

        //    engine.UseMiddleware<AuthenticationMiddleware>();
        //}

        ///// <summary>
        ///// Configure PDF
        ///// </summary>
        //public static void UseNopPdf(this IEngineBuilder _)
        //{
        //    if (!DataSettingsManager.IsDatabaseInstalled())
        //        return;

        //    var fileProvider = EngineContext.Current.Resolve<INopFileProvider>();
        //    var fontPaths = fileProvider.EnumerateFiles(fileProvider.MapPath("~/App_Data/Pdf/"), "*.ttf") ?? Enumerable.Empty<string>();

        //    //write placeholder characters instead of unavailable glyphs for both debug/release configurations
        //    QuestPDF.Settings.CheckIfAllTextGlyphsAreAvailable = false;

        //    foreach (var fp in fontPaths)
        //    {
        //        FontManager.RegisterFont(File.OpenRead(fp));
        //    }
        //}

        ///// <summary>
        ///// Configure the request localization feature
        ///// </summary>
        ///// <param name="engine">Builder for configuring a engine's request pipeline</param>
        //public static void UseNopRequestLocalization(this IEngineBuilder engine)
        //{
        //    engine.UseRequestLocalization(options =>
        //    {
        //        if (!DataSettingsManager.IsDatabaseInstalled())
        //            return;

        //        var languageService = EngineContext.Current.Resolve<ILanguageService>();
        //        var localizationSettings = EngineContext.Current.Resolve<LocalizationSettings>();

        //        //prepare supported cultures
        //        var cultures = languageService
        //            .GetAllLanguages()
        //            .OrderBy(language => language.DisplayOrder)
        //            .Select(language => new CultureInfo(language.LanguageCulture))
        //            .ToList();
        //        options.SupportedCultures = cultures;
        //        options.SupportedUICultures = cultures;
        //        options.DefaultRequestCulture = new RequestCulture(cultures.FirstOrDefault() ?? new CultureInfo(NopCommonDefaults.DefaultLanguageCulture));
        //        options.ApplyCurrentCultureToResponseHeaders = true;

        //        //configure culture providers
        //        options.AddInitialRequestCultureProvider(new NopSeoUrlCultureProvider());
        //        var cookieRequestCultureProvider = options.RequestCultureProviders.OfType<CookieRequestCultureProvider>().FirstOrDefault();
        //        if (cookieRequestCultureProvider is not null)
        //            cookieRequestCultureProvider.CookieName = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.CultureCookie}";
        //        if (!localizationSettings.AutomaticallyDetectLanguage)
        //        {
        //            var headerRequestCultureProvider = options
        //                .RequestCultureProviders
        //                .OfType<AcceptLanguageHeaderRequestCultureProvider>()
        //                .FirstOrDefault();
        //            if (headerRequestCultureProvider is not null)
        //                options.RequestCultureProviders.Remove(headerRequestCultureProvider);
        //        }
        //    });
        //}

        ///// <summary>
        ///// Configure Endpoints routing
        ///// </summary>
        ///// <param name="engine">Builder for configuring a engine's request pipeline</param>
        //public static void UseNopEndpoints(this IEngineBuilder engine)
        //{
        //    //Execute the endpoint selected by the routing middleware
        //    engine.UseEndpoints(endpoints =>
        //    {
        //        //register all routes
        //        EngineContext.Current.Resolve<IRoutePublisher>().RegisterRoutes(endpoints);
        //    });
        //}

        ///// <summary>
        ///// Configure applying forwarded headers to their matching fields on the current request.
        ///// </summary>
        ///// <param name="engine">Builder for configuring a engine's request pipeline</param>
        //public static void UseNopProxy(this IEngineBuilder engine)
        //{
        //    var appSettings = EngineContext.Current.Resolve<AppSettings>();
        //    var hostingConfig = appSettings.Get<HostingConfig>();

        //    if (hostingConfig.UseProxy)
        //    {
        //        var options = new ForwardedHeadersOptions
        //        {
        //            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
        //            // IIS already serves as a reverse proxy and will add X-Forwarded headers to all requests,
        //            // so we need to increase this limit, otherwise, passed forwarding headers will be ignored.
        //            ForwardLimit = 2
        //        };

        //        if (!string.IsNullOrEmpty(hostingConfig.ForwardedForHeaderName))
        //            options.ForwardedForHeaderName = hostingConfig.ForwardedForHeaderName;

        //        if (!string.IsNullOrEmpty(hostingConfig.ForwardedProtoHeaderName))
        //            options.ForwardedProtoHeaderName = hostingConfig.ForwardedProtoHeaderName;

        //        options.KnownNetworks.Clear();
        //        options.KnownProxies.Clear();

        //        if (!string.IsNullOrEmpty(hostingConfig.KnownProxies))
        //        {
        //            foreach (var strIp in hostingConfig.KnownProxies.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
        //            {
        //                if (IPAddress.TryParse(strIp, out var ip))
        //                    options.KnownProxies.Add(ip);
        //            }
        //        }

        //        if (!string.IsNullOrEmpty(hostingConfig.KnownNetworks))
        //        {
        //            foreach (var strIpNet in hostingConfig.KnownNetworks.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
        //            {
        //                var ipNetParts = strIpNet.Split("/");
        //                if (ipNetParts.Length == 2)
        //                {
        //                    if (IPAddress.TryParse(ipNetParts[0], out var ip) && int.TryParse(ipNetParts[1], out var length))
        //                        options.KnownNetworks.Add(new IPNetwork(ip, length));
        //                }
        //            }
        //        }

        //        if (options.KnownProxies.Count > 1 || options.KnownNetworks.Count > 1)
        //            options.ForwardLimit = null; //disable the limit, because KnownProxies is configured

        //        //configure forwarding
        //        engine.UseForwardedHeaders(options);
        //    }
        //}

        ///// <summary>
        ///// Configure WebMarkupMin
        ///// </summary>
        ///// <param name="engine">Builder for configuring a engine's request pipeline</param>
        //public static void UseNopWebMarkupMin(this IEngineBuilder engine)
        //{
        //    //check whether database is installed
        //    if (!DataSettingsManager.IsDatabaseInstalled())
        //        return;

        //    engine.UseWebMarkupMin();
        //}
    }
}
