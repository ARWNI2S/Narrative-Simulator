using ARWNI2S.Infrastructure.Network.Protocol;

namespace ARWNI2S.Engine.Network.Protocol
{
    public class StringPackageInfo : IKeyedPackageInfo<string>, IStringPackage
    {
        public string Key { get; set; }

        public string Body { get; set; }

        public string[] Parameters { get; set; }
    }
}