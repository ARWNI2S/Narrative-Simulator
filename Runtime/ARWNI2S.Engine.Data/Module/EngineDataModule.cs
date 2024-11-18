using ARWNI2S.Engine.Modules;
using ARWNI2S.Infrastructure.Engine.Builder;
using Microsoft.Extensions.DependencyInjection;
using ServiceLifecycleStage = ARWNI2S.Infrastructure.Lifecycle.ServiceLifecycleStage;

#pragma warning disable IDE0130 // El espacio de nombres no coincide con la estructura de carpetas
namespace ARWNI2S.Engine
#pragma warning restore IDE0130 // El espacio de nombres no coincide con la estructura de carpetas
{
    [EngineModule(ServiceLifecycleStage.First)]
    public sealed class EngineDataModule : EngineModuleBase
    {
        protected override void ConfigureEngine(IEngineBuilder engine)
        {

        }

        protected override void ConfigureModuleServices(IServiceCollection services)
        {
            //services.AddSingleton<IAssetTypeProvider, DataAssetTypeProvider>();
            //services.AddSingleton<DataAssetService>();
            //services.AddSingleton<IDataAssetService, DataAssetService>();
            //services.AddFromExisting<IAssetService, DataAssetService>();
        }

        protected override Task OnStartAsync(CancellationToken token)
        {
            return base.OnStartAsync(token);
        }
    }
}
