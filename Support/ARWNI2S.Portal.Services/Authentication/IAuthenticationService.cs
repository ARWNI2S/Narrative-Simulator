using ARWNI2S.Node.Core.Entities.Users;

namespace ARWNI2S.Portal.Services.Authentication
{
    /// <summary>
    /// Authentication service interface
    /// </summary>
    public partial interface IAuthenticationService
    {
        /// <summary>
        /// Sign in
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="isPersistent">Whether the authentication session is persisted across multiple requests</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task SignInAsync(User user, bool isPersistent);

        /// <summary>
        /// Sign out
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task SignOutAsync();

        /// <summary>
        /// Get authenticated user
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the user
        /// </returns>
        Task<User> GetAuthenticatedUserAsync();
    }
}