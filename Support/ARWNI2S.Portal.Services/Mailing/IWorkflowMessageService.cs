using ARWNI2S.Node.Data.Entities.Users;
using ARWNI2S.Portal.Services.Entities.Blogs;
using ARWNI2S.Portal.Services.Entities.Mailing;
using ARWNI2S.Portal.Services.Entities.News;

namespace ARWNI2S.Portal.Services.Mailing
{
    /// <summary>
    /// Workflow message service
    /// </summary>
    public partial interface IWorkflowMessageService
    {
        #region User workflow

        /// <summary>
        /// Sends 'New user' notification message to an administrator
        /// </summary>
        /// <param name="user">User instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the queued email identifier
        /// </returns>
        Task<IList<int>> SendUserRegisteredNotificationMessageAsync(User user, int languageId);

        /// <summary>
        /// Sends a welcome message to a user
        /// </summary>
        /// <param name="user">User instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the queued email identifier
        /// </returns>
        Task<IList<int>> SendUserWelcomeMessageAsync(User user, int languageId);

        /// <summary>
        /// Sends an email validation message to a user
        /// </summary>
        /// <param name="user">User instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the queued email identifier
        /// </returns>
        Task<IList<int>> SendUserEmailValidationMessageAsync(User user, int languageId);

        /// <summary>
        /// Sends an email re-validation message to a user
        /// </summary>
        /// <param name="user">User instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the queued email identifier
        /// </returns>
        Task<IList<int>> SendUserEmailRevalidationMessageAsync(User user, int languageId);

        /// <summary>
        /// Sends password recovery message to a user
        /// </summary>
        /// <param name="user">User instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the queued email identifier
        /// </returns>
        Task<IList<int>> SendUserPasswordRecoveryMessageAsync(User user, int languageId);

        #endregion

        #region Newsletter workflow

        /// <summary>
        /// Sends a newsletter subscription activation message
        /// </summary>
        /// <param name="subscription">Newsletter subscription</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the queued email identifier
        /// </returns>
        Task<IList<int>> SendNewsLetterSubscriptionActivationMessageAsync(NewsLetterSubscription subscription, int languageId);

        /// <summary>
        /// Sends a newsletter subscription deactivation message
        /// </summary>
        /// <param name="subscription">Newsletter subscription</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the queued email identifier
        /// </returns>
        Task<IList<int>> SendNewsLetterSubscriptionDeactivationMessageAsync(NewsLetterSubscription subscription, int languageId);

        #endregion

        #region Send a message to a friend

        ///// <summary>
        ///// Sends "email a friend" message
        ///// </summary>
        ///// <param name="user">User instance</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <param name="product">Product instance</param>
        ///// <param name="userEmail">User's email</param>
        ///// <param name="friendsEmail">Friend's email</param>
        ///// <param name="personalMessage">Personal message</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the queued email identifier
        ///// </returns>
        //Task<IList<int>> SendProductEmailAFriendMessageAsync(User user, int languageId,
        //    Product product, string userEmail, string friendsEmail, string personalMessage);

        ///// <summary>
        ///// Sends wishlist "email a friend" message
        ///// </summary>
        ///// <param name="user">User</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <param name="userEmail">User's email</param>
        ///// <param name="friendsEmail">Friend's email</param>
        ///// <param name="personalMessage">Personal message</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the queued email identifier
        ///// </returns>
        //Task<IList<int>> SendWishlistEmailAFriendMessageAsync(User user, int languageId,
        //    string userEmail, string friendsEmail, string personalMessage);

        #endregion

        //#region Message Notifications

        ///// <summary>
        ///// Sends a system message notification
        ///// </summary>
        ///// <param name="systemMessage">System message</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the queued email identifier
        ///// </returns>
        //Task<IList<int>> SendSystemMessageNotificationAsync(SystemMessage systemMessage, int languageId);

        //#endregion

        #region Misc

        /// <summary>
        /// Sends a "new VAT submitted" notification to an administrator
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="vatName">Received VAT name</param>
        /// <param name="vatAddress">Received VAT address</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the queued email identifier
        /// </returns>
        Task<IList<int>> SendNewVatSubmittedNotificationAsync(User user, string vatName, string vatAddress, int languageId);

        /// <summary>
        /// Sends a blog comment notification message to an administrator
        /// </summary>
        /// <param name="blogComment">Blog comment</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the queued email identifier
        /// </returns>
        Task<IList<int>> SendBlogCommentNotificationMessageAsync(BlogComment blogComment, int languageId);

        /// <summary>
        /// Sends a news comment notification message to an administrator
        /// </summary>
        /// <param name="newsComment">News comment</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the queued email identifier
        /// </returns>
        Task<IList<int>> SendNewsCommentNotificationMessageAsync(NewsComment newsComment, int languageId);

        ///// <summary>
        ///// Sends a 'Back in stock' notification message to a user
        ///// </summary>
        ///// <param name="subscription">Subscription</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the queued email identifier
        ///// </returns>
        //Task<IList<int>> SendBackInStockNotificationAsync(BackInStockSubscription subscription, int languageId);

        /// <summary>
        /// Sends "contact us" message
        /// </summary>
        /// <param name="languageId">Message language identifier</param>
        /// <param name="senderEmail">Sender email</param>
        /// <param name="senderName">Sender name</param>
        /// <param name="subject">Email subject. Pass null if you want a message template subject to be used.</param>
        /// <param name="body">Email body</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the queued email identifier
        /// </returns>
        Task<IList<int>> SendContactUsMessageAsync(int languageId, string senderEmail, string senderName, string subject, string body);

        /// <summary>
        /// Sends a test email
        /// </summary>
        /// <param name="messageTemplateId">Message template identifier</param>
        /// <param name="sendToEmail">Send to email</param>
        /// <param name="tokens">Tokens</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the queued email identifier
        /// </returns>
        Task<int> SendTestEmailAsync(int messageTemplateId, string sendToEmail, List<Token> tokens, int languageId);

        #endregion

        #region Common

        /// <summary>
        /// Send notification
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="emailAccount">Email account</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="tokens">Tokens</param>
        /// <param name="toEmailAddress">Recipient email address</param>
        /// <param name="toName">Recipient name</param>
        /// <param name="attachmentFilePath">Attachment file path</param>
        /// <param name="attachmentFileName">Attachment file name</param>
        /// <param name="replyToEmailAddress">"Reply to" email</param>
        /// <param name="replyToName">"Reply to" name</param>
        /// <param name="fromEmail">Sender email. If specified, then it overrides passed "emailAccount" details</param>
        /// <param name="fromName">Sender name. If specified, then it overrides passed "emailAccount" details</param>
        /// <param name="subject">Subject. If specified, then it overrides subject of a message template</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the queued email identifier
        /// </returns>
        Task<int> SendNotificationAsync(MessageTemplate messageTemplate,
            EmailAccount emailAccount, int languageId, IList<Token> tokens,
            string toEmailAddress, string toName,
            string attachmentFilePath = null, string attachmentFileName = null,
            string replyToEmailAddress = null, string replyToName = null,
            string fromEmail = null, string fromName = null, string subject = null);

        #endregion
    }
}