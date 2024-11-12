namespace ARWNI2S.Engine.Options
{
    /// <summary>
    /// Provides programmatic configuration for the MVRM framework.
    /// </summary>
    public class EngineOptions
    {
        /// <summary>
        /// Creates a new instance of <see cref="EngineOptions"/>.
        /// </summary>
        public EngineOptions()
        {
        }

        public int MaxIAsyncEnumerableBufferLimit { get; set; } = 8192;
    }
}
