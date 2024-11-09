using ARWNI2S.Engine.Network.Session;

namespace ARWNI2S.Engine.Network.Command
{
    public struct CommandExecutingContext
    {
        /// <summary>
        /// Gets the session.
        /// </summary>
        public INodeSession Session { get; set; }

        /// <summary>
        /// Gets the request info.
        /// </summary>
        public object Package { get; set; }

        /// <summary>
        /// Gets the current command.
        /// </summary>
        public ICommand CurrentCommand { get; set; }

        /// <summary>
        /// Gets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        public Exception Exception { get; set; }

        public CancellationToken CancellationToken { get; set; }
    }
}
