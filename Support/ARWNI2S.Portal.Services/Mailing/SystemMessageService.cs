using LinqToDB;

namespace ARWNI2S.Portal.Services.Mailing
{
    public partial class SystemMessageService : ISystemMessageService
    {
        private readonly GameplaySettings _gameplaySettings;
        private readonly IRepository<SystemMessage> _messageRepository;
        private readonly IRepository<MessageAttachment> _messageAttachmentRepository;
        private readonly IUserService _userService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IHtmlFormatter _htmlFormatter;
        private readonly IWorkContext _workContext;

        public SystemMessageService(GameplaySettings gameplaySettings,
            IRepository<SystemMessage> messageRepository,
            IRepository<MessageAttachment> messageAttachmentRepository,
            IUserService userService,
            IGenericAttributeService genericAttributeService,
            IWorkflowMessageService workflowMessageService,
            IHtmlFormatter htmlFormatter,
            IWorkContext workContext)
        {
            _gameplaySettings = gameplaySettings;
            _messageRepository = messageRepository;
            _messageAttachmentRepository = messageAttachmentRepository;
            _userService = userService;
            _genericAttributeService = genericAttributeService;
            _workflowMessageService = workflowMessageService;
            _htmlFormatter = htmlFormatter;
            _workContext = workContext;
        }

        #region System Messages

        /// <summary>
        /// Deletes a system message
        /// </summary>
        /// <param name="systemMessage">Private message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteSystemMessageAsync(SystemMessage systemMessage)
        {
            await _messageRepository.DeleteAsync(systemMessage);
        }

        /// <summary>
        /// Gets a system message
        /// </summary>
        /// <param name="systemMessageId">The system message identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the system message
        /// </returns>
        public virtual async Task<SystemMessage> GetSystemMessageByIdAsync(int systemMessageId)
        {
            return await _messageRepository.GetByIdAsync(systemMessageId, cache => default);
        }

        /// <summary>
        /// Gets private messages
        /// </summary>
        /// <param name="nodeId">The node identifier; pass 0 to load all messages</param>
        /// <param name="toPlayerId">The user identifier who should receive the message</param>
        /// <param name="isRead">A value indicating whether loaded messages are read. false - to load not read messages only, 1 to load read messages only, null to load all messages</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the private messages
        /// </returns>
        public virtual async Task<IPagedList<SystemMessage>> GetAllSystemMessagesAsync(int nodeId,
            int toPlayerId, bool? isRead,
            string keywords, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var systemMessages = await _messageRepository.GetAllPagedAsync(query =>
            {
                if (nodeId > 0)
                    query = query.Where(sm => nodeId == sm.NodeId);
                if (toPlayerId > 0)
                    query = query.Where(sm => toPlayerId == sm.ToUserId);
                if (isRead.HasValue)
                    query = query.Where(sm => isRead.Value == sm.IsRead);

                if (!string.IsNullOrEmpty(keywords))
                {
                    query = query.Where(sm => sm.Subject.Contains(keywords));
                    query = query.Where(sm => sm.Text.Contains(keywords));
                }

                query = query.OrderByDescending(sm => sm.CreatedOnUtc);

                return query;
            }, pageIndex, pageSize);

            return systemMessages;
        }

        /// <summary>
        /// Inserts a system message
        /// </summary>
        /// <param name="systemMessage">Private message</param>
        /// <param name="attachments"></param>
        /// <returns>A task that represents the asynchronous operation</returns>
        /// <exception cref="NodeException"></exception>
        public virtual async Task InsertSystemMessageAsync(SystemMessage systemMessage, IList<Reward> attachments = null)
        {
            var userTo = await _userService.GetUserByIdAsync(systemMessage.ToUserId) ?? throw new NodeException("Recipient could not be loaded");

            await _messageRepository.InsertAsync(systemMessage);

            if (attachments != null)
            {
                var messageAttachments = await attachments.Select(entity =>
                {
                    var attachent = new MessageAttachment
                    {
                        Claimed = false,
                        RewardId = entity.Id,
                        SystemMessageId = systemMessage.Id
                    };
                    return attachent;
                }).ToListAsync();

                foreach (var attachment in messageAttachments)
                    await InsertMessageAttachmentAsync(attachment);
            }

            //UI notification
            await _genericAttributeService.SaveAttributeAsync(userTo, GameplayDefaults.NotifiedAboutNewSystemMessagesAttribute, false, systemMessage.NodeId);

            //Email notification
            if (_gameplaySettings.NotifyAboutSystemMessages)
                await _workflowMessageService.SendSystemMessageNotificationAsync(systemMessage, (await _workContext.GetWorkingLanguageAsync()).Id);
        }

        /// <summary>
        /// Updates the system message
        /// </summary>
        /// <param name="systemMessage">Private message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateSystemMessageAsync(SystemMessage systemMessage)
        {
            ArgumentNullException.ThrowIfNull(systemMessage);

            await _messageRepository.UpdateAsync(systemMessage);
        }

        public virtual string FormatSystemMessageText(SystemMessage systemMessage)
        {
            var text = systemMessage.Text;

            if (string.IsNullOrEmpty(text))
                return string.Empty;

            text = _htmlFormatter.FormatText(text, false, true, false, true, false, false);

            return text;
        }

        public virtual async Task<bool> TryDeleteSystemMessageAsync(SystemMessage systemMessage)
        {
            if (systemMessage == null || !systemMessage.IsRead)
                return false;

            var attachments = await GetMessageAttachmentsBySystemMessageIdAsync(systemMessage.Id);

            if (attachments != null && attachments.Any(attachment => !attachment.Claimed))
                return false;

            await DeleteSystemMessageAsync(systemMessage);

            if (attachments != null)
                await DeleteMessageAttachmentsAsync(systemMessage);

            return true;
        }

        #endregion

        #region Message attachments

        public virtual async Task DeleteMessageAttachmentAsync(MessageAttachment messageAttachment)
        {
            await _messageAttachmentRepository.DeleteAsync(messageAttachment);
        }

        public virtual async Task DeleteMessageAttachmentsAsync(SystemMessage systemMessage)
        {
            await _messageAttachmentRepository.DeleteAsync(attachment => attachment.SystemMessageId == systemMessage.Id);
        }

        public virtual async Task InsertMessageAttachmentAsync(MessageAttachment messageAttachment)
        {
            await _messageAttachmentRepository.InsertAsync(messageAttachment);
        }

        public virtual async Task<MessageAttachment> GetMessageAttachmentByIdAsync(int id)
        {
            return await _messageAttachmentRepository.GetByIdAsync(id, cache => default);
        }

        public virtual async Task<IList<MessageAttachment>> GetMessageAttachmentsBySystemMessageIdAsync(int systemMessageId)
        {
            return await _messageAttachmentRepository.Table.Where(attachment => attachment.SystemMessageId == systemMessageId).ToListAsync();
        }


        #endregion
    }
}
