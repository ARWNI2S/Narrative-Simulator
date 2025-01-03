namespace ARWNI2S.Framework.Users.Entities
{
    /// <summary>
    /// "User is change multi-factor authentication provider" event
    /// </summary>
    public partial class UserChangeMultiFactorAuthenticationProviderEvent
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="user">User</param>
        public UserChangeMultiFactorAuthenticationProviderEvent(User user)
        {
            User = user;
        }

        /// <summary>
        /// Get or set the user
        /// </summary>
        public User User { get; }
    }
}