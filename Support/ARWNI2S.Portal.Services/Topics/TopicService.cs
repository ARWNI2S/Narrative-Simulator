using ARWNI2S.Node.Core;
using ARWNI2S.Node.Core.Caching;
using ARWNI2S.Node.Data;
using ARWNI2S.Node.Core.Entities.Users;
using ARWNI2S.Node.Services.Clustering;
using ARWNI2S.Node.Services.Users;
using ARWNI2S.Portal.Services.Entities.Topics;
using ARWNI2S.Node.Services.Security;

namespace ARWNI2S.Portal.Services.Topics
{
    /// <summary>
    /// Topic service
    /// </summary>
    public partial class TopicService : ITopicService
    {
        #region Fields

        private readonly IAclService _aclService;
        private readonly IUserService _userService;
        private readonly IRepository<Topic> _topicRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly INodeMappingService _nodeMappingService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public TopicService(
            IAclService aclService,
            IUserService userService,
            IRepository<Topic> topicRepository,
            IStaticCacheManager staticCacheManager,
            INodeMappingService nodeMappingService,
            IWorkContext workContext)
        {
            _aclService = aclService;
            _userService = userService;
            _topicRepository = topicRepository;
            _staticCacheManager = staticCacheManager;
            _nodeMappingService = nodeMappingService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a topic
        /// </summary>
        /// <param name="topic">Topic</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteTopicAsync(Topic topic)
        {
            await _topicRepository.DeleteAsync(topic);
        }

        /// <summary>
        /// Gets a topic
        /// </summary>
        /// <param name="topicId">The topic identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the opic
        /// </returns>
        public virtual async Task<Topic> GetTopicByIdAsync(int topicId)
        {
            return await _topicRepository.GetByIdAsync(topicId, cache => default);
        }

        /// <summary>
        /// Gets a topic
        /// </summary>
        /// <param name="systemName">The topic system name</param>
        /// <param name="nodeId">Node identifier; pass 0 to ignore filtering by node and load the first one</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the topic
        /// </returns>
        public virtual async Task<Topic> GetTopicBySystemNameAsync(string systemName, int nodeId = 0)
        {
            if (string.IsNullOrEmpty(systemName))
                return null;

            var user = (User)await _workContext.GetCurrentUserAsync();
            var userRoleIds = await _userService.GetUserRoleIdsAsync(user);

            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(TopicServicesDefaults.TopicBySystemNameCacheKey, systemName, nodeId, userRoleIds);

            return await _staticCacheManager.GetAsync(cacheKey, async () =>
            {
                var query = _topicRepository.Table
                    .Where(t => t.Published);

                //apply node mapping constraints
                query = await _nodeMappingService.ApplyNodeMapping(query, nodeId);

                //apply ACL constraints
                query = await _aclService.ApplyAcl(query, userRoleIds);

                return query.Where(t => t.SystemName == systemName)
                    .OrderBy(t => t.Id)
                    .FirstOrDefault();
            });
        }

        /// <summary>
        /// Gets all topics
        /// </summary>
        /// <param name="nodeId">Node identifier; pass 0 to load all records</param>
        /// <param name="ignoreAcl">A value indicating whether to ignore ACL rules</param>
        /// <param name="showHidden">A value indicating whether to show hidden topics</param>
        /// <param name="onlyIncludedInTopMenu">A value indicating whether to show only topics which include on the top menu</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the topics
        /// </returns>
        public virtual async Task<IList<Topic>> GetAllTopicsAsync(int nodeId,
            bool ignoreAcl = false, bool showHidden = false, bool onlyIncludedInTopMenu = false)
        {
            var user = (User)await _workContext.GetCurrentUserAsync();
            var userRoleIds = await _userService.GetUserRoleIdsAsync(user);

            return await _topicRepository.GetAllAsync(async query =>
            {
                if (!showHidden)
                {
                    query = query.Where(t => t.Published);

                    //apply node mapping constraints
                    query = await _nodeMappingService.ApplyNodeMapping(query, nodeId);

                    //apply ACL constraints
                    if (!ignoreAcl)
                        query = await _aclService.ApplyAcl(query, userRoleIds);
                }

                if (onlyIncludedInTopMenu)
                    query = query.Where(t => t.IncludeInTopMenu);

                return query.OrderBy(t => t.DisplayOrder).ThenBy(t => t.SystemName);
            }, cache =>
            {
                return ignoreAcl
                    ? cache.PrepareKeyForDefaultCache(TopicServicesDefaults.TopicsAllCacheKey, nodeId, showHidden, onlyIncludedInTopMenu)
                    : cache.PrepareKeyForDefaultCache(TopicServicesDefaults.TopicsAllWithACLCacheKey, nodeId, showHidden, onlyIncludedInTopMenu, userRoleIds);
            });
        }

        /// <summary>
        /// Gets all topics
        /// </summary>
        /// <param name="nodeId">Node identifier; pass 0 to load all records</param>
        /// <param name="keywords">Keywords to search into body or title</param>
        /// <param name="ignoreAcl">A value indicating whether to ignore ACL rules</param>
        /// <param name="showHidden">A value indicating whether to show hidden topics</param>
        /// <param name="onlyIncludedInTopMenu">A value indicating whether to show only topics which include on the top menu</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the opics
        /// </returns>
        public virtual async Task<IList<Topic>> GetAllTopicsAsync(int nodeId, string keywords,
            bool ignoreAcl = false, bool showHidden = false, bool onlyIncludedInTopMenu = false)
        {
            var topics = await GetAllTopicsAsync(nodeId,
                ignoreAcl: ignoreAcl,
                showHidden: showHidden,
                onlyIncludedInTopMenu: onlyIncludedInTopMenu);

            if (!string.IsNullOrWhiteSpace(keywords))
            {
                return topics
                    .Where(topic => (topic.Title?.Contains(keywords, StringComparison.InvariantCultureIgnoreCase) ?? false) ||
                        (topic.Body?.Contains(keywords, StringComparison.InvariantCultureIgnoreCase) ?? false))
                    .ToList();
            }

            return topics;
        }

        /// <summary>
        /// Inserts a topic
        /// </summary>
        /// <param name="topic">Topic</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertTopicAsync(Topic topic)
        {
            await _topicRepository.InsertAsync(topic);
        }

        /// <summary>
        /// Updates the topic
        /// </summary>
        /// <param name="topic">Topic</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateTopicAsync(Topic topic)
        {
            await _topicRepository.UpdateAsync(topic);
        }

        #endregion
    }
}