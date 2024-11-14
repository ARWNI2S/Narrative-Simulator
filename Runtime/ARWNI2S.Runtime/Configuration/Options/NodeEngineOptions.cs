using ARWNI2S.Node.Hosting;

namespace ARWNI2S.Node.Configuration.Options
{
    /// <summary>
    /// Options for configuring the behavior for <see cref="NodeEngineHost.CreateBuilder(NodeEngineOptions)"/>.
    /// </summary>
    public class NodeEngineOptions
    {
        /// <summary>
        /// The command line arguments.
        /// </summary>
        public string[] Args { get; init; }

        /// <summary>
        /// The environment name.
        /// </summary>
        public string EnvironmentName { get; init; }

        /// <summary>
        /// The engine name.
        /// </summary>
        public string EngineName { get; init; }

        /// <summary>
        /// The content root path.
        /// </summary>
        public string ContentRootPath { get; init; }

        /// <summary>
        /// The node root path.
        /// </summary>
        public string NodeRootPath { get; init; }
    }
}
