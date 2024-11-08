using ARWNI2S.Node.Core.Infrastructure;
using ARWNI2S.Portal.Services.Entities.Security;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;

namespace ARWNI2S.Portal.Framework.Security.Honeypot
{
    /// <summary>
    /// HTML extensions
    /// </summary>
    public static class HtmlExtensions
    {
        /// <summary>
        /// Generate honeypot input
        /// </summary>
        /// <returns>Result</returns>
        public static IHtmlContent GenerateHoneypotInput(this IHtmlHelper htmlHelper)
        {
            ArgumentNullException.ThrowIfNull(htmlHelper);

            var sb = new StringBuilder();

            sb.AppendFormat("<div style=\"display:none;\">");
            sb.Append(Environment.NewLine);

            var securitySettings = NodeEngineContext.Current.Resolve<PortalSecuritySettings>();
            sb.AppendFormat("<input id=\"{0}\" name=\"{0}\" type=\"text\">", securitySettings.HoneypotInputName);

            sb.Append(Environment.NewLine);
            sb.Append("</div>");

            return new HtmlString(sb.ToString());
        }
    }
}