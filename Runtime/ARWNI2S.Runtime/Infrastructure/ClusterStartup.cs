using ARWNI2S.Infrastructure;
using ARWNI2S.Runtime.Hosting.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ARWNI2S.Runtime.Infrastructure
{
    public class ClusterStartup : INI2SStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //services.AddNI2SNodeServerServices();

            services.AddClusteringServices();

            services.AddNI2SRuntimeServices();
        }

        public void Configure(IHost application)
        {
            application.UseClustering();
        }

        public int Order => 100;     // clustering services should be loaded after error handlers
    }
}
