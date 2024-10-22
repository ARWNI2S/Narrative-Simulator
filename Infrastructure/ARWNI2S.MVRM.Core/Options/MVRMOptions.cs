namespace ARWNI2S.Engine.Options
{
    /// <summary>
    /// Provides programmatic configuration for the MVRM framework.
    /// </summary>
    public class MVRMOptions
    {
        /// <summary>
        /// Creates a new instance of <see cref="MVRMOptions"/>.
        /// </summary>
        public MVRMOptions()
        {
        }

        public int MaxIAsyncEnumerableBufferLimit { get; set; } = 8192;
    }
}
