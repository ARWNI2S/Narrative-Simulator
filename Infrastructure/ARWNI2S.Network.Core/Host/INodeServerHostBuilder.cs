using ARWNI2S.Engine.Network.Middleware;
using ARWNI2S.Engine.Network.Session;
using ARWNI2S.Infrastructure.Network.Protocol;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ARWNI2S.Engine.Network.Host
{
    public interface INodeServerHostBuilder : IHostBuilder, IMinimalApiHostBuilder
    {
        INodeServerHostBuilder ConfigureSupplementServices(Action<HostBuilderContext, IServiceCollection> configureDelegate);
    }

    public interface INodeServerHostBuilder<TReceivePackage> : INodeServerHostBuilder
    {
        INodeServerHostBuilder<TReceivePackage> ConfigureServerOptions(Func<HostBuilderContext, IConfiguration, IConfiguration> serverOptionsReader);

        new INodeServerHostBuilder<TReceivePackage> ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate);

        new INodeServerHostBuilder<TReceivePackage> ConfigureSupplementServices(Action<HostBuilderContext, IServiceCollection> configureDelegate);

        INodeServerHostBuilder<TReceivePackage> UseMiddleware<TMiddleware>()
            where TMiddleware : class, IMiddleware;

        INodeServerHostBuilder<TReceivePackage> UsePipelineFilter<TPipelineFilter>()
            where TPipelineFilter : IPipelineFilter<TReceivePackage>, new();

        INodeServerHostBuilder<TReceivePackage> UsePipelineFilterFactory<TPipelineFilterFactory>()
            where TPipelineFilterFactory : class, IPipelineFilterFactory<TReceivePackage>;

        INodeServerHostBuilder<TReceivePackage> UseHostedService<THostedService>()
            where THostedService : class, IHostedService;

        INodeServerHostBuilder<TReceivePackage> UsePackageDecoder<TPackageDecoder>()
            where TPackageDecoder : class, IPackageDecoder<TReceivePackage>;

        INodeServerHostBuilder<TReceivePackage> UsePackageEncoder<TPackageEncoder>()
            where TPackageEncoder : class, IPackageEncoder<TReceivePackage>;

        INodeServerHostBuilder<TReceivePackage> UsePackageHandlingScheduler<TPackageHandlingScheduler>()
            where TPackageHandlingScheduler : class, IPackageHandlingScheduler<TReceivePackage>;

        INodeServerHostBuilder<TReceivePackage> UseSessionFactory<TSessionFactory>()
            where TSessionFactory : class, ISessionFactory;

        INodeServerHostBuilder<TReceivePackage> UseSession<TSession>()
            where TSession : INodeSession;

        INodeServerHostBuilder<TReceivePackage> UsePackageHandlingContextAccessor();
    }
}