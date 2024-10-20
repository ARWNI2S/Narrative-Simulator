﻿using ARWNI2S.Node.Core.Entities.Localization;
using ARWNI2S.Node.Core.Entities;
using ARWNI2S.Portal.Services.Entities.Common;

namespace ARWNI2S.Portal.Services.Entities.Users
{
    /// <summary>
    /// Represents a user attribute
    /// </summary>
    public partial class UserAttribute : BaseEntity, ILocalizedEntity
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the attribute is required
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Gets or sets the attribute control type identifier
        /// </summary>
        public int AttributeControlTypeId { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets the attribute control type
        /// </summary>
        public AttributeControlType AttributeControlType
        {
            get => (AttributeControlType)AttributeControlTypeId;
            set => AttributeControlTypeId = (int)value;
        }
    }
}
