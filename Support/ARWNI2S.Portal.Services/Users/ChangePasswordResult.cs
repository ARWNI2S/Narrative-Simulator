namespace ARWNI2S.Portal.Services.Users
{
    /// <summary>
    /// Change password result
    /// </summary>
    public partial class ChangePasswordResult
    {
        public ChangePasswordResult()
        {
            Errors = [];
        }

        /// <summary>
        /// Gets a value indicating whether request has been completed successfully
        /// </summary>
        public bool Success => !Errors.Any();

        /// <summary>
        /// Add error
        /// </summary>
        /// <param name="error">Error</param>
        public void AddError(string error)
        {
            Errors.Add(error);
        }

        /// <summary>
        /// Errors
        /// </summary>
        public IList<string> Errors { get; set; }
    }
}