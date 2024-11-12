using ARWNI2S.Node.Core.Configuration;
using ARWNI2S.Node.Core.Infrastructure;
using ARWNI2S.Runtime.Hosting;
using ARWNI2S.Runtime.Hosting.Extensions;
using Autofac.Extensions.DependencyInjection;

namespace ARWNI2S.Narrator
{
    public class Program
    {
        public static async Task Main(string[] args)
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

            var app = builder.Build();

            //configure the application HTTP request pipeline
            app.ConfigureEngine();
            await app.StartEngineAsync();

            await app.RunAsync();
        }
    }
}