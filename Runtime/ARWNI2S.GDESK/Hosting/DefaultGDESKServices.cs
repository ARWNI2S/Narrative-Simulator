using ARWNI2S.Engine.Infrastructure;
using ARWNI2S.Engine.Simulation;
using ARWNI2S.Engine.Simulation.Kernel;
using ARWNI2S.Engine.Simulation.Runtime;
using ARWNI2S.Engine.Simulation.Time;
using ARWNI2S.Infrastructure.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ARWNI2S.Engine.Configuration.Options;
using ARWNI2S.Engine.Configuration.Validators;
using ARWNI2S.Infrastructure.Configuration.Validators;
using ARWNI2S.Engine.Simulation.Simulable.Builder;
using ARWNI2S.Engine.Simulation.Simulable.Factory;

namespace ARWNI2S.Engine.Hosting
{
    internal static class DefaultGDESKServices
    {
        private static readonly ServiceDescriptor ServiceDescriptor = new(typeof(ServicesAdded), new ServicesAdded());

        internal static void AddDefaultServices(IGDESKBuilder builder)
        {
            var services = builder.Services;
            if (services.Contains(ServiceDescriptor))
            {
                return;
            }

            services.Add(ServiceDescriptor);

            // Common services
            services.AddLogging();
            services.AddOptions();
            services.TryAddSingleton(TimeProvider.System);

            // Registro del fallback genérico de Simulable Factory y Builder
            services.TryAddSingleton(typeof(ISimulableObjectFactory<>), typeof(GenericSimulableObjectFactory<>));
            services.TryAddSingleton(typeof(ISimulableObjectBuilder<>), typeof(GenericObjectBuilder<>));

            services.AddSingleton<IOptionFormatter>(sp => sp.GetService<IOptionFormatter<LoadSheddingOptions>>());
            services.AddTransient<IConfigurationValidator, LoadSheddingValidator>();

            // Statistics
            services.AddSingleton<IEnvironmentStatisticsProvider, EnvironmentStatisticsProvider>();

            services.AddSingleton<Dispatcher>();
            services.AddSingleton<EventPool>();
            services.TryAddSingleton<OverloadDetector>();

            services.TryAddSingleton<ILocalKernelDetails, LocalKernelDetails>();
            services.TryAddSingleton<SimulableRuntime>();
            services.TryAddSingleton<ISimulableRuntime, SimulableRuntime>();

            if (!services.Any(x => x.ServiceType == typeof(ISimulationClock)))
            {
                services.TryAddSingleton<SimulationClock>();
                services.AddFromExisting<ISimulationClock, SimulationClock>();
            }

            if (!services.Any(x => x.ServiceType == typeof(ISimulation)))
            {
                services.TryAddSingleton<DefaultSimulation>();
                services.AddFromExisting<ISimulation, DefaultSimulation>();
            }

            ApplyConfiguration(builder);
        }

        private static void ApplyConfiguration(IGDESKBuilder builder)
        {
            var services = builder.Services;
            var cfg = builder.Configuration.GetSection("GDESK");
            //var knownProviderTypes = GetRegisteredProviders();

            //if (cfg["Name"] is { Length: > 0 } name)
            //{
            //    services.Configure<SiloOptions>(siloOptions => siloOptions.SiloName = name);
            //}

            //services.Configure<ClusterOptions>(cfg);
            //services.Configure<SiloMessagingOptions>(cfg.GetSection("Messaging"));
            //if (cfg.GetSection("Endpoints") is { } ep && ep.Exists())
            //{
            //    services.Configure<EndpointOptions>(o => o.Bind(ep));
            //}

            //if (bool.TryParse(cfg["EnableDistributedTracing"], out var enableDistributedTracing) && enableDistributedTracing)
            //{
            //    builder.AddActivityPropagation();
            //}

            //ApplySubsection(builder, cfg, knownProviderTypes, "Clustering");
            //ApplySubsection(builder, cfg, knownProviderTypes, "Reminders");
            //ApplyNamedSubsections(builder, cfg, knownProviderTypes, "BroadcastChannel");
            //ApplyNamedSubsections(builder, cfg, knownProviderTypes, "Streaming");
            //ApplyNamedSubsections(builder, cfg, knownProviderTypes, "GrainStorage");
            //ApplyNamedSubsections(builder, cfg, knownProviderTypes, "GrainDirectory");

            //static void ConfigureProvider(IGDESKBuilder builder, Dictionary<(string Kind, string Name), Type> knownProviderTypes, string kind, string? name, IConfigurationSection configurationSection)
            //{
            //    var providerType = configurationSection["ProviderType"] ?? "Default";
            //    var provider = GetRequiredProvider(knownProviderTypes, kind, providerType);
            //    provider.Configure(builder, name, configurationSection);
            //}


            //static void ApplySubsection(IGDESKBuilder builder, IConfigurationSection cfg, Dictionary<(string Kind, string Name), Type> knownProviderTypes, string sectionName)
            //{
            //    if (cfg.GetSection(sectionName) is { } section && section.Exists())
            //    {
            //        ConfigureProvider(builder, knownProviderTypes, sectionName, name: null, section);
            //    }
            //}

            //static void ApplyNamedSubsections(IGDESKBuilder builder, IConfigurationSection cfg, Dictionary<(string Kind, string Name), Type> knownProviderTypes, string sectionName)
            //{
            //    if (cfg.GetSection(sectionName) is { } section && section.Exists())
            //    {
            //        foreach (var child in section.GetChildren())
            //        {
            //            ConfigureProvider(builder, knownProviderTypes, sectionName, name: child.Key, child);
            //        }
            //    }
            //}
        }

        //private class AllowNI2STypes : ITypeNameFilter
        //{
        //    public bool? IsTypeNameAllowed(string typeName, string assemblyName)
        //    {
        //        if (assemblyName is { Length: > 0 } && assemblyName.Contains("ARWNI2S"))
        //        {
        //            return true;
        //        }

        //        return null;
        //    }
        //}

        private class ServicesAdded { }
    }
}