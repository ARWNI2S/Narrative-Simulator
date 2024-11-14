using ARWNI2S.Node.Core;
using ARWNI2S.Node.Data;
using ARWNI2S.Node.Core.Entities.Clustering;
using ARWNI2S.Node.Data.Extensions;
using ARWNI2S.Node.Services.Clustering;

namespace ARWNI2S.Frontline.Services.Clustering
{
    /// <summary>
    /// Clustering service
    /// </summary>
    public partial class ClusteringService : IClusteringService
    {
        #region Fields

        private readonly IRepository<ClusterNode> _nodeRepository;
        private static readonly char[] separator = [','];

        #endregion

        #region Ctor

        public ClusteringService(IRepository<ClusterNode> nodeRepository)
        {
            _nodeRepository = nodeRepository;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Parse comma-separated Hosts
        /// </summary>
        /// <param name="node">Node</param>
        /// <returns>Comma-separated hosts</returns>
        protected virtual string[] ParseHostValues(ClusterNode node)
        {
            ArgumentNullException.ThrowIfNull(node);

            var parsedValues = new List<string>();
            if (string.IsNullOrEmpty(node.Hosts))
                return [.. parsedValues];

            var hosts = node.Hosts.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            foreach (var host in hosts)
            {
                var tmp = host.Trim();
                if (!string.IsNullOrEmpty(tmp))
                    parsedValues.Add(tmp);
            }

            return [.. parsedValues];
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a node
        /// </summary>
        /// <param name="node">Node</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteNodeAsync(ClusterNode node)
        {
            ArgumentNullException.ThrowIfNull(node);

            var allNodes = await GetAllNodesAsync();
            if (allNodes.Count == 1)
                throw new NodeException("You cannot delete the only configured node");

            await _nodeRepository.DeleteAsync(node);
        }

        /// <summary>
        /// Gets all nodes
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the nodes
        /// </returns>
        public virtual async Task<IList<ClusterNode>> GetAllNodesAsync(NodeState? nodeState = null)
        {
            return await _nodeRepository.GetAllAsync(query =>
            {
                if (nodeState != null)
                    query = query.Where(n => n.CurrentState == nodeState);

                return from s in query orderby s.DisplayOrder, s.Id select s;
            }, _ => default, includeDeleted: false);
        }

        /// <summary>
        /// Gets all nodes
        /// </summary>
        /// <returns>
        /// The nodes
        /// </returns>
        public virtual IList<ClusterNode> GetAllNodes()
        {
            return _nodeRepository.GetAll(query =>
            {
                return from s in query orderby s.DisplayOrder, s.Id select s;
            }, _ => default, includeDeleted: false);
        }

        /// <summary>
        /// Gets a node 
        /// </summary>
        /// <param name="nodeId">Node identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the node
        /// </returns>
        public virtual async Task<ClusterNode> GetNodeByIdAsync(int nodeId)
        {
            return await _nodeRepository.GetByIdAsync(nodeId, cache => default, false);
        }

        public async Task<ClusterNode> GetNodeByNodeIdAsync(Guid nodeId)
        {
            var query = _nodeRepository.Table;

            query = query.Where(x => x.NodeId == nodeId);

            return await query.FirstOrDefaultAsync();
        }

        /// <summary>
        /// Inserts a node
        /// </summary>
        /// <param name="node">Node</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertNodeAsync(ClusterNode node)
        {
            await _nodeRepository.InsertAsync(node);
        }

        /// <summary>
        /// Updates the node
        /// </summary>
        /// <param name="node">Node</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateNodeAsync(ClusterNode node)
        {
            await _nodeRepository.UpdateAsync(node);
        }

        /// <summary>
        /// Updates the node
        /// </summary>
        /// <param name="node">Node</param>
        public virtual void UpdateNode(ClusterNode node)
        {
            _nodeRepository.Update(node);
        }

        /// <summary>
        /// Indicates whether a node contains a specified host
        /// </summary>
        /// <param name="node">Node</param>
        /// <param name="host">Host</param>
        /// <returns>true - contains, false - no</returns>
        public virtual bool ContainsHostValue(ClusterNode node, string host)
        {
            ArgumentNullException.ThrowIfNull(node);

            if (string.IsNullOrEmpty(host))
                return false;

            var contains = ParseHostValues(node).Any(x => x.Equals(host, StringComparison.InvariantCultureIgnoreCase));

            return contains;
        }

        /// <summary>
        /// Returns a list of names of not existing nodes
        /// </summary>
        /// <param name="nodeIdsNames">The names and/or IDs of the node to check</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of names and/or IDs not existing nodes
        /// </returns>
        public async Task<Guid[]> GetNotExistingNodesAsync(string[] nodeIdsNames)
        {
            ArgumentNullException.ThrowIfNull(nodeIdsNames);

            var query = _nodeRepository.Table;

            var queryFilter = nodeIdsNames.Distinct().ToArray();

            //filtering by name
            var filter = await query.Select(node => node.NodeId.ToString())
                .Where(node => queryFilter.Contains(node))
                .ToListAsync();
            queryFilter = queryFilter.Except(filter).ToArray();

            //if some names not found
            if (queryFilter.Length == 0)
                return [];

            //filtering by IDs
            filter = await query.Select(node => node.Id.ToString())
                .Where(node => queryFilter.Contains(node))
                .ToListAsync();
            queryFilter = queryFilter.Except(filter).ToArray();

            var queryResult = await query.Select(node => node.NodeId)
                .Where(node => queryFilter.Contains(node.ToString()))
                .ToListAsync();

            return [.. queryResult];
        }

        public async Task<ClusterMap> GetClusterMapRegistrationAsync()
        {
            var allNodes = await GetAllNodesAsync();

            return new ClusterMap()
            {
                OnlineNodes = allNodes.Where(n => n.CurrentState == NodeState.Online).ToList(),
                OfflineNodes = allNodes.Where(n => n.CurrentState == NodeState.Offline).ToList(),
                NodesWithError = allNodes.Where(n => n.CurrentState == NodeState.Error).ToList(),

                SpinningUpNodes = allNodes.Where(n => n.CurrentState == NodeState.Joining).ToList(),
                SpinningDownNodes = allNodes.Where(n => n.CurrentState == NodeState.Leaving).ToList(),

                ClusterStatus = new ClusterStatus
                {
                    OnlineNodes = allNodes.Count(n => n.CurrentState == NodeState.Online),
                    OfflineNodes = allNodes.Count(n => n.CurrentState == NodeState.Offline),
                    NodesWithError = allNodes.Count(n => n.CurrentState == NodeState.Error),

                    SpinningUpNodes = allNodes.Count(n => n.CurrentState == NodeState.Joining),
                    SpinningDownNodes = allNodes.Count(n => n.CurrentState == NodeState.Leaving)
                }
            };
        }

        public ClusterStatus GetClusterStatus()
        {
            var allNodes = GetAllNodes();

            return new ClusterStatus
            {
                OnlineNodes = allNodes.Count(n => n.CurrentState == NodeState.Online),
                OfflineNodes = allNodes.Count(n => n.CurrentState == NodeState.Offline),
                NodesWithError = allNodes.Count(n => n.CurrentState == NodeState.Error),

                SpinningUpNodes = allNodes.Count(n => n.CurrentState == NodeState.Joining),
                SpinningDownNodes = allNodes.Count(n => n.CurrentState == NodeState.Leaving)
            };
        }

        #endregion
    }
}