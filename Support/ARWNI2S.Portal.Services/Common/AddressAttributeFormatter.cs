﻿using ARWNI2S.Node.Services.Localization;
using ARWNI2S.Portal.Services.Entities.Common;
using ARWNI2S.Portal.Services.Html;
using System.Net;
using System.Text;

namespace ARWNI2S.Portal.Services.Common
{
    /// <summary>
    /// Address attribute helper
    /// </summary>
    public partial class AddressAttributeFormatter : IAddressAttributeFormatter
    {
        #region Fields

        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly IAddressAttributeService _addressAttributeService;
        private readonly IHtmlFormatter _htmlFormatter;
        private readonly ILocalizationService _localizationService;
        private readonly PortalWorkContext _workContext;

        #endregion

        #region Ctor

        public AddressAttributeFormatter(IAddressAttributeParser addressAttributeParser,
            IAddressAttributeService addressAttributeService,
            IHtmlFormatter htmlFormatter,
            ILocalizationService localizationService,
            PortalWorkContext workContext)
        {
            _addressAttributeParser = addressAttributeParser;
            _addressAttributeService = addressAttributeService;
            _htmlFormatter = htmlFormatter;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Formats attributes
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="separator">Separator</param>
        /// <param name="htmlEncode">A value indicating whether to encode (HTML) values</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the attributes
        /// </returns>
        public virtual async Task<string> FormatAttributesAsync(string attributesXml,
            string separator = "<br />",
            bool htmlEncode = true)
        {
            var result = new StringBuilder();
            var currentLanguage = await _workContext.GetWorkingLanguageAsync();
            var attributes = await _addressAttributeParser.ParseAddressAttributesAsync(attributesXml);
            for (var i = 0; i < attributes.Count; i++)
            {
                var attribute = attributes[i];
                var valuesStr = _addressAttributeParser.ParseValues(attributesXml, attribute.Id);
                for (var j = 0; j < valuesStr.Count; j++)
                {
                    var valueStr = valuesStr[j];
                    var formattedAttribute = string.Empty;
                    if (!attribute.ShouldHaveValues())
                    {
                        //no values
                        if (attribute.AttributeControlType == AttributeControlType.MultilineTextbox)
                        {
                            //multiline textbox
                            var attributeName = await _localizationService.GetLocalizedAsync(attribute, a => a.Name, currentLanguage.Id);
                            //encode (if required)
                            if (htmlEncode)
                                attributeName = WebUtility.HtmlEncode(attributeName);
                            formattedAttribute = $"{attributeName}: {_htmlFormatter.FormatText(valueStr, false, true, false, false, false, false)}";
                            //we never encode multiline textbox input
                        }
                        else if (attribute.AttributeControlType == AttributeControlType.FileUpload)
                        {
                            //file upload
                            //not supported for address attributes
                        }
                        else
                        {
                            //other attributes (textbox, datepicker)
                            formattedAttribute = $"{await _localizationService.GetLocalizedAsync(attribute, a => a.Name, currentLanguage.Id)}: {valueStr}";
                            //encode (if required)
                            if (htmlEncode)
                                formattedAttribute = WebUtility.HtmlEncode(formattedAttribute);
                        }
                    }
                    else
                    {
                        if (int.TryParse(valueStr, out var attributeValueId))
                        {
                            var attributeValue = await _addressAttributeService.GetAddressAttributeValueByIdAsync(attributeValueId);
                            if (attributeValue != null)
                            {
                                formattedAttribute = $"{await _localizationService.GetLocalizedAsync(attribute, a => a.Name, currentLanguage.Id)}: {await _localizationService.GetLocalizedAsync(attributeValue, a => a.Name, currentLanguage.Id)}";
                            }
                            //encode (if required)
                            if (htmlEncode)
                                formattedAttribute = WebUtility.HtmlEncode(formattedAttribute);
                        }
                    }

                    if (string.IsNullOrEmpty(formattedAttribute))
                        continue;

                    if (i != 0 || j != 0)
                        result.Append(separator);

                    result.Append(formattedAttribute);
                }
            }

            return result.ToString();
        }

        #endregion
    }
}