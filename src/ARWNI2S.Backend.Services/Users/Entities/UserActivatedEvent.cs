namespace ARWNI2S.Backend.Services.Users.Entities
{
    /// <summary>
    /// User activated event
    /// </summary>
    public partial class UserActivatedEvent
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="user">user</param>
        public UserActivatedEvent(User user)
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