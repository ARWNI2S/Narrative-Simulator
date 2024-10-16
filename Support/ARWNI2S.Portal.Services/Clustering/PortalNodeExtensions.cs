using ARWNI2S.Node.Core.Entities.Clustering;
using ARWNI2S.Node.Data.Entities.Clustering;

namespace ARWNI2S.Portal.Services.Clustering
{
    internal static class PortalNodeExtensions
    {
        //TODO:
        public static string GetUrl(this INI2SNode node)
        {
            if (node != null && node is NI2SNode portalNode)
                return portalNode.GetUrl();

            return null;
        }
        public static string GetUrl(this NI2SNode portalNode)
        {
            return portalNode.Hosts.Split(',').FirstOrDefault();
        }
    }
}
