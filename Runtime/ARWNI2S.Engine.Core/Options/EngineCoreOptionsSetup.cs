using Microsoft.Extensions.Options;

namespace ARWNI2S.Engine.Options
{
    internal sealed class EngineCoreOptionsSetup : IConfigureOptions<EngineCoreOptions>
    {
        private readonly IServiceProvider _services;

        public EngineCoreOptionsSetup(IServiceProvider services)
        {
            _services = services;
        }

        public void Configure(EngineCoreOptions options)
        {
            options.EngineServices = _services;
        }
    }
}