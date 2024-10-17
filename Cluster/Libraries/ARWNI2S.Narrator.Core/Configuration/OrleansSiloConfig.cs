using ARWNI2S.Infrastructure.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace ARWNI2S.Node.Core.Configuration
{
    /// <summary>
    /// Represents simulation clustering type enumeration
    /// </summary>
    public enum SimulationClusteringType
    {
        [EnumMember(Value = "localhost")]
        Localhost,
        [EnumMember(Value = "azure")]
        AzureStorage,
        [EnumMember(Value = "sqlserver")]
        SqlServer,
        [EnumMember(Value = "redis")]
        Redis
    }

    /// <summary>
    /// Represents distributed cache configuration parameters
    /// </summary>

    public partial class OrleansSiloConfig : IConfig
    {
        /// <summary>
        /// Gets or sets a distributed cache type
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public SimulationClusteringType SimulationClusteringType { get; private set; } = SimulationClusteringType.Localhost;

    }
}
