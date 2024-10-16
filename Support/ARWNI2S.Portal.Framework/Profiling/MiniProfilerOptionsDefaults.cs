using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using StackExchange.Profiling;
using StackExchange.Profiling.Storage;

namespace ARWNI2S.Portal.Framework.Profiling
{
    /// <summary>
    /// Configures the default (important: with DI for IMemoryCache) before further user configuration.
    /// </summary>
    public class MiniProfilerOptionsDefaults : IConfigureOptions<MiniProfilerOptions>
    {
        private readonly IMemoryCache _cache;
        public MiniProfilerOptionsDefaults(IMemoryCache cache) => _cache = cache;

        public void Configure(MiniProfilerOptions options)
        {
            options.Storage ??= new MemoryCacheStorage(_cache, TimeSpan.FromMinutes(30));
        }
    }
}
