using ARWNI2S.Engine.Hosting.Extensions;
using ARWNI2S.Node.Builder;
using ARWNI2S.Node.Core.Configuration;
using ARWNI2S.Node.Core.Infrastructure;
using ARWNI2S.Node.Hosting;
using ARWNI2S.Node.Hosting.Extensions;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ARWNI2S.Node
{
    public static class EntryPoint
    {
        public static async Task RunDefaultsAsync(string[] args)
        {
            var app = await StartDefaultsAsync(args);

            await app.RunAsync();
        }

        public static NodeEngineBuilder CreateDefaults(string[] args)
        {
            var builder = NodeEngineHost.CreateBuilder(args);

            builder.Configuration.AddJsonFile(NI2SConfigurationDefaults.NI2SSettingsFilePath, true, true);
            if (!string.IsNullOrEmpty(builder.Environment?.EnvironmentName))
            {
                var path = string.Format(NI2SConfigurationDefaults.NI2SSettingsEnvironmentFilePath, builder.Environment.EnvironmentName);
                builder.Configuration.AddJsonFile(path, true, true);
            }
            builder.Configuration.AddEnvironmentVariables();

            //load application settings
            builder.Services.ConfigureEngineSettings(builder);

            var appSettings = Singleton<NI2SSettings>.Instance;
            var useAutofac = appSettings.Get<CommonConfig>().UseAutofac;

            if (useAutofac)
                builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            else
                builder.Host.UseDefaultServiceProvider(options =>
                {
                    //we don't validate the scopes, since at the app start and the initial configuration we need 
                    //to resolve some services (registered as "scoped") through the root container
                    options.ValidateScopes = false;
                    options.ValidateOnBuild = true;
                });

            //add services to the application and configure service provider
            builder.Services.ConfigureEngineServices(builder);

            return builder;
        }

        public static NodeEngineHost BuildDefaults(string[] args)
        {
            return CreateDefaults(args).Build();
        }

        public static NodeEngineHost ConfigureDefaults(string[] args)
        {
            var engine = BuildDefaults(args);

            //configure the engine
            engine.ConfigureEngine();

            return engine;
        }

        public static async Task<NodeEngineHost> StartDefaultsAsync(string[] args)
        {
            var app = ConfigureDefaults(args);

            await app.StartEngineAsync();

            return app;
        }

    }
}