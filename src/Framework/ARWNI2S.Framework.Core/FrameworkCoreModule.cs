using ARWNI2S.Engine.Builder;
using ARWNI2S.Engine.Extensibility;
using ARWNI2S.Framework.Configuration;
using ARWNI2S.Lifecycle;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ARWNI2S.Framework
{
    internal class FrameworkCoreModule : EngineModule
    {
        public override int Order => NI2SLifecycleStage.RuntimeServices;


        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            ////user agent helper
            //services.AddScoped<IUserAgentHelper, UserAgentHelper>();

            ////plugins
            //services.AddScoped<IPluginService, PluginService>();
            //services.AddScoped<OfficialFeedManager>();





            services.AddScoped<ISettingService, SettingService>();
        }

        public override void ConfigureEngine(IEngineBuilder engineBuilder)
        {
            ////check whether requested page is keep alive page
            //application.UseKeepAlive();

            ////check whether database is installed
            //application.UseInstallUrl();

            ////use HTTP session
            //application.UseSession();

            ////use request localization
            //application.UseNopRequestLocalization();

            ////configure PDF
            //application.UseNopPdf();

            //engineBuilder.EngineServices.GetRequiredService<IModuleManager>().FrameworkModules[typeof(CoreModule)] = this;
            base.ConfigureEngine(engineBuilder);
        }
    }
}
