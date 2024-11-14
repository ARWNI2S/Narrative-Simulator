namespace ARWNI2S.Node.Configuration.Options
{
    /// <summary>
    /// Provides programmatic configuration for the MVRM framework.
    /// </summary>
    public class NiisOptions
    {
        /// <summary>
        /// Creates a new instance of <see cref="NiisOptions"/>.
        /// </summary>
        public NiisOptions()
        {
        }

        public int MaxIAsyncEnumerableBufferLimit { get; set; } = 8192;
    }
}
