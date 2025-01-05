namespace ARWNI2S.Backend.Services.Users
{
    /// <summary>
    /// User multi-factor authentication info
    /// </summary>
    public partial class UserMultiFactorAuthenticationInfo
    {
        public UserMultiFactorAuthenticationInfo()
        {
            CustomValues = new Dictionary<string, object>();
        }
        public string UserName { get; set; }

        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }

        /// <summary>
        /// You can store any custom value in this property
        /// </summary>
        public Dictionary<string, object> CustomValues { get; set; }
    }
}