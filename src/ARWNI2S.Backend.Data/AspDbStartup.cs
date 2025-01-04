using ARWNI2S.Configuration;
using ARWNI2S.Engine.Caching;
using ARWNI2S.Engine.Data;
using ARWNI2S.Engine.Data.Migrations;
using ARWNI2S.Environment;
using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Conventions;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ARWNI2S.Backend.Data
{
    internal class AspDbStartup : INiisStartup
    {
        public int Order => 10;

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {

            var mAssemblies = Singleton<ITypeFinder>.Instance.FindClassesOfType<MigrationBase>()
                .Select(t => t.Assembly)
                .Where(assembly => !assembly.FullName.Contains("FluentMigrator.Runner"))
                .Distinct()
                .ToArray();

            services
                // add common FluentMigrator services
                .AddFluentMigratorCore()
                .AddScoped<IProcessorAccessor, NI2SProcessorAccessor>()
                // set accessor for the connection string
                .AddScoped<IConnectionStringAccessor>(x => DataSettingsManager.LoadSettings())
                .AddSingleton<IMigrationManager, MigrationManager>()
                .AddSingleton<IConventionSet, NI2SConventionSet>()
                .ConfigureRunner(rb =>
                    rb.WithVersionTable(new MigrationVersionInfo()).AddSqlServer().AddMySql5().AddPostgres()
                        // define the assembly containing the migrations
                        .ScanIn(mAssemblies).For.Migrations());

            services.AddTransient(p => new Lazy<IVersionLoader>(p.GetRequiredService<IVersionLoader>()));

            //data layer
            services.AddTransient<IDataProviderManager, DataProviderManager>();
            services.AddTransient(serviceProvider =>
                serviceProvider.GetRequiredService<IDataProviderManager>().DataProvider);

            //repositories	
            services.AddScoped(typeof(IRepository<>), typeof(EntityRepository<>));

            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            using var scope = services.BuildServiceProvider().CreateScope();
            var runner = scope.ServiceProvider.GetRequiredService<IMigrationManager>();
            foreach (var assembly in mAssemblies)
                runner.ApplyUpMigrations(assembly, MigrationProcessType.NoDependencies);
        }

        public void ConfigureApplication(IApplicationBuilder engineBuilder)
        {
            var config = Singleton<NI2SSettings>.Instance.Get<CacheConfig>();

            LinqToDB.Common.Configuration.Linq.DisableQueryCache = config.LinqDisableQueryCache;
        }
    }
}
