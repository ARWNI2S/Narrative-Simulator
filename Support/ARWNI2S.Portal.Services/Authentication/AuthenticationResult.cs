namespace ARWNI2S.Portal.Services.Authentication
{
    public sealed class AuthenticationResult
    {
        public bool Success { get; set; }
        public string ReturnUrl { get; set; }
        public string Error { get; set; }
    }
}
