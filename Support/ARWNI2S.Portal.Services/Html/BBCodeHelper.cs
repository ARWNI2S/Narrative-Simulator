using ARWNI2S.Node.Core.Common;
using ARWNI2S.Portal.Services.Html.CodeFormatter;
using System.Text.RegularExpressions;

namespace ARWNI2S.Portal.Services.Html
{
    /// <summary>
    /// Represents a BBCode helper
    /// </summary>
    public partial class BBCodeHelper : IBBCodeHelper
    {
        #region Fields

        private readonly CommonSettings _commonSettings;

        private static readonly Regex _regexBold = RegexBold();
        private static readonly Regex _regexItalic = RegexItalic();
        private static readonly Regex _regexUnderLine = RegexUnderline();
        private static readonly Regex _regexUrl1 = RegexUrl1();
        private static readonly Regex _regexUrl2 = RegexUrl2();
        private static readonly Regex _regexQuote = RegexQuote();
        private static readonly Regex _regexImg = RegexImg();

        #endregion

        #region Ctor

        public BBCodeHelper(CommonSettings commonSettings)
        {
            _commonSettings = commonSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Formats the text
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="replaceBold">A value indicating whether to replace Bold</param>
        /// <param name="replaceItalic">A value indicating whether to replace Italic</param>
        /// <param name="replaceUnderline">A value indicating whether to replace Underline</param>
        /// <param name="replaceUrl">A value indicating whether to replace URL</param>
        /// <param name="replaceCode">A value indicating whether to replace Code</param>
        /// <param name="replaceQuote">A value indicating whether to replace Quote</param>
        /// <param name="replaceImg">A value indicating whether to replace Img</param>
        /// <returns>Formatted text</returns>
        public virtual string FormatText(string text, bool replaceBold, bool replaceItalic,
            bool replaceUnderline, bool replaceUrl, bool replaceCode, bool replaceQuote, bool replaceImg)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            if (replaceBold)
                // format the bold tags: [b][/b] becomes: <strong></strong>
                text = _regexBold.Replace(text, "<strong>$1</strong>");

            if (replaceItalic)
                // format the italic tags: [i][/i] becomes: <em></em>
                text = _regexItalic.Replace(text, "<em>$1</em>");

            if (replaceUnderline)
                // format the underline tags: [u][/u] becomes: <u></u>
                text = _regexUnderLine.Replace(text, "<u>$1</u>");

            if (replaceUrl)
            {
                var newWindow = _commonSettings.BbcodeEditorOpenLinksInNewWindow;
                // format the URL tags: [url=https://www.dragoncorp.org]my site[/url]
                // becomes: <a href="https://www.dragoncorp.org">my site</a>
                text = _regexUrl1.Replace(text, $"<a href=\"$1\" rel=\"nofollow\"{(newWindow ? " target=_blank" : "")}>$2</a>");

                // format the URL tags: [url]https://www.dragoncorp.org[/url]
                // becomes: <a href="https://www.dragoncorp.org">https://www.dragoncorp.org</a>
                text = _regexUrl2.Replace(text, $"<a href=\"$1\" rel=\"nofollow\"{(newWindow ? " target=_blank" : "")}>$1</a>");
            }

            if (replaceQuote)
                while (_regexQuote.IsMatch(text))
                    text = _regexQuote.Replace(text, "<b>$1 wrote:</b><div class=\"quote\">$2</div>");

            if (replaceCode)
                text = CodeFormatHelper.FormatTextSimple(text);

            if (replaceImg)
                // format the img tags: [img]https://www.dragoncorp.org/Content/Images/Image.jpg[/img]
                // becomes: <img src="https://www.dragoncorp.org/Content/Images/Image.jpg">
                text = _regexImg.Replace(text, "<img src=\"$1\" class=\"user-posted-image\" alt=\"\">");

            return text;
        }

        /// <summary>
        /// Removes all quotes from string
        /// </summary>
        /// <param name="str">Source string</param>
        /// <returns>string</returns>
        public virtual string RemoveQuotes(string str)
        {
            str = RegexQuote1().Replace(str, string.Empty);
            str = RegexQuote2().Replace(str, string.Empty);
            return str;
        }

        [GeneratedRegex("\\[b\\](.+?)\\[/b\\]", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
        private static partial Regex RegexBold();
        [GeneratedRegex("\\[i\\](.+?)\\[/i\\]", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
        private static partial Regex RegexItalic();
        [GeneratedRegex("\\[u\\](.+?)\\[/u\\]", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
        private static partial Regex RegexUnderline();
        [GeneratedRegex("\\[url\\=(https?:.+?)\\]([^\\]]+)\\[/url\\]", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
        private static partial Regex RegexUrl1();
        [GeneratedRegex("\\[url\\](https?:.+?)\\[/url\\]", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
        private static partial Regex RegexUrl2();
        [GeneratedRegex("\\[quote=(.+?)\\](.+?)\\[/quote\\]", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
        private static partial Regex RegexQuote();
        [GeneratedRegex("\\[img\\](.+?)\\[/img\\]", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
        private static partial Regex RegexImg();
        [GeneratedRegex("\\[quote=(.+?)\\]", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
        private static partial Regex RegexQuote1();
        [GeneratedRegex("\\[/quote\\]", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
        private static partial Regex RegexQuote2();

        #endregion
    }
}