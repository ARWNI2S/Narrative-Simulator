﻿using ARWNI2S.Infrastructure.Configuration;
using ARWNI2S.Node.Core.Security.Secrets;

namespace ARWNI2S.Portal.Services.Entities.Security
{
    /// <summary>
    /// CAPTCHA settings
    /// </summary>
    public partial class CaptchaSettings : ISettings
    {
        /// <summary>
        /// Is CAPTCHA enabled?
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Type of reCAPTCHA
        /// </summary>
        public CaptchaType CaptchaType { get; set; }

        /// <summary>
        /// A value indicating whether CAPTCHA should be displayed on the login page
        /// </summary>
        public bool ShowOnLoginPage { get; set; }

        /// <summary>
        /// A value indicating whether CAPTCHA should be displayed on the registration page
        /// </summary>
        public bool ShowOnRegistrationPage { get; set; }

        /// <summary>
        /// A value indicating whether CAPTCHA should be displayed on the contacts page
        /// </summary>
        public bool ShowOnContactUsPage { get; set; }

        /// <summary>
        /// A value indicating whether CAPTCHA should be displayed on the "comment blog" page
        /// </summary>
        public bool ShowOnBlogCommentPage { get; set; }

        /// <summary>
        /// A value indicating whether CAPTCHA should be displayed on the "comment news" page
        /// </summary>
        public bool ShowOnNewsCommentPage { get; set; }

        /// <summary>
        /// A value indicating whether CAPTCHA should be displayed on the "Apply for partner account" page
        /// </summary>
        public bool ShowOnApplyPartnerPage { get; set; }

        /// <summary>
        /// A value indicating whether CAPTCHA should be displayed on the "forgot password" page
        /// </summary>
        public bool ShowOnForgotPasswordPage { get; set; }

        /// <summary>
        /// The base reCAPTCHA API URL
        /// </summary>
        public string ReCaptchaApiUrl { get; set; }
        /// <summary>
        /// reCAPTCHA public key
        /// </summary>
        [Secret]
        public string ReCaptchaPublicKey { get; set; }

        /// <summary>
        /// reCAPTCHA private key
        /// </summary>
        [Secret]
        public string ReCaptchaPrivateKey { get; set; }

        /// <summary>
        /// reCAPTCHA V3 score threshold
        /// </summary>
        public decimal ReCaptchaV3ScoreThreshold { get; set; }

        /// <summary>
        /// reCAPTCHA theme
        /// </summary>
        public string ReCaptchaTheme { get; set; }

        /// <summary>
        /// The length of time, in seconds, before the reCAPTCHA request times out
        /// </summary>
        public int? ReCaptchaRequestTimeout { get; set; }
        /// <summary>
        /// reCAPTCHA default language
        /// </summary>
        public string ReCaptchaDefaultLanguage { get; set; }

        /// <summary>
        /// A value indicating whether reCAPTCHA language should be set automatically
        /// </summary>
        public bool AutomaticallyChooseLanguage { get; set; }
    }
}