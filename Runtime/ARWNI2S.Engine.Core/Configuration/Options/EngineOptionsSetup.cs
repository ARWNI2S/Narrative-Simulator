using Microsoft.Extensions.Options;

namespace ARWNI2S.Engine.Configuration.Options
{
    internal sealed class EngineOptionsSetup : IConfigureOptions<EngineOptions>
    {
        private readonly IServiceProvider _services;

        public EngineOptionsSetup(IServiceProvider services)
        {
            _services = services;
        }

        public void Configure(EngineOptions options)
        {
            options.EngineServices = _services;
        }
    }
}