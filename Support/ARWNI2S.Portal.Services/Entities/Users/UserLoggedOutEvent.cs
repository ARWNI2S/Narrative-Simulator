using ARWNI2S.Node.Core.Entities.Users;

namespace ARWNI2S.Portal.Services.Entities.Users
{
    /// <summary>
    /// "User is logged out" event
    /// </summary>
    public partial class UserLoggedOutEvent
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="user">User</param>
        public UserLoggedOutEvent(User user)
        {
            User = user;
        }

        /// <summary>
        /// Get or set the user
        /// </summary>
        public User User { get; }
    }
}