﻿using ARWNI2S.Infrastructure.Configuration;

namespace ARWNI2S.Portal.Services.Entities.Security
{
    /// <summary>
    /// Security settings
    /// </summary>
    public partial class PortalSecuritySettings : ISettings
    {
        /// <summary>
        /// Gets or sets an encryption key
        /// </summary>
        public string EncryptionKey { get; set; }

        /// <summary>
        /// Gets or sets a list of admin area allowed IP addresses
        /// </summary>
        public List<string> AdminAreaAllowedIpAddresses { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether honeypot is enabled on the registration page
        /// </summary>
        public bool HoneypotEnabled { get; set; }

        /// <summary>
        /// Gets or sets a honeypot input name
        /// </summary>
        public string HoneypotInputName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to allow non-ASCII characters in headers
        /// </summary>
        public bool AllowNonAsciiCharactersInHeaders { get; set; }
    }
}