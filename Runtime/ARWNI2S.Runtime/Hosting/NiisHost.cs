﻿using ARWNI2S.Engine.Configuration.Options;
using ARWNI2S.Node.Builder;
using ARWNI2S.Node.Hosting.Extensions;
using ARWNI2S.Node.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ARWNI2S.Node.Hosting
{
    /// <summary>
    /// Provides convenience methods for creating instances of <see cref="INodeHost"/> and <see cref="INodeHostBuilder"/> with pre-configured defaults.
    /// </summary>
    internal static class NiisHost
    {
        internal static void ConfigureNodeDefaults(INodeHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((ctx, cb) =>
            {
                if (ctx.HostingEnvironment.IsDevelopment())
                {
                    StaticNodeAssetsLoader.UseStaticNodeAssets(ctx.HostingEnvironment, ctx.Configuration);
                }
            });

            ConfigureNodeDefaultsWorker(
                builder.UseCoreEngine(ConfigureCoreEngine),
                services =>
                {
                    //services.AddRouting();
                });

            builder
                .UseNetwork()
                .UseNI2SIntegration();
        }

        private static void ConfigureCoreEngine(NodeHostBuilderContext builderContext, EngineOptions options)
        {
            options.Configure(builderContext.Configuration.GetSection("CoreEngine"), reloadOnChange: true);
        }

        private static void ConfigureNodeDefaultsWorker(INodeHostBuilder builder, Action<IServiceCollection> configureRouting)
        {
            builder.ConfigureServices((hostingContext, services) =>
            {
                // Fallback
                //services.PostConfigure<HostFilteringOptions>(options =>
                //{
                //    if (options.AllowedHosts == null || options.AllowedHosts.Count() == 0)
                //    {
                //        // "AllowedHosts": "localhost;127.0.0.1;[::1]"
                //        var hosts = hostingContext.Configuration["AllowedHosts"]?.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                //        // Fall back to "*" to disable.
                //        options.AllowedHosts = (hosts?.Length > 0 ? hosts : new[] { "*" });
                //    }
                //});
                //// Change notification
                //services.AddSingleton<IOptionsChangeTokenSource<HostFilteringOptions>>(
                //        new ConfigurationChangeTokenSource<HostFilteringOptions>(hostingContext.Configuration));

                //services.AddTransient<IStartupFilter, HostFilteringStartupFilter>();
                //services.AddTransient<IStartupFilter, ForwardedHeadersStartupFilter>();
                //services.AddTransient<IConfigureOptions<ForwardedHeadersOptions>, ForwardedHeadersOptionsSetup>();

                // Provide a way for the default host builder to configure routing. This probably means calling AddRouting.
                // A lambda is used here because we don't want to reference AddRouting directly because of trimming.
                // This avoids the overhead of calling AddRoutingCore multiple times on app startup.
                if (configureRouting == null)
                {
                    //services.AddRoutingCore();
                }
                else
                {
                    configureRouting(services);
                }
            });
        }
    }
}