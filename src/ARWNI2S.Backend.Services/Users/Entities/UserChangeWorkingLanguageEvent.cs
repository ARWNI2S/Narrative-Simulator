namespace ARWNI2S.Backend.Services.Users.Entities
{
    /// <summary>
    /// User change working language event
    /// </summary>
    public partial class UserChangeWorkingLanguageEvent
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="user">User</param>
        public UserChangeWorkingLanguageEvent(User user)
        {
            User = user;
        }

        /// <summary>
        /// User
        /// </summary>
        public User User
        {
            get;
        }
    }
}
