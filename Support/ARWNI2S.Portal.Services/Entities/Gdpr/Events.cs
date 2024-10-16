namespace ARWNI2S.Portal.Services.Entities.Gdpr
{
    /// <summary>
    /// User permanently deleted (GDPR)
    /// </summary>
    public partial class UserPermanentlyDeleted
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <param name="email">Email</param>
        public UserPermanentlyDeleted(int userId, string email)
        {
            UserId = userId;
            Email = email;
        }

        /// <summary>
        /// User identifier
        /// </summary>
        public int UserId { get; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; }
    }
}