﻿namespace ARWNI2S.Portal.Framework.Menu
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Checks whether this node or child ones has a specified system name
        /// </summary>
        /// <param name="node">Node</param>
        /// <param name="systemName">System name</param>
        /// <returns>Result</returns>
        public static bool ContainsSystemName(this SiteMapNode node, string systemName)
        {
            ArgumentNullException.ThrowIfNull(node);

            if (string.IsNullOrWhiteSpace(systemName))
                return false;

            if (systemName.Equals(node.SystemName, StringComparison.InvariantCultureIgnoreCase))
                return true;

            return node.ChildNodes.Any(cn => cn.ContainsSystemName(systemName));
        }

        public static SiteMapNode FindNodeRecursive(this SiteMapNode node, string systemName)
        {
            ArgumentNullException.ThrowIfNull(node);

            if (string.IsNullOrWhiteSpace(systemName))
                return null;

            if (systemName.Equals(node.SystemName, StringComparison.InvariantCultureIgnoreCase))
                return node;

            foreach (var childNode in node.ChildNodes)
            {
                var result = childNode.FindNodeRecursive(systemName);
                if (result != null)
                    return result;
            }

            return null;
        }

    }
}
