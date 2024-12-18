using ARWNI2S.Configuration;
using ARWNI2S.Infrastructure;

namespace ARWNI2S.Narrator.Framework.Orleans
{
    public class NI2SSiloBuilderConfigurator : ISiloConfigurator
    {
        public void Configure(ISiloBuilder siloBuilder)
        {
            var niisSettings = Singleton<NI2SSettings>.Instance;


        }
    }
}
