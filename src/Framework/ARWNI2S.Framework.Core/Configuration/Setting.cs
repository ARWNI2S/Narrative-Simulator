using ARWNI2S.Engine.Data;

namespace ARWNI2S.Framework.Configuration
{
    /// <summary>
    /// Represents a setting
    /// </summary>
    public partial class Setting : DataEntity, ISetting
    {
        public Setting()
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="value">Value</param>
        /// <param name="nodeId">Node identifier</param>
        public Setting(string name, string value, int nodeId = 0)
        {
            Name = name;
            Value = value;
            NodeId = nodeId;
        }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the node for which this setting is valid. 0 is set when the setting is for all nodes
        /// </summary>
        public int NodeId { get; set; }

        /// <summary>
        /// To string
        /// </summary>
        /// <returns>Result</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}