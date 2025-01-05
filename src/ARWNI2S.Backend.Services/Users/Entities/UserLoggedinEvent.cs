namespace ARWNI2S.Backend.Services.Users.Entities
{
    /// <summary>
    /// User logged-in event
    /// </summary>
    public partial class UserLoggedinEvent
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="user">User</param>
        public UserLoggedinEvent(User user)
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