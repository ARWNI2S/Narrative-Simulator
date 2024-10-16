using ARWNI2S.Node.Core;
using ARWNI2S.Node.Core.Entities.Clustering;
using ARWNI2S.Node.Core.Entities.Users;
using ARWNI2S.Node.Core.Infrastructure;
using ARWNI2S.Node.Core.Runtime;
using ARWNI2S.Node.Data;
using ARWNI2S.Node.Data.Entities.Clustering;
using ARWNI2S.Node.Data.Entities.Users;
using ARWNI2S.Node.Services.Clustering;
using ARWNI2S.Node.Services.Common;

namespace ARWNI2S.Portal.Framework
{
    /// <summary>
    /// NI2SNode context for web application
    /// </summary>
    public partial class NI2SPortalContext : INodeContext
    {
        #region Fields

        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IExecutionContextAccessor _executionContextAccessor;
        private readonly IRepository<NI2SNode> _nodeRepository;
        private readonly IClusteringService _clusterService;

        private NI2SNode _cachedNode;
        private int? _cachedActiveNodeScopeConfiguration;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="genericAttributeService">Generic attribute service</param>
        /// <param name="executionContextAccessor">HTTP context accessor</param>
        /// <param name="nodeRepository">NI2SNode repository</param>
        /// <param name="clusterService">NI2SNode service</param>
        public NI2SPortalContext(
            IGenericAttributeService genericAttributeService,
            IExecutionContextAccessor executionContextAccessor,
            IRepository<NI2SNode> nodeRepository,
            IClusteringService clusterService)
        {
            _genericAttributeService = genericAttributeService;
            _executionContextAccessor = executionContextAccessor;
            _nodeRepository = nodeRepository;
            _clusterService = clusterService;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current node
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<NI2SNode> GetCurrentNodeAsync()
        {
            if (_cachedNode != null)
                return _cachedNode;

            //try to determine the current node by HOST header
            string host = _executionContextAccessor.ExecutionContext?.Request.Host;

            var allNodes = await _clusterService.GetAllNodesAsync();
            var node = allNodes.FirstOrDefault(s => _clusterService.ContainsHostValue(s, host));

            //load the first found node
            node ??= allNodes.FirstOrDefault();

            _cachedNode = node ?? throw new NodeException("No node could be loaded");

            return _cachedNode;
        }

        /// <summary>
        /// Gets the current node
        /// </summary>
        public virtual NI2SNode GetCurrentNode()
        {
            if (_cachedNode != null)
                return _cachedNode;

            //try to determine the current node by HOST header
            string host = _executionContextAccessor.ExecutionContext?.Request.Host;

            //we cannot call async methods here. otherwise, an application can hang. so it's a workaround to avoid that
            var allNodes = _nodeRepository.GetAll(query =>
            {
                return from s in query orderby s.DisplayOrder, s.Id select s;
            }, _ => default, includeDeleted: false);

            var node = allNodes.FirstOrDefault(s => _clusterService.ContainsHostValue(s, host));

            //load the first found node
            node ??= allNodes.FirstOrDefault();

            _cachedNode = node ?? throw new NodeException("No node could be loaded");

            return _cachedNode;
        }

        /// <summary>
        /// Gets active node scope configuration
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<int> GetActiveNodeScopeConfigurationAsync()
        {
            if (_cachedActiveNodeScopeConfiguration.HasValue)
                return _cachedActiveNodeScopeConfiguration.Value;

            //ensure that we have 2 (or more) nodes
            if ((await _clusterService.GetAllNodesAsync()).Count > 1)
            {
                //do not inject IWorkContext via constructor because it'll cause circular references
                var currentUser = (User)await EngineContext.Current.Resolve<IWorkContext>().GetCurrentUserAsync();

                //try to get node identifier from attributes
                var nodeId = await _genericAttributeService
                    .GetAttributeAsync<int>(currentUser, UserDefaults.AdminAreaNodeScopeConfigurationAttribute);

                _cachedActiveNodeScopeConfiguration = (await _clusterService.GetNodeByIdAsync(nodeId))?.Id ?? 0;
            }
            else
                _cachedActiveNodeScopeConfiguration = 0;

            return _cachedActiveNodeScopeConfiguration ?? 0;
        }

        Task<INI2SNode> INodeContext.GetCurrentNodeAsync()
        {
            throw new NotImplementedException();
        }

        INI2SNode INodeContext.GetCurrentNode()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}