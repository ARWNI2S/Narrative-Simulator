namespace ARWNI2S.Framework.Users.Entities
{
    /// <summary>
    /// Represents default values related to users data
    /// </summary>
    public static partial class UserDefaults
    {
        #region System user roles

        /// <summary>
        /// Gets a system name of 'administrators' user role
        /// </summary>
        public static string AdministratorsRoleName => "Administrators";

        /// <summary>
        /// Gets a system name of 'forum moderators' user role
        /// </summary>
        public static string ForumModeratorsRoleName => "ForumModerators";

        /// <summary>
        /// Gets a system name of 'registered' user role
        /// </summary>
        public static string RegisteredRoleName => "Registered";

        /// <summary>
        /// Gets a system name of 'guests' user role
        /// </summary>
        public static string GuestsRoleName => "Guests";

        /// <summary>
        /// Gets a system name of 'vendors' user role
        /// </summary>
        public static string VendorsRoleName => "Vendors";

        #endregion

        #region System users

        /// <summary>
        /// Gets a system name of 'search engine' user object
        /// </summary>
        public static string SearchEngineUserName => "SearchEngine";

        /// <summary>
        /// Gets a system name of 'background task' user object
        /// </summary>
        public static string BackgroundTaskUserName => "BackgroundTask";

        #endregion

        #region User attributes

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'DiscountCouponCode'
        /// </summary>
        public static string DiscountCouponCodeAttribute => "DiscountCouponCode";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'GiftCardCouponCodes'
        /// </summary>
        public static string GiftCardCouponCodesAttribute => "GiftCardCouponCodes";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'AvatarPictureId'
        /// </summary>
        public static string AvatarPictureIdAttribute => "AvatarPictureId";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'ForumPostCount'
        /// </summary>
        public static string ForumPostCountAttribute => "ForumPostCount";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'Signature'
        /// </summary>
        public static string SignatureAttribute => "Signature";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'PasswordRecoveryToken'
        /// </summary>
        public static string PasswordRecoveryTokenAttribute => "PasswordRecoveryToken";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'PasswordRecoveryTokenDateGenerated'
        /// </summary>
        public static string PasswordRecoveryTokenDateGeneratedAttribute => "PasswordRecoveryTokenDateGenerated";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'AccountActivationToken'
        /// </summary>
        public static string AccountActivationTokenAttribute => "AccountActivationToken";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'EmailRevalidationToken'
        /// </summary>
        public static string EmailRevalidationTokenAttribute => "EmailRevalidationToken";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'LastVisitedPage'
        /// </summary>
        public static string LastVisitedPageAttribute => "LastVisitedPage";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'ImpersonatedUserId'
        /// </summary>
        public static string ImpersonatedUserIdAttribute => "ImpersonatedUserId";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'AdminAreaStoreScopeConfiguration'
        /// </summary>
        public static string AdminAreaStoreScopeConfigurationAttribute => "AdminAreaStoreScopeConfiguration";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'SelectedPaymentMethod'
        /// </summary>
        public static string SelectedPaymentMethodAttribute => "SelectedPaymentMethod";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'SelectedShippingOption'
        /// </summary>
        public static string SelectedShippingOptionAttribute => "SelectedShippingOption";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'SelectedPickupPoint'
        /// </summary>
        public static string SelectedPickupPointAttribute => "SelectedPickupPoint";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'CheckoutAttributes'
        /// </summary>
        public static string CheckoutAttributes => "CheckoutAttributes";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'OfferedShippingOptions'
        /// </summary>
        public static string OfferedShippingOptionsAttribute => "OfferedShippingOptions";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'LastContinueShoppingPage'
        /// </summary>
        public static string LastContinueShoppingPageAttribute => "LastContinueShoppingPage";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'NotifiedAboutNewPrivateMessages'
        /// </summary>
        public static string NotifiedAboutNewPrivateMessagesAttribute => "NotifiedAboutNewPrivateMessages";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'WorkingThemeName'
        /// </summary>
        public static string WorkingThemeNameAttribute => "WorkingThemeName";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'UseRewardPointsDuringCheckout'
        /// </summary>
        public static string UseRewardPointsDuringCheckoutAttribute => "UseRewardPointsDuringCheckout";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'EuCookieLawAccepted'
        /// </summary>
        public static string EuCookieLawAcceptedAttribute => "EuCookieLaw.Accepted";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'SelectedMultiFactorAuthProvider'
        /// </summary>
        public static string SelectedMultiFactorAuthenticationProviderAttribute => "SelectedMultiFactorAuthProvider";

        /// <summary>
        /// Gets a name of session key
        /// </summary>
        public static string UserMultiFactorAuthenticationInfo => "UserMultiFactorAuthenticationInfo";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'HideConfigurationSteps'
        /// </summary>
        public static string HideConfigurationStepsAttribute => "HideConfigurationSteps";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'CloseConfigurationSteps'
        /// </summary>
        public static string CloseConfigurationStepsAttribute => "CloseConfigurationSteps";

        #endregion
    }
}