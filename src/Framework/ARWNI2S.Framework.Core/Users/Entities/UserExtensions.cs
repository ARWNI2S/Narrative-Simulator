namespace ARWNI2S.Framework.Users.Entities
{
    /// <summary>
    /// User extensions
    /// </summary>
    public static class UserExtensions
    {
        /// <summary>
        /// Gets a value indicating whether user a search engine
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>Result</returns>
        public static bool IsSearchEngineAccount(this User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            if (!user.IsSystemAccount || string.IsNullOrEmpty(user.SystemName))
                return false;

            var result = user.SystemName.Equals(UserDefaults.SearchEngineUserName, StringComparison.InvariantCultureIgnoreCase);

            return result;
        }

        /// <summary>
        /// Gets a value indicating whether the user is a built-in record for background tasks
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>Result</returns>
        public static bool IsBackgroundTaskAccount(this User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            if (!user.IsSystemAccount || string.IsNullOrEmpty(user.SystemName))
                return false;

            var result = user.SystemName.Equals(UserDefaults.BackgroundTaskUserName, StringComparison.InvariantCultureIgnoreCase);

            return result;
        }
    }
}