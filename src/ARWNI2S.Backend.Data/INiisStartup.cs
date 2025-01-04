using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ARWNI2S.Backend.Data
{
    public interface INiisStartup
    {
        int Order { get; }

        void ConfigureServices(IServiceCollection services, IConfiguration configuration);

        void ConfigureApplication(IApplicationBuilder applicationBuilder);
    }
}