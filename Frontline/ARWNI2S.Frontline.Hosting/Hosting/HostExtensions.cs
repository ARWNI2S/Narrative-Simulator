using ARWNI2S.Node.Core.Configuration;
using ARWNI2S.Node.Core.Infrastructure;
using ARWNI2S.Node.Data;
using ARWNI2S.Node.Data.Migrations;
using ARWNI2S.Node.Services.Logging;
using ARWNI2S.Node.Services.Plugins;
using ARWNI2S.Node.Services.ScheduleTasks;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace ARWNI2S.Frontline.Hosting
{
    public static class HostExtensions
    {
        /// <summary>
        /// Configure the node NI2S engine
        /// </summary>
        /// <param name="application">Builder for configuring a node's NI2S engine</param>
        public static void ConfigureEngine(this IHost application)
        {
            EngineContext.Current.ConfigureEngine(application);
        }

        /// <summary>
        /// Starts the engine
        /// </summary>
        /// <param name="_">unused</param>
        /// <returns>async task</returns>
        public static async Task StartEngineAsync(this IHost _)
        {
            var engine = EngineContext.Current;

            //further actions are performed only when the database is installed
            if (DataSettingsManager.IsDatabaseInstalled())
            {
                //log node start
                await engine.Resolve<ILogService>().InformationAsync("Node started");

                //install and update modules
                var moduleService = engine.Resolve<IModuleService>();
                await moduleService.InstallModulesAsync();
                await moduleService.UpdateModulesAsync();

                //update dragonCorp core and db
                var migrationManager = engine.Resolve<IMigrationManager>();
                var assembly = Assembly.GetAssembly(typeof(HostExtensions));
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
        public static void UseNI2SExceptionHandler(this IHost application)
        {
            var ni2sSettings = EngineContext.Current.Resolve<NI2SSettings>();
            var hostEnvironment = EngineContext.Current.Resolve<IHostEnvironment>();
            var useDetailedExceptions = ni2sSettings.Get<CommonConfig>().DisplayFullErrorStack || hostEnvironment.IsDevelopment();
            if (useDetailedExceptions)
            {
                //get detailed exceptions for developing and testing purposes
                //application.UseDeveloperExceptionPage();
            }
            else
            {
                //or use special exception handler
                //application.UseExceptionHandler("/Error/Error");
            }

            //log errors
            //application.UseExceptionHandler(handler =>
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

        /// <summary>
        /// Adds a special handler that checks for responses with the 404 status code that do not have a body
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseNotFoundResult(this IHost application)
        {
            //application.UseStatusCodePages(async context =>
            //{
            //    //handle 404 Not Found
            //    if (context.HttpContext.Response.StatusCode == StatusCodes.Status404NotFound)
            //    {
            //        var webHelper = EngineContext.Current.Resolve<IWebHelper>();
            //        if (!webHelper.IsStaticResource())
            //        {
            //            //get original path and query
            //            var originalPath = context.HttpContext.Request.Path;
            //            var originalQueryString = context.HttpContext.Request.QueryString;

            //            if (DataSettingsManager.IsDatabaseInstalled())
            //            {
            //                var commonSettings = EngineContext.Current.Resolve<CommonSettings>();

            //                if (commonSettings.Log404Errors)
            //                {
            //                    var logger = EngineContext.Current.Resolve<ILogService>();
            //                    var workContext = EngineContext.Current.Resolve<IWorkContext>();

            //                    await logger.ErrorAsync($"Error 404. The requested page ({originalPath}) was not found",
            //                        user: await workContext.GetCurrentUserAsync());
            //                }
            //            }

            //            try
            //            {
            //                //get new path
            //                var pageNotFoundPath = "/page-not-found";
            //                //re-execute request with new path
            //                context.HttpContext.Response.Redirect(context.HttpContext.Request.PathBase + pageNotFoundPath);
            //            }
            //            finally
            //            {
            //                //return original path to request
            //                context.HttpContext.Request.QueryString = originalQueryString;
            //                context.HttpContext.Request.Path = originalPath;
            //            }
            //        }
            //    }
            //});
        }

        /// <summary>
        /// Adds a special handler that checks for responses with the 400 status code (bad request)
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseBadRequestResult(this IHost application)
        {
            //application.UseStatusCodePages(async context =>
            //{
            //    //handle 404 (Bad request)
            //    if (context.HttpContext.Response.StatusCode == StatusCodes.Status400BadRequest)
            //    {
            //        var logger = EngineContext.Current.Resolve<ILogService>();
            //        var workContext = EngineContext.Current.Resolve<IWorkContext>();
            //        await logger.ErrorAsync("Error 400. Bad request", null, user: await workContext.GetCurrentUserAsync());
            //    }
            //});
        }

        /// <summary>
        /// Configure applying forwarded headers to their matching fields on the current request.
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseNodeProxy(this IHost application)
        {
            var ni2sSettings = EngineContext.Current.Resolve<NI2SSettings>();

            if (ni2sSettings.Get<HostingConfig>().UseProxy)
            {
                //var options = new ForwardedHeadersOptions
                //{
                //    ForwardedHeaders = ForwardedHeaders.All,
                //    // IIS already serves as a reverse proxy and will add X-Forwarded headers to all requests,
                //    // so we need to increase this limit, otherwise, passed forwarding headers will be ignored.
                //    ForwardLimit = 2
                //};

                //if (!string.IsNullOrEmpty(ni2sSettings.Get<HostingConfig>().ForwardedForHeaderName))
                //    options.ForwardedForHeaderName = ni2sSettings.Get<HostingConfig>().ForwardedForHeaderName;

                //if (!string.IsNullOrEmpty(ni2sSettings.Get<HostingConfig>().ForwardedProtoHeaderName))
                //    options.ForwardedProtoHeaderName = ni2sSettings.Get<HostingConfig>().ForwardedProtoHeaderName;

                //if (!string.IsNullOrEmpty(ni2sSettings.Get<HostingConfig>().KnownProxies))
                //{
                //    foreach (var strIp in ni2sSettings.Get<HostingConfig>().KnownProxies.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
                //    {
                //        if (IPAddress.TryParse(strIp, out var ip))
                //            options.KnownProxies.Add(ip);
                //    }

                //    if (options.KnownProxies.Count > 1)
                //        options.ForwardLimit = null; //disable the limit, because KnownProxies is configured
                //}

                ////configure forwarding
                //application.UseForwardedHeaders(options);
            }
        }
    }
}
