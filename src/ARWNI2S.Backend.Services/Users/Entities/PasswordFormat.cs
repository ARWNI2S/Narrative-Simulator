namespace ARWNI2S.Backend.Services.Users.Entities
{
    /// <summary>
    /// Password format
    /// </summary>
    public enum PasswordFormat
    {
        /// <summary>
        /// Clear
        /// </summary>
        Clear = 0,

        /// <summary>
        /// Hashed
        /// </summary>
        Hashed = 1,

        /// <summary>
        /// Encrypted
        /// </summary>
        Encrypted = 2
    }
}