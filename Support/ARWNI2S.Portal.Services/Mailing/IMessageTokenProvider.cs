using ARWNI2S.Node.Core.Entities.Clustering;
using ARWNI2S.Node.Data.Entities.Users;
using ARWNI2S.Portal.Services.Entities.Blogs;
using ARWNI2S.Portal.Services.Entities.Mailing;
using ARWNI2S.Portal.Services.Entities.News;

namespace ARWNI2S.Portal.Services.Mailing
{
    /// <summary>
    /// Message token provider
    /// </summary>
    public partial interface IMessageTokenProvider
    {
        /// <summary>
        /// Add node tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="node">Node</param>
        /// <param name="emailAccount">Email account</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task AddNodeTokensAsync(IList<Token> tokens, INI2SNode node, EmailAccount emailAccount);

        /// <summary>
        /// Add user tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="userId">User identifier</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task AddUserTokensAsync(IList<Token> tokens, int userId);

        /// <summary>
        /// Add user tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="user">User</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task AddUserTokensAsync(IList<Token> tokens, User user);

        ///// <summary>
        ///// Add partner tokens
        ///// </summary>
        ///// <param name="tokens">List of already added tokens</param>
        ///// <param name="partner">Partner</param>
        ///// <returns>A task that represents the asynchronous operation</returns>
        //Task AddPartnerTokensAsync(IList<Token> tokens, Partner partner);

        /// <summary>
        /// Add newsletter subscription tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="subscription">Newsletter subscription</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task AddNewsLetterSubscriptionTokensAsync(IList<Token> tokens, NewsLetterSubscription subscription);

        /// <summary>
        /// Add blog comment tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="blogComment">Blog post comment</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task AddBlogCommentTokensAsync(IList<Token> tokens, BlogComment blogComment);

        /// <summary>
        /// Add news comment tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="newsComment">News comment</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task AddNewsCommentTokensAsync(IList<Token> tokens, NewsComment newsComment);

        /// <summary>
        /// Add system message tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="systemMessage">System message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        //Task AddSystemMessageTokensAsync(IList<Token> tokens, SystemMessage systemMessage);

        /// <summary>
        /// Get collection of allowed (supported) message tokens for campaigns
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the collection of allowed (supported) message tokens for campaigns
        /// </returns>
        Task<IEnumerable<string>> GetListOfCampaignAllowedTokensAsync();

        /// <summary>
        /// Get collection of allowed (supported) message tokens
        /// </summary>
        /// <param name="tokenGroups">Collection of token groups; pass null to get all available tokens</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the collection of allowed message tokens
        /// </returns>
        Task<IEnumerable<string>> GetListOfAllowedTokensAsync(IEnumerable<string> tokenGroups = null);

        /// <summary>
        /// Get token groups of message template
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <returns>Collection of token group names</returns>
        IEnumerable<string> GetTokenGroups(MessageTemplate messageTemplate);
    }
}