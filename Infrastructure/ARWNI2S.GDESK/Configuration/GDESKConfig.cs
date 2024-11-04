using ARWNI2S.Infrastructure.Configuration;

namespace ARWNI2S.Engine.Configuration
{
    public class GDESKConfig : IConfig
    {
        public int EventPoolMaxSize { get; set; } = 5000;
        public int EventPoolWarmupSize { get; set; } = 500;
    }
}
