namespace ARWNI2S.Portal.Services.Entities.Mailing
{
    /// <summary>
    /// Represents message template system names
    /// </summary>
    public static partial class MessageTemplateSystemNames
    {
        #region User

        /// <summary>
        /// Represents system name of notification about new registration
        /// </summary>
        public const string UserRegisteredNotification = "NewUser.Notification";

        /// <summary>
        /// Represents system name of user welcome message
        /// </summary>
        public const string UserWelcomeMessage = "User.WelcomeMessage";

        /// <summary>
        /// Represents system name of email validation message
        /// </summary>
        public const string UserEmailValidationMessage = "User.EmailValidationMessage";

        /// <summary>
        /// Represents system name of email revalidation message
        /// </summary>
        public const string UserEmailRevalidationMessage = "User.EmailRevalidationMessage";

        /// <summary>
        /// Represents system name of password recovery message
        /// </summary>
        public const string UserPasswordRecoveryMessage = "User.PasswordRecovery";

        #endregion

        #region Newsletter

        /// <summary>
        /// Represents system name of subscription activation message
        /// </summary>
        public const string NewsletterSubscriptionActivationMessage = "NewsLetterSubscription.ActivationMessage";

        /// <summary>
        /// Represents system name of subscription deactivation message
        /// </summary>
        public const string NewsletterSubscriptionDeactivationMessage = "NewsLetterSubscription.DeactivationMessage";

        #endregion

        #region To friend

        /// <summary>
        /// Represents system name of 'Email a friend' message
        /// </summary>
        public const string EmailAFriendMessage = "Service.EmailAFriend";

        #endregion

        #region Misc

        /// <summary>
        /// Represents system name of notification administrator about applying new partner account
        /// </summary>
        public const string NewPartnerAccountApplyNotification = "PartnerAccountApply.Notification";

        /// <summary>
        /// Represents system name of notification partner about changing information
        /// </summary>
        public const string PartnerInformationChangeNotification = "PartnerInformationChange.Notification";

        /// <summary>
        /// Represents system name of notification administrator about submitting new VAT
        /// </summary>
        public const string NewVatSubmittedNotification = "NewVATSubmitted.Notification";

        /// <summary>
        /// Represents system name of notification administrator about new blog comment
        /// </summary>
        public const string BlogCommentNotification = "Blog.BlogComment";

        /// <summary>
        /// Represents system name of notification administrator about new news comment
        /// </summary>
        public const string NewsCommentNotification = "News.NewsComment";

        /// <summary>
        /// Represents system name of 'Contact us' message
        /// </summary>
        public const string ContactUsMessage = "Service.ContactUs";

        /// <summary>
        /// Represents system name of 'Contact partner' message
        /// </summary>
        public const string ContactPartnerMessage = "Service.ContactPartner";

        #endregion

        /// <summary>
        /// Represents system name of notification about new system message
        /// </summary>
        public const string SystemMessageNotification = "Player.NewMessage";

    }
}