using ARWNI2S.Node.Core.Entities.Clustering;

namespace ARWNI2S.Portal.Services.Clustering
{
    internal static class PortalNodeExtensions
    {
        //TODO:
        public static string GetUrl(this ClusterNode portalNode)
        {
            return portalNode.Hosts.Split(',').FirstOrDefault();
        }
    }
}
