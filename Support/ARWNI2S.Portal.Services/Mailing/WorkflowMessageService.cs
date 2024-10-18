using ARWNI2S.Infrastructure;
using ARWNI2S.Node.Core;
using ARWNI2S.Node.Core.Events;
using ARWNI2S.Node.Core.Entities.Clustering;
using ARWNI2S.Node.Core.Entities.Users;
using ARWNI2S.Node.Data.Extensions;
using ARWNI2S.Node.Services.Clustering;
using ARWNI2S.Node.Services.Localization;
using ARWNI2S.Portal.Services.Entities.Blogs;
using ARWNI2S.Portal.Services.Entities.Common;
using ARWNI2S.Portal.Services.Entities.Mailing;
using ARWNI2S.Portal.Services.Entities.News;
using ARWNI2S.Portal.Services.Users;
using System.Net;

namespace ARWNI2S.Portal.Services.Mailing
{
    /// <summary>
    /// Workflow message service
    /// </summary>
    public partial class WorkflowMessageService : IWorkflowMessageService
    {
        #region Fields

        private readonly CommonSettings _commonSettings;
        private readonly EmailAccountSettings _emailAccountSettings;
        //private readonly IAddressService _addressService;
        //private readonly IAffiliateService _affiliateService;
        private readonly UserService _userService;
        private readonly IEmailAccountService _emailAccountService;
        private readonly INodeEventPublisher _eventPublisher;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly IMessageTokenProvider _messageTokenProvider;
        //private readonly IOrderService _orderService;
        //private readonly IProductService _productService;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly INodeContext _nodeContext;
        private readonly IClusteringService _clusteringService;
        private readonly ITokenizer _tokenizer;
        private readonly MessagesSettings _messagesSettings;

        #endregion

        #region Ctor

        public WorkflowMessageService(CommonSettings commonSettings,
            EmailAccountSettings emailAccountSettings,
            //IAddressService addressService,
            //IAffiliateService affiliateService,
            UserService userService,
            IEmailAccountService emailAccountService,
            INodeEventPublisher eventPublisher,
            ILanguageService languageService,
            ILocalizationService localizationService,
            IMessageTemplateService messageTemplateService,
            IMessageTokenProvider messageTokenProvider,
            //IOrderService orderService,
            //IProductService productService,
            IQueuedEmailService queuedEmailService,
            INodeContext nodeContext,
            IClusteringService clusteringService,
            ITokenizer tokenizer,
            MessagesSettings messagesSettings)
        {
            _commonSettings = commonSettings;
            _emailAccountSettings = emailAccountSettings;
            //_addressService = addressService;
            //_affiliateService = affiliateService;
            _userService = userService;
            _emailAccountService = emailAccountService;
            _eventPublisher = eventPublisher;
            _languageService = languageService;
            _localizationService = localizationService;
            _messageTemplateService = messageTemplateService;
            _messageTokenProvider = messageTokenProvider;
            //_orderService = orderService;
            //_productService = productService;
            _queuedEmailService = queuedEmailService;
            _nodeContext = nodeContext;
            _clusteringService = clusteringService;
            _tokenizer = tokenizer;
            _messagesSettings = messagesSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get active message templates by the name
        /// </summary>
        /// <param name="messageTemplateName">Message template name</param>
        /// <param name="nodeId">Node identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of message templates
        /// </returns>
        protected virtual async Task<IList<MessageTemplate>> GetActiveMessageTemplatesAsync(string messageTemplateName, int nodeId)
        {
            //get message templates by the name
            var messageTemplates = await _messageTemplateService.GetMessageTemplatesByNameAsync(messageTemplateName, nodeId);

            //no template found
            if (!messageTemplates?.Any() ?? true)
                return [];

            //filter active templates
            messageTemplates = messageTemplates.Where(messageTemplate => messageTemplate.IsActive).ToList();

            return messageTemplates;
        }

        /// <summary>
        /// Get EmailAccount to use with a message templates
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the emailAccount
        /// </returns>
        protected virtual async Task<EmailAccount> GetEmailAccountOfMessageTemplateAsync(MessageTemplate messageTemplate, int languageId)
        {
            var emailAccountId = await _localizationService.GetLocalizedAsync(messageTemplate, mt => mt.EmailAccountId, languageId);
            //some 0 validation (for localizable "Email account" dropdownlist which saves 0 if "Standard" value is chosen)
            if (emailAccountId == 0)
                emailAccountId = messageTemplate.EmailAccountId;

            var emailAccount = (await _emailAccountService.GetEmailAccountByIdAsync(emailAccountId) ?? await _emailAccountService.GetEmailAccountByIdAsync(_emailAccountSettings.DefaultEmailAccountId)) ??
                               (await _emailAccountService.GetAllEmailAccountsAsync()).FirstOrDefault();
            return emailAccount;
        }

        /// <summary>
        /// Ensure language is active
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <param name="nodeId">Node identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the return a value language identifier
        /// </returns>
        protected virtual async Task<int> EnsureLanguageIsActiveAsync(int languageId, int nodeId)
        {
            //load language by specified ID
            var language = await _languageService.GetLanguageByIdAsync(languageId);

            if (language == null || !language.Published)
            {
                //load any language from the specified node
                language = (await _languageService.GetAllLanguagesAsync(nodeId: nodeId)).FirstOrDefault();
            }

            if (language == null || !language.Published)
            {
                //load any language
                language = (await _languageService.GetAllLanguagesAsync()).FirstOrDefault();
            }

            if (language == null)
                throw new NodeException("No active language could be loaded");

            return language.Id;
        }

        /// <summary>
        /// Get email and name to send email for administrator
        /// </summary>
        /// <param name="messageTemplateEmailAccount">Message template email account</param>
        /// <returns>Email address and name to send email for administrator</returns>
        protected virtual async Task<(string email, string name)> GetAdminNameAndEmailAsync(EmailAccount messageTemplateEmailAccount)
        {
            var adminEmailAccount = _messagesSettings.UseDefaultEmailAccountForSendAdminEmails ? await _emailAccountService.GetEmailAccountByIdAsync(_emailAccountSettings.DefaultEmailAccountId) : null;
            adminEmailAccount ??= messageTemplateEmailAccount;

            return (adminEmailAccount.Email, adminEmailAccount.DisplayName);
        }

        #endregion

        #region Methods

        #region User workflow

        /// <summary>
        /// Sends 'New user' notification message to an administrator
        /// </summary>
        /// <param name="user">User instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the queued email identifier
        /// </returns>
        public virtual async Task<IList<int>> SendUserRegisteredNotificationMessageAsync(User user, int languageId)
        {
            ArgumentNullException.ThrowIfNull(user);

            var node = (NI2SNode)await _nodeContext.GetCurrentNodeAsync();
            languageId = await EnsureLanguageIsActiveAsync(languageId, node.Id);

            var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.UserRegisteredNotification, node.Id);
            if (!messageTemplates.Any())
                return [];

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddUserTokensAsync(commonTokens, user);

            return await messageTemplates.SelectAwait(async messageTemplate =>
            {
                //email account
                var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                await _messageTokenProvider.AddNodeTokensAsync(tokens, node, emailAccount);

                //event notification
                await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

                var (toEmail, toName) = await GetAdminNameAndEmailAsync(emailAccount);

                return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToListAsync();
        }

        /// <summary>
        /// Sends a welcome message to a user
        /// </summary>
        /// <param name="user">User instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the queued email identifier
        /// </returns>
        public virtual async Task<IList<int>> SendUserWelcomeMessageAsync(User user, int languageId)
        {
            ArgumentNullException.ThrowIfNull(user);

            var node = (NI2SNode)await _nodeContext.GetCurrentNodeAsync();
            languageId = await EnsureLanguageIsActiveAsync(languageId, node.Id);

            var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.UserWelcomeMessage, node.Id);
            if (!messageTemplates.Any())
                return [];

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddUserTokensAsync(commonTokens, user);

            return await messageTemplates.SelectAwait(async messageTemplate =>
            {
                //email account
                var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                await _messageTokenProvider.AddNodeTokensAsync(tokens, node, emailAccount);

                //event notification
                await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

                var toEmail = user.Email;
                var toName = await _userService.GetUserFullNameAsync(user);

                return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToListAsync();
        }

        /// <summary>
        /// Sends an email validation message to a user
        /// </summary>
        /// <param name="user">User instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the queued email identifier
        /// </returns>
        public virtual async Task<IList<int>> SendUserEmailValidationMessageAsync(User user, int languageId)
        {
            ArgumentNullException.ThrowIfNull(user);

            var node = (NI2SNode)await _nodeContext.GetCurrentNodeAsync();
            languageId = await EnsureLanguageIsActiveAsync(languageId, node.Id);

            var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.UserEmailValidationMessage, node.Id);
            if (!messageTemplates.Any())
                return [];

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddUserTokensAsync(commonTokens, user);

            return await messageTemplates.SelectAwait(async messageTemplate =>
            {
                //email account
                var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                await _messageTokenProvider.AddNodeTokensAsync(tokens, node, emailAccount);

                //event notification
                await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

                var toEmail = user.Email;
                var toName = await _userService.GetUserFullNameAsync(user);

                return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToListAsync();
        }

        /// <summary>
        /// Sends an email re-validation message to a user
        /// </summary>
        /// <param name="user">User instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the queued email identifier
        /// </returns>
        public virtual async Task<IList<int>> SendUserEmailRevalidationMessageAsync(User user, int languageId)
        {
            ArgumentNullException.ThrowIfNull(user);

            var node = (NI2SNode)await _nodeContext.GetCurrentNodeAsync();
            languageId = await EnsureLanguageIsActiveAsync(languageId, node.Id);

            var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.UserEmailRevalidationMessage, node.Id);
            if (!messageTemplates.Any())
                return [];

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddUserTokensAsync(commonTokens, user);

            return await messageTemplates.SelectAwait(async messageTemplate =>
            {
                //email account
                var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                await _messageTokenProvider.AddNodeTokensAsync(tokens, node, emailAccount);

                //event notification
                await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

                //email to re-validate
                var toEmail = user.EmailToRevalidate;
                var toName = await _userService.GetUserFullNameAsync(user);

                return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToListAsync();
        }

        /// <summary>
        /// Sends password recovery message to a user
        /// </summary>
        /// <param name="user">User instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the queued email identifier
        /// </returns>
        public virtual async Task<IList<int>> SendUserPasswordRecoveryMessageAsync(User user, int languageId)
        {
            ArgumentNullException.ThrowIfNull(user);

            var node = (NI2SNode)await _nodeContext.GetCurrentNodeAsync();
            languageId = await EnsureLanguageIsActiveAsync(languageId, node.Id);

            var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.UserPasswordRecoveryMessage, node.Id);
            if (!messageTemplates.Any())
                return [];

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddUserTokensAsync(commonTokens, user);

            return await messageTemplates.SelectAwait(async messageTemplate =>
            {
                //email account
                var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                await _messageTokenProvider.AddNodeTokensAsync(tokens, node, emailAccount);

                //event notification
                await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

                var toEmail = user.Email;
                var toName = await _userService.GetUserFullNameAsync(user);

                return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToListAsync();
        }

        #endregion

        #region Order workflow

        ///// <summary>
        ///// Sends an order placed notification to a partner
        ///// </summary>
        ///// <param name="order">Order instance</param>
        ///// <param name="partner">Partner instance</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the queued email identifier
        ///// </returns>
        //public virtual async Task<IList<int>> SendOrderPlacedPartnerNotificationAsync(Order order, Partner partner, int languageId)
        //{
        //    if (order == null)
        //        throw new ArgumentNullException(nameof(order));

        //    if (partner == null)
        //        throw new ArgumentNullException(nameof(partner));

        //    var node = await _clusteringService.GetNodeByIdAsync(order.NodeId) ?? await _nodeContext.GetCurrentNodeAsync();
        //    languageId = await EnsureLanguageIsActiveAsync(languageId, node.Id);

        //    var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.OrderPlacedPartnerNotification, node.Id);
        //    if (!messageTemplates.Any())
        //        return new List<int>();

        //    //tokens
        //    var commonTokens = new List<Token>();
        //    await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId, partner.Id);
        //    await _messageTokenProvider.AddUserTokensAsync(commonTokens, order.UserId);

        //    return await messageTemplates.SelectAwait(async messageTemplate =>
        //    {
        //        //email account
        //        var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

        //        var tokens = new List<Token>(commonTokens);
        //        await _messageTokenProvider.AddNodeTokensAsync(tokens, node, emailAccount);

        //        //event notification
        //        await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

        //        var toEmail = partner.Email;
        //        var toName = partner.Name;

        //        return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        //    }).ToListAsync();
        //}

        ///// <summary>
        ///// Sends an order placed notification to an administrator
        ///// </summary>
        ///// <param name="order">Order instance</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the queued email identifier
        ///// </returns>
        //public virtual async Task<IList<int>> SendOrderPlacedNotificationAsync(Order order, int languageId)
        //{
        //    if (order == null)
        //        throw new ArgumentNullException(nameof(order));

        //    var node = await _clusteringService.GetNodeByIdAsync(order.NodeId) ?? await _nodeContext.GetCurrentNodeAsync();
        //    languageId = await EnsureLanguageIsActiveAsync(languageId, node.Id);

        //    var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.OrderPlacedNotification, node.Id);
        //    if (!messageTemplates.Any())
        //        return new List<int>();

        //    //tokens
        //    var commonTokens = new List<Token>();
        //    await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
        //    await _messageTokenProvider.AddUserTokensAsync(commonTokens, order.UserId);

        //    return await messageTemplates.SelectAwait(async messageTemplate =>
        //    {
        //        //email account
        //        var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

        //        var tokens = new List<Token>(commonTokens);
        //        await _messageTokenProvider.AddNodeTokensAsync(tokens, node, emailAccount);

        //        //event notification
        //        await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

        //        var (toEmail, toName) = await GetAdminNameAndEmailAsync(emailAccount);

        //        return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        //    }).ToListAsync();
        //}

        ///// <summary>
        ///// Sends an order placed notification to an affiliate
        ///// </summary>
        ///// <param name="order">Order instance</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the queued email identifier
        ///// </returns>
        //public virtual async Task<IList<int>> SendOrderPlacedAffiliateNotificationAsync(Order order, int languageId)
        //{
        //    if (order == null)
        //        throw new ArgumentNullException(nameof(order));

        //    var affiliate = await _affiliateService.GetAffiliateByIdAsync(order.AffiliateId);

        //    if (affiliate == null)
        //        throw new ArgumentNullException(nameof(affiliate));

        //    var node = await _clusteringService.GetNodeByIdAsync(order.NodeId) ?? await _nodeContext.GetCurrentNodeAsync();
        //    languageId = await EnsureLanguageIsActiveAsync(languageId, node.Id);

        //    var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.OrderPlacedAffiliateNotification, node.Id);
        //    if (!messageTemplates.Any())
        //        return new List<int>();

        //    //tokens
        //    var commonTokens = new List<Token>();
        //    await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
        //    await _messageTokenProvider.AddUserTokensAsync(commonTokens, order.UserId);

        //    return await messageTemplates.SelectAwait(async messageTemplate =>
        //    {
        //        //email account
        //        var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

        //        var tokens = new List<Token>(commonTokens);
        //        await _messageTokenProvider.AddNodeTokensAsync(tokens, node, emailAccount);

        //        //event notification
        //        await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

        //        var affiliateAddress = await _addressService.GetAddressByIdAsync(affiliate.AddressId);
        //        var toEmail = affiliateAddress.Email;
        //        var toName = $"{affiliateAddress.FirstName} {affiliateAddress.LastName}";

        //        return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        //    }).ToListAsync();
        //}

        ///// <summary>
        ///// Sends an order paid notification to an administrator
        ///// </summary>
        ///// <param name="order">Order instance</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the queued email identifier
        ///// </returns>
        //public virtual async Task<IList<int>> SendOrderPaidNotificationAsync(Order order, int languageId)
        //{
        //    if (order == null)
        //        throw new ArgumentNullException(nameof(order));

        //    var node = await _clusteringService.GetNodeByIdAsync(order.NodeId) ?? await _nodeContext.GetCurrentNodeAsync();
        //    languageId = await EnsureLanguageIsActiveAsync(languageId, node.Id);

        //    var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.OrderPaidNotification, node.Id);
        //    if (!messageTemplates.Any())
        //        return new List<int>();

        //    //tokens
        //    var commonTokens = new List<Token>();
        //    await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
        //    await _messageTokenProvider.AddUserTokensAsync(commonTokens, order.UserId);

        //    return await messageTemplates.SelectAwait(async messageTemplate =>
        //    {
        //        //email account
        //        var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

        //        var tokens = new List<Token>(commonTokens);
        //        await _messageTokenProvider.AddNodeTokensAsync(tokens, node, emailAccount);

        //        //event notification
        //        await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

        //        var (toEmail, toName) = await GetAdminNameAndEmailAsync(emailAccount);

        //        return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        //    }).ToListAsync();
        //}

        ///// <summary>
        ///// Sends an order paid notification to an affiliate
        ///// </summary>
        ///// <param name="order">Order instance</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the queued email identifier
        ///// </returns>
        //public virtual async Task<IList<int>> SendOrderPaidAffiliateNotificationAsync(Order order, int languageId)
        //{
        //    if (order == null)
        //        throw new ArgumentNullException(nameof(order));

        //    var affiliate = await _affiliateService.GetAffiliateByIdAsync(order.AffiliateId);

        //    if (affiliate == null)
        //        throw new ArgumentNullException(nameof(affiliate));

        //    var node = await _clusteringService.GetNodeByIdAsync(order.NodeId) ?? await _nodeContext.GetCurrentNodeAsync();
        //    languageId = await EnsureLanguageIsActiveAsync(languageId, node.Id);

        //    var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.OrderPaidAffiliateNotification, node.Id);
        //    if (!messageTemplates.Any())
        //        return new List<int>();

        //    //tokens
        //    var commonTokens = new List<Token>();
        //    await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
        //    await _messageTokenProvider.AddUserTokensAsync(commonTokens, order.UserId);

        //    return await messageTemplates.SelectAwait(async messageTemplate =>
        //    {
        //        //email account
        //        var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

        //        var tokens = new List<Token>(commonTokens);
        //        await _messageTokenProvider.AddNodeTokensAsync(tokens, node, emailAccount);

        //        //event notification
        //        await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

        //        var affiliateAddress = await _addressService.GetAddressByIdAsync(affiliate.AddressId);
        //        var toEmail = affiliateAddress.Email;
        //        var toName = $"{affiliateAddress.FirstName} {affiliateAddress.LastName}";

        //        return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        //    }).ToListAsync();
        //}

        ///// <summary>
        ///// Sends an order paid notification to a user
        ///// </summary>
        ///// <param name="order">Order instance</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <param name="attachmentFilePath">Attachment file path</param>
        ///// <param name="attachmentFileName">Attachment file name. If specified, then this file name will be sent to a recipient. Otherwise, "AttachmentFilePath" name will be used.</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the queued email identifier
        ///// </returns>
        //public virtual async Task<IList<int>> SendOrderPaidUserNotificationAsync(Order order, int languageId,
        //    string attachmentFilePath = null, string attachmentFileName = null)
        //{
        //    if (order == null)
        //        throw new ArgumentNullException(nameof(order));

        //    var node = await _clusteringService.GetNodeByIdAsync(order.NodeId) ?? await _nodeContext.GetCurrentNodeAsync();
        //    languageId = await EnsureLanguageIsActiveAsync(languageId, node.Id);

        //    var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.OrderPaidUserNotification, node.Id);
        //    if (!messageTemplates.Any())
        //        return new List<int>();

        //    //tokens
        //    var commonTokens = new List<Token>();
        //    await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
        //    await _messageTokenProvider.AddUserTokensAsync(commonTokens, order.UserId);

        //    return await messageTemplates.SelectAwait(async messageTemplate =>
        //    {
        //        //email account
        //        var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

        //        var tokens = new List<Token>(commonTokens);
        //        await _messageTokenProvider.AddNodeTokensAsync(tokens, node, emailAccount);

        //        //event notification
        //        await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

        //        var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

        //        var toEmail = billingAddress.Email;
        //        var toName = $"{billingAddress.FirstName} {billingAddress.LastName}";

        //        return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName,
        //            attachmentFilePath, attachmentFileName);
        //    }).ToListAsync();
        //}

        ///// <summary>
        ///// Sends an order paid notification to a partner
        ///// </summary>
        ///// <param name="order">Order instance</param>
        ///// <param name="partner">Partner instance</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the queued email identifier
        ///// </returns>
        //public virtual async Task<IList<int>> SendOrderPaidPartnerNotificationAsync(Order order, Partner partner, int languageId)
        //{
        //    if (order == null)
        //        throw new ArgumentNullException(nameof(order));

        //    if (partner == null)
        //        throw new ArgumentNullException(nameof(partner));

        //    var node = await _clusteringService.GetNodeByIdAsync(order.NodeId) ?? await _nodeContext.GetCurrentNodeAsync();
        //    languageId = await EnsureLanguageIsActiveAsync(languageId, node.Id);

        //    var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.OrderPaidPartnerNotification, node.Id);
        //    if (!messageTemplates.Any())
        //        return new List<int>();

        //    //tokens
        //    var commonTokens = new List<Token>();
        //    await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId, partner.Id);
        //    await _messageTokenProvider.AddUserTokensAsync(commonTokens, order.UserId);

        //    return await messageTemplates.SelectAwait(async messageTemplate =>
        //    {
        //        //email account
        //        var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

        //        var tokens = new List<Token>(commonTokens);
        //        await _messageTokenProvider.AddNodeTokensAsync(tokens, node, emailAccount);

        //        //event notification
        //        await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

        //        var toEmail = partner.Email;
        //        var toName = partner.Name;

        //        return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        //    }).ToListAsync();
        //}

        ///// <summary>
        ///// Sends an order placed notification to a user
        ///// </summary>
        ///// <param name="order">Order instance</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <param name="attachmentFilePath">Attachment file path</param>
        ///// <param name="attachmentFileName">Attachment file name. If specified, then this file name will be sent to a recipient. Otherwise, "AttachmentFilePath" name will be used.</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the queued email identifier
        ///// </returns>
        //public virtual async Task<IList<int>> SendOrderPlacedUserNotificationAsync(Order order, int languageId,
        //    string attachmentFilePath = null, string attachmentFileName = null)
        //{
        //    if (order == null)
        //        throw new ArgumentNullException(nameof(order));

        //    var node = await _clusteringService.GetNodeByIdAsync(order.NodeId) ?? await _nodeContext.GetCurrentNodeAsync();
        //    languageId = await EnsureLanguageIsActiveAsync(languageId, node.Id);

        //    var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.OrderPlacedUserNotification, node.Id);
        //    if (!messageTemplates.Any())
        //        return new List<int>();

        //    //tokens
        //    var commonTokens = new List<Token>();
        //    await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
        //    await _messageTokenProvider.AddUserTokensAsync(commonTokens, order.UserId);

        //    return await messageTemplates.SelectAwait(async messageTemplate =>
        //    {
        //        //email account
        //        var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

        //        var tokens = new List<Token>(commonTokens);
        //        await _messageTokenProvider.AddNodeTokensAsync(tokens, node, emailAccount);

        //        //event notification
        //        await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

        //        var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

        //        var toEmail = billingAddress.Email;
        //        var toName = $"{billingAddress.FirstName} {billingAddress.LastName}";

        //        return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName,
        //            attachmentFilePath, attachmentFileName);
        //    }).ToListAsync();
        //}

        ///// <summary>
        ///// Sends a shipment sent notification to a user
        ///// </summary>
        ///// <param name="shipment">Shipment</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the queued email identifier
        ///// </returns>
        //public virtual async Task<IList<int>> SendShipmentSentUserNotificationAsync(Shipment shipment, int languageId)
        //{
        //    if (shipment == null)
        //        throw new ArgumentNullException(nameof(shipment));

        //    var order = await _orderService.GetOrderByIdAsync(shipment.OrderId) ?? throw new NodeException("Order cannot be loaded");
        //    var node = await _clusteringService.GetNodeByIdAsync(order.NodeId) ?? await _nodeContext.GetCurrentNodeAsync();
        //    languageId = await EnsureLanguageIsActiveAsync(languageId, node.Id);

        //    var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.ShipmentSentUserNotification, node.Id);
        //    if (!messageTemplates.Any())
        //        return new List<int>();

        //    //tokens
        //    var commonTokens = new List<Token>();
        //    await _messageTokenProvider.AddShipmentTokensAsync(commonTokens, shipment, languageId);
        //    await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
        //    await _messageTokenProvider.AddUserTokensAsync(commonTokens, order.UserId);

        //    return await messageTemplates.SelectAwait(async messageTemplate =>
        //    {
        //        //email account
        //        var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

        //        var tokens = new List<Token>(commonTokens);
        //        await _messageTokenProvider.AddNodeTokensAsync(tokens, node, emailAccount);

        //        //event notification
        //        await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

        //        var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

        //        var toEmail = billingAddress.Email;
        //        var toName = $"{billingAddress.FirstName} {billingAddress.LastName}";

        //        return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        //    }).ToListAsync();
        //}

        ///// <summary>
        ///// Sends a shipment ready for pickup notification to a user
        ///// </summary>
        ///// <param name="shipment">Shipment</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the queued email identifier
        ///// </returns>
        //public virtual async Task<IList<int>> SendShipmentReadyForPickupNotificationAsync(Shipment shipment, int languageId)
        //{
        //    var order = await _orderService.GetOrderByIdAsync(shipment.OrderId) ?? throw new NodeException("Order cannot be loaded");
        //    var node = await _clusteringService.GetNodeByIdAsync(order.NodeId) ?? await _nodeContext.GetCurrentNodeAsync();
        //    languageId = await EnsureLanguageIsActiveAsync(languageId, node.Id);

        //    var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.ShipmentReadyForPickupUserNotification, node.Id);
        //    if (!messageTemplates.Any())
        //        return new List<int>();

        //    //tokens
        //    var commonTokens = new List<Token>();
        //    await _messageTokenProvider.AddShipmentTokensAsync(commonTokens, shipment, languageId);
        //    await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
        //    await _messageTokenProvider.AddUserTokensAsync(commonTokens, order.UserId);

        //    return await messageTemplates.SelectAwait(async messageTemplate =>
        //    {
        //        //email account
        //        var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

        //        var tokens = new List<Token>(commonTokens);
        //        await _messageTokenProvider.AddNodeTokensAsync(tokens, node, emailAccount);

        //        //event notification
        //        await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

        //        var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

        //        var toEmail = billingAddress.Email;
        //        var toName = $"{billingAddress.FirstName} {billingAddress.LastName}";

        //        return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        //    }).ToListAsync();
        //}

        ///// <summary>
        ///// Sends a shipment delivered notification to a user
        ///// </summary>
        ///// <param name="shipment">Shipment</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the queued email identifier
        ///// </returns>
        //public virtual async Task<IList<int>> SendShipmentDeliveredUserNotificationAsync(Shipment shipment, int languageId)
        //{
        //    if (shipment == null)
        //        throw new ArgumentNullException(nameof(shipment));

        //    var order = await _orderService.GetOrderByIdAsync(shipment.OrderId) ?? throw new NodeException("Order cannot be loaded");
        //    var node = await _clusteringService.GetNodeByIdAsync(order.NodeId) ?? await _nodeContext.GetCurrentNodeAsync();
        //    languageId = await EnsureLanguageIsActiveAsync(languageId, node.Id);

        //    var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.ShipmentDeliveredUserNotification, node.Id);
        //    if (!messageTemplates.Any())
        //        return new List<int>();

        //    //tokens
        //    var commonTokens = new List<Token>();
        //    await _messageTokenProvider.AddShipmentTokensAsync(commonTokens, shipment, languageId);
        //    await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
        //    await _messageTokenProvider.AddUserTokensAsync(commonTokens, order.UserId);

        //    return await messageTemplates.SelectAwait(async messageTemplate =>
        //    {
        //        //email account
        //        var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

        //        var tokens = new List<Token>(commonTokens);
        //        await _messageTokenProvider.AddNodeTokensAsync(tokens, node, emailAccount);

        //        //event notification
        //        await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

        //        var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

        //        var toEmail = billingAddress.Email;
        //        var toName = $"{billingAddress.FirstName} {billingAddress.LastName}";

        //        return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        //    }).ToListAsync();
        //}

        ///// <summary>
        ///// Sends an order processing notification to a user
        ///// </summary>
        ///// <param name="order">Order instance</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <param name="attachmentFilePath">Attachment file path</param>
        ///// <param name="attachmentFileName">Attachment file name. If specified, then this file name will be sent to a recipient. Otherwise, "AttachmentFilePath" name will be used.</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the queued email identifier
        ///// </returns>
        //public virtual async Task<IList<int>> SendOrderProcessingUserNotificationAsync(Order order, int languageId,
        //    string attachmentFilePath = null, string attachmentFileName = null)
        //{
        //    if (order == null)
        //        throw new ArgumentNullException(nameof(order));

        //    var node = await _clusteringService.GetNodeByIdAsync(order.NodeId) ?? await _nodeContext.GetCurrentNodeAsync();
        //    languageId = await EnsureLanguageIsActiveAsync(languageId, node.Id);

        //    var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.OrderProcessingUserNotification, node.Id);
        //    if (!messageTemplates.Any())
        //        return new List<int>();

        //    //tokens
        //    var commonTokens = new List<Token>();
        //    await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
        //    await _messageTokenProvider.AddUserTokensAsync(commonTokens, order.UserId);

        //    return await messageTemplates.SelectAwait(async messageTemplate =>
        //    {
        //        //email account
        //        var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

        //        var tokens = new List<Token>(commonTokens);
        //        await _messageTokenProvider.AddNodeTokensAsync(tokens, node, emailAccount);

        //        //event notification
        //        await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

        //        var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

        //        var toEmail = billingAddress.Email;
        //        var toName = $"{billingAddress.FirstName} {billingAddress.LastName}";

        //        return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName,
        //            attachmentFilePath, attachmentFileName);
        //    }).ToListAsync();
        //}

        ///// <summary>
        ///// Sends an order completed notification to a user
        ///// </summary>
        ///// <param name="order">Order instance</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <param name="attachmentFilePath">Attachment file path</param>
        ///// <param name="attachmentFileName">Attachment file name. If specified, then this file name will be sent to a recipient. Otherwise, "AttachmentFilePath" name will be used.</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the queued email identifier
        ///// </returns>
        //public virtual async Task<IList<int>> SendOrderCompletedUserNotificationAsync(Order order, int languageId,
        //    string attachmentFilePath = null, string attachmentFileName = null)
        //{
        //    if (order == null)
        //        throw new ArgumentNullException(nameof(order));

        //    var node = await _clusteringService.GetNodeByIdAsync(order.NodeId) ?? await _nodeContext.GetCurrentNodeAsync();
        //    languageId = await EnsureLanguageIsActiveAsync(languageId, node.Id);

        //    var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.OrderCompletedUserNotification, node.Id);
        //    if (!messageTemplates.Any())
        //        return new List<int>();

        //    //tokens
        //    var commonTokens = new List<Token>();
        //    await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
        //    await _messageTokenProvider.AddUserTokensAsync(commonTokens, order.UserId);

        //    return await messageTemplates.SelectAwait(async messageTemplate =>
        //    {
        //        //email account
        //        var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

        //        var tokens = new List<Token>(commonTokens);
        //        await _messageTokenProvider.AddNodeTokensAsync(tokens, node, emailAccount);

        //        //event notification
        //        await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

        //        var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

        //        var toEmail = billingAddress.Email;
        //        var toName = $"{billingAddress.FirstName} {billingAddress.LastName}";

        //        return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName,
        //            attachmentFilePath, attachmentFileName);
        //    }).ToListAsync();
        //}

        ///// <summary>
        ///// Sends an order cancelled notification to a user
        ///// </summary>
        ///// <param name="order">Order instance</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the queued email identifier
        ///// </returns>
        //public virtual async Task<IList<int>> SendOrderCancelledUserNotificationAsync(Order order, int languageId)
        //{
        //    if (order == null)
        //        throw new ArgumentNullException(nameof(order));

        //    var node = await _clusteringService.GetNodeByIdAsync(order.NodeId) ?? await _nodeContext.GetCurrentNodeAsync();
        //    languageId = await EnsureLanguageIsActiveAsync(languageId, node.Id);

        //    var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.OrderCancelledUserNotification, node.Id);
        //    if (!messageTemplates.Any())
        //        return new List<int>();

        //    //tokens
        //    var commonTokens = new List<Token>();
        //    await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
        //    await _messageTokenProvider.AddUserTokensAsync(commonTokens, order.UserId);

        //    return await messageTemplates.SelectAwait(async messageTemplate =>
        //    {
        //        //email account
        //        var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

        //        var tokens = new List<Token>(commonTokens);
        //        await _messageTokenProvider.AddNodeTokensAsync(tokens, node, emailAccount);

        //        //event notification
        //        await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

        //        var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

        //        var toEmail = billingAddress.Email;
        //        var toName = $"{billingAddress.FirstName} {billingAddress.LastName}";

        //        return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        //    }).ToListAsync();
        //}

        ///// <summary>
        ///// Sends an order refunded notification to an administrator
        ///// </summary>
        ///// <param name="order">Order instance</param>
        ///// <param name="refundedAmount">Amount refunded</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the queued email identifier
        ///// </returns>
        //public virtual async Task<IList<int>> SendOrderRefundedNotificationAsync(Order order, decimal refundedAmount, int languageId)
        //{
        //    if (order == null)
        //        throw new ArgumentNullException(nameof(order));

        //    var node = await _clusteringService.GetNodeByIdAsync(order.NodeId) ?? await _nodeContext.GetCurrentNodeAsync();
        //    languageId = await EnsureLanguageIsActiveAsync(languageId, node.Id);

        //    var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.OrderRefundedNotification, node.Id);
        //    if (!messageTemplates.Any())
        //        return new List<int>();

        //    //tokens
        //    var commonTokens = new List<Token>();
        //    await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
        //    await _messageTokenProvider.AddOrderRefundedTokensAsync(commonTokens, order, refundedAmount);
        //    await _messageTokenProvider.AddUserTokensAsync(commonTokens, order.UserId);

        //    return await messageTemplates.SelectAwait(async messageTemplate =>
        //    {
        //        //email account
        //        var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

        //        var tokens = new List<Token>(commonTokens);
        //        await _messageTokenProvider.AddNodeTokensAsync(tokens, node, emailAccount);

        //        //event notification
        //        await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

        //        var (toEmail, toName) = await GetAdminNameAndEmailAsync(emailAccount);

        //        return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        //    }).ToListAsync();
        //}

        ///// <summary>
        ///// Sends an order refunded notification to a user
        ///// </summary>
        ///// <param name="order">Order instance</param>
        ///// <param name="refundedAmount">Amount refunded</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the queued email identifier
        ///// </returns>
        //public virtual async Task<IList<int>> SendOrderRefundedUserNotificationAsync(Order order, decimal refundedAmount, int languageId)
        //{
        //    if (order == null)
        //        throw new ArgumentNullException(nameof(order));

        //    var node = await _clusteringService.GetNodeByIdAsync(order.NodeId) ?? await _nodeContext.GetCurrentNodeAsync();
        //    languageId = await EnsureLanguageIsActiveAsync(languageId, node.Id);

        //    var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.OrderRefundedUserNotification, node.Id);
        //    if (!messageTemplates.Any())
        //        return new List<int>();

        //    //tokens
        //    var commonTokens = new List<Token>();
        //    await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
        //    await _messageTokenProvider.AddOrderRefundedTokensAsync(commonTokens, order, refundedAmount);
        //    await _messageTokenProvider.AddUserTokensAsync(commonTokens, order.UserId);

        //    return await messageTemplates.SelectAwait(async messageTemplate =>
        //    {
        //        //email account
        //        var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

        //        var tokens = new List<Token>(commonTokens);
        //        await _messageTokenProvider.AddNodeTokensAsync(tokens, node, emailAccount);

        //        //event notification
        //        await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

        //        var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

        //        var toEmail = billingAddress.Email;
        //        var toName = $"{billingAddress.FirstName} {billingAddress.LastName}";

        //        return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        //    }).ToListAsync();
        //}

        ///// <summary>
        ///// Sends a new order note added notification to a user
        ///// </summary>
        ///// <param name="orderNote">Order note</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the queued email identifier
        ///// </returns>
        //public virtual async Task<IList<int>> SendNewOrderNoteAddedUserNotificationAsync(OrderNote orderNote, int languageId)
        //{
        //    if (orderNote == null)
        //        throw new ArgumentNullException(nameof(orderNote));

        //    var order = await _orderService.GetOrderByIdAsync(orderNote.OrderId) ?? throw new NodeException("Order cannot be loaded");
        //    var node = await _clusteringService.GetNodeByIdAsync(order.NodeId) ?? await _nodeContext.GetCurrentNodeAsync();
        //    languageId = await EnsureLanguageIsActiveAsync(languageId, node.Id);

        //    var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.NewOrderNoteAddedUserNotification, node.Id);
        //    if (!messageTemplates.Any())
        //        return new List<int>();

        //    //tokens
        //    var commonTokens = new List<Token>();
        //    await _messageTokenProvider.AddOrderNoteTokensAsync(commonTokens, orderNote);
        //    await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
        //    await _messageTokenProvider.AddUserTokensAsync(commonTokens, order.UserId);

        //    return await messageTemplates.SelectAwait(async messageTemplate =>
        //    {
        //        //email account
        //        var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

        //        var tokens = new List<Token>(commonTokens);
        //        await _messageTokenProvider.AddNodeTokensAsync(tokens, node, emailAccount);

        //        //event notification
        //        await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

        //        var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

        //        var toEmail = billingAddress.Email;
        //        var toName = $"{billingAddress.FirstName} {billingAddress.LastName}";

        //        return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        //    }).ToListAsync();
        //}

        ///// <summary>
        ///// Sends a "Recurring payment cancelled" notification to an administrator
        ///// </summary>
        ///// <param name="recurringPayment">Recurring payment</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the queued email identifier
        ///// </returns>
        //public virtual async Task<IList<int>> SendRecurringPaymentCancelledNotificationAsync(RecurringPayment recurringPayment, int languageId)
        //{
        //    if (recurringPayment == null)
        //        throw new ArgumentNullException(nameof(recurringPayment));

        //    var order = await _orderService.GetOrderByIdAsync(recurringPayment.InitialOrderId) ?? throw new NodeException("Order cannot be loaded");
        //    var node = await _clusteringService.GetNodeByIdAsync(order.NodeId) ?? await _nodeContext.GetCurrentNodeAsync();
        //    languageId = await EnsureLanguageIsActiveAsync(languageId, node.Id);

        //    var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.RecurringPaymentCancelledNotification, node.Id);
        //    if (!messageTemplates.Any())
        //        return new List<int>();

        //    //tokens
        //    var commonTokens = new List<Token>();
        //    await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
        //    await _messageTokenProvider.AddUserTokensAsync(commonTokens, order.UserId);
        //    await _messageTokenProvider.AddRecurringPaymentTokensAsync(commonTokens, recurringPayment);

        //    return await messageTemplates.SelectAwait(async messageTemplate =>
        //    {
        //        //email account
        //        var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

        //        var tokens = new List<Token>(commonTokens);
        //        await _messageTokenProvider.AddNodeTokensAsync(tokens, node, emailAccount);

        //        //event notification
        //        await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

        //        var (toEmail, toName) = await GetAdminNameAndEmailAsync(emailAccount);

        //        return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        //    }).ToListAsync();
        //}

        ///// <summary>
        ///// Sends a "Recurring payment cancelled" notification to a user
        ///// </summary>
        ///// <param name="recurringPayment">Recurring payment</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the queued email identifier
        ///// </returns>
        //public virtual async Task<IList<int>> SendRecurringPaymentCancelledUserNotificationAsync(RecurringPayment recurringPayment, int languageId)
        //{
        //    if (recurringPayment == null)
        //        throw new ArgumentNullException(nameof(recurringPayment));

        //    var order = await _orderService.GetOrderByIdAsync(recurringPayment.InitialOrderId) ?? throw new NodeException("Order cannot be loaded");
        //    var node = await _clusteringService.GetNodeByIdAsync(order.NodeId) ?? await _nodeContext.GetCurrentNodeAsync();
        //    languageId = await EnsureLanguageIsActiveAsync(languageId, node.Id);

        //    var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.RecurringPaymentCancelledUserNotification, node.Id);
        //    if (!messageTemplates.Any())
        //        return new List<int>();

        //    //tokens
        //    var commonTokens = new List<Token>();
        //    await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
        //    await _messageTokenProvider.AddUserTokensAsync(commonTokens, order.UserId);
        //    await _messageTokenProvider.AddRecurringPaymentTokensAsync(commonTokens, recurringPayment);

        //    return await messageTemplates.SelectAwait(async messageTemplate =>
        //    {
        //        //email account
        //        var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

        //        var tokens = new List<Token>(commonTokens);
        //        await _messageTokenProvider.AddNodeTokensAsync(tokens, node, emailAccount);

        //        //event notification
        //        await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

        //        var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

        //        var toEmail = billingAddress.Email;
        //        var toName = $"{billingAddress.FirstName} {billingAddress.LastName}";

        //        return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        //    }).ToListAsync();
        //}

        ///// <summary>
        ///// Sends a "Recurring payment failed" notification to a user
        ///// </summary>
        ///// <param name="recurringPayment">Recurring payment</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the queued email identifier
        ///// </returns>
        //public virtual async Task<IList<int>> SendRecurringPaymentFailedUserNotificationAsync(RecurringPayment recurringPayment, int languageId)
        //{
        //    if (recurringPayment == null)
        //        throw new ArgumentNullException(nameof(recurringPayment));

        //    var order = await _orderService.GetOrderByIdAsync(recurringPayment.InitialOrderId) ?? throw new NodeException("Order cannot be loaded");
        //    var node = await _clusteringService.GetNodeByIdAsync(order.NodeId) ?? await _nodeContext.GetCurrentNodeAsync();
        //    languageId = await EnsureLanguageIsActiveAsync(languageId, node.Id);

        //    var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.RecurringPaymentFailedUserNotification, node.Id);
        //    if (!messageTemplates.Any())
        //        return new List<int>();

        //    //tokens
        //    var commonTokens = new List<Token>();
        //    await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
        //    await _messageTokenProvider.AddUserTokensAsync(commonTokens, order.UserId);
        //    await _messageTokenProvider.AddRecurringPaymentTokensAsync(commonTokens, recurringPayment);

        //    return await messageTemplates.SelectAwait(async messageTemplate =>
        //    {
        //        //email account
        //        var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

        //        var tokens = new List<Token>(commonTokens);
        //        await _messageTokenProvider.AddNodeTokensAsync(tokens, node, emailAccount);

        //        //event notification
        //        await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

        //        var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

        //        var toEmail = billingAddress.Email;
        //        var toName = $"{billingAddress.FirstName} {billingAddress.LastName}";

        //        return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        //    }).ToListAsync();
        //}

        #endregion

        #region Newsletter workflow

        /// <summary>
        /// Sends a newsletter subscription activation message
        /// </summary>
        /// <param name="subscription">Newsletter subscription</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the queued email identifier
        /// </returns>
        public virtual async Task<IList<int>> SendNewsLetterSubscriptionActivationMessageAsync(NewsLetterSubscription subscription, int languageId)
        {
            ArgumentNullException.ThrowIfNull(subscription);

            var node = (NI2SNode)await _nodeContext.GetCurrentNodeAsync();
            languageId = await EnsureLanguageIsActiveAsync(languageId, node.Id);

            var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.NewsletterSubscriptionActivationMessage, node.Id);
            if (!messageTemplates.Any())
                return [];

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddNewsLetterSubscriptionTokensAsync(commonTokens, subscription);

            return await messageTemplates.SelectAwait(async messageTemplate =>
            {
                //email account
                var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                await _messageTokenProvider.AddNodeTokensAsync(tokens, node, emailAccount);

                //event notification
                await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

                return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, subscription.Email, string.Empty);
            }).ToListAsync();
        }

        /// <summary>
        /// Sends a newsletter subscription deactivation message
        /// </summary>
        /// <param name="subscription">Newsletter subscription</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the queued email identifier
        /// </returns>
        public virtual async Task<IList<int>> SendNewsLetterSubscriptionDeactivationMessageAsync(NewsLetterSubscription subscription, int languageId)
        {
            ArgumentNullException.ThrowIfNull(subscription);

            var node = (NI2SNode)await _nodeContext.GetCurrentNodeAsync();
            languageId = await EnsureLanguageIsActiveAsync(languageId, node.Id);

            var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.NewsletterSubscriptionDeactivationMessage, node.Id);
            if (!messageTemplates.Any())
                return [];

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddNewsLetterSubscriptionTokensAsync(commonTokens, subscription);

            return await messageTemplates.SelectAwait(async messageTemplate =>
            {
                //email account
                var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                await _messageTokenProvider.AddNodeTokensAsync(tokens, node, emailAccount);

                //event notification
                await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

                return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, subscription.Email, string.Empty);
            }).ToListAsync();
        }

        #endregion

        #region Send a message to a friend

        ///// <summary>
        ///// Sends "email a friend" message
        ///// </summary>
        ///// <param name="user">User instance</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <param name="product">Product instance</param>
        ///// <param name="userEmail">User's email</param>
        ///// <param name="friendsEmail">Friend's email</param>
        ///// <param name="personalMessage">Personal message</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the queued email identifier
        ///// </returns>
        //public virtual async Task<IList<int>> SendProductEmailAFriendMessageAsync(User user, int languageId,
        //    Product product, string userEmail, string friendsEmail, string personalMessage)
        //{
        //    if (user == null)
        //        throw new ArgumentNullException(nameof(user));

        //    if (product == null)
        //        throw new ArgumentNullException(nameof(product));

        //    var node = await _nodeContext.GetCurrentNodeAsync();
        //    languageId = await EnsureLanguageIsActiveAsync(languageId, node.Id);

        //    var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.EmailAFriendMessage, node.Id);
        //    if (!messageTemplates.Any())
        //        return new List<int>();

        //    //tokens
        //    var commonTokens = new List<Token>();
        //    await _messageTokenProvider.AddUserTokensAsync(commonTokens, user);
        //    await _messageTokenProvider.AddProductTokensAsync(commonTokens, product, languageId);
        //    commonTokens.Add(new Token("EmailAFriend.PersonalMessage", personalMessage, true));
        //    commonTokens.Add(new Token("EmailAFriend.Email", userEmail));

        //    return await messageTemplates.SelectAwait(async messageTemplate =>
        //    {
        //        //email account
        //        var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

        //        var tokens = new List<Token>(commonTokens);
        //        await _messageTokenProvider.AddNodeTokensAsync(tokens, node, emailAccount);

        //        //event notification
        //        await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

        //        return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, friendsEmail, string.Empty);
        //    }).ToListAsync();
        //}

        ///// <summary>
        ///// Sends wishlist "email a friend" message
        ///// </summary>
        ///// <param name="user">User</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <param name="userEmail">User's email</param>
        ///// <param name="friendsEmail">Friend's email</param>
        ///// <param name="personalMessage">Personal message</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the queued email identifier
        ///// </returns>
        //public virtual async Task<IList<int>> SendWishlistEmailAFriendMessageAsync(User user, int languageId,
        //     string userEmail, string friendsEmail, string personalMessage)
        //{
        //    if (user == null)
        //        throw new ArgumentNullException(nameof(user));

        //    var node = await _nodeContext.GetCurrentNodeAsync();
        //    languageId = await EnsureLanguageIsActiveAsync(languageId, node.Id);

        //    var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.WishlistToFriendMessage, node.Id);
        //    if (!messageTemplates.Any())
        //        return new List<int>();

        //    //tokens
        //    var commonTokens = new List<Token>();
        //    await _messageTokenProvider.AddUserTokensAsync(commonTokens, user);
        //    commonTokens.Add(new Token("Wishlist.PersonalMessage", personalMessage, true));
        //    commonTokens.Add(new Token("Wishlist.Email", userEmail));

        //    return await messageTemplates.SelectAwait(async messageTemplate =>
        //    {
        //        //email account
        //        var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

        //        var tokens = new List<Token>(commonTokens);
        //        await _messageTokenProvider.AddNodeTokensAsync(tokens, node, emailAccount);

        //        //event notification
        //        await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

        //        return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, friendsEmail, string.Empty);
        //    }).ToListAsync();
        //}

        #endregion

        #region Return requests

        ///// <summary>
        ///// Sends 'New Return Request' message to an administrator
        ///// </summary>
        ///// <param name="returnRequest">Return request</param>
        ///// <param name="orderItem">Order item</param>
        ///// <param name="order">Order</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the queued email identifier
        ///// </returns>
        //public virtual async Task<IList<int>> SendNewReturnRequestNotificationAsync(ReturnRequest returnRequest, OrderItem orderItem, Order order, int languageId)
        //{
        //    if (returnRequest == null)
        //        throw new ArgumentNullException(nameof(returnRequest));

        //    if (orderItem == null)
        //        throw new ArgumentNullException(nameof(orderItem));

        //    if (order == null)
        //        throw new ArgumentNullException(nameof(order));

        //    var node = await _clusteringService.GetNodeByIdAsync(order.NodeId) ?? await _nodeContext.GetCurrentNodeAsync();
        //    languageId = await EnsureLanguageIsActiveAsync(languageId, node.Id);

        //    var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.NewReturnRequestNotification, node.Id);
        //    if (!messageTemplates.Any())
        //        return new List<int>();

        //    //tokens
        //    var commonTokens = new List<Token>();
        //    await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
        //    await _messageTokenProvider.AddUserTokensAsync(commonTokens, returnRequest.UserId);
        //    await _messageTokenProvider.AddReturnRequestTokensAsync(commonTokens, returnRequest, orderItem, languageId);

        //    return await messageTemplates.SelectAwait(async messageTemplate =>
        //    {
        //        //email account
        //        var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

        //        var tokens = new List<Token>(commonTokens);
        //        await _messageTokenProvider.AddNodeTokensAsync(tokens, node, emailAccount);

        //        //event notification
        //        await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

        //        var (toEmail, toName) = await GetAdminNameAndEmailAsync(emailAccount);

        //        return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        //    }).ToListAsync();
        //}

        ///// <summary>
        ///// Sends 'New Return Request' message to a user
        ///// </summary>
        ///// <param name="returnRequest">Return request</param>
        ///// <param name="orderItem">Order item</param>
        ///// <param name="order">Order</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the queued email identifier
        ///// </returns>
        //public virtual async Task<IList<int>> SendNewReturnRequestUserNotificationAsync(ReturnRequest returnRequest, OrderItem orderItem, Order order)
        //{
        //    if (returnRequest == null)
        //        throw new ArgumentNullException(nameof(returnRequest));

        //    if (orderItem == null)
        //        throw new ArgumentNullException(nameof(orderItem));

        //    if (order == null)
        //        throw new ArgumentNullException(nameof(order));

        //    var node = await _clusteringService.GetNodeByIdAsync(order.NodeId) ?? await _nodeContext.GetCurrentNodeAsync();
        //    var languageId = await EnsureLanguageIsActiveAsync(order.UserLanguageId, node.Id);

        //    var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.NewReturnRequestUserNotification, node.Id);
        //    if (!messageTemplates.Any())
        //        return new List<int>();

        //    var user = await _userService.GetUserByIdAsync(returnRequest.UserId);

        //    //tokens
        //    var commonTokens = new List<Token>();
        //    await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
        //    await _messageTokenProvider.AddUserTokensAsync(commonTokens, user);
        //    await _messageTokenProvider.AddReturnRequestTokensAsync(commonTokens, returnRequest, orderItem, languageId);

        //    return await messageTemplates.SelectAwait(async messageTemplate =>
        //    {
        //        //email account
        //        var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

        //        var tokens = new List<Token>(commonTokens);
        //        await _messageTokenProvider.AddNodeTokensAsync(tokens, node, emailAccount);

        //        //event notification
        //        await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

        //        var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

        //        var toEmail = await _userService.IsGuestAsync(user)
        //            ? billingAddress.Email
        //            : user.Email;
        //        var toName = await _userService.IsGuestAsync(user)
        //            ? billingAddress.FirstName
        //            : await _userService.GetUserFullNameAsync(user);

        //        return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        //    }).ToListAsync();
        //}

        ///// <summary>
        ///// Sends 'Return Request status changed' message to a user
        ///// </summary>
        ///// <param name="returnRequest">Return request</param>
        ///// <param name="orderItem">Order item</param>
        ///// <param name="order">Order</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the queued email identifier
        ///// </returns>
        //public virtual async Task<IList<int>> SendReturnRequestStatusChangedUserNotificationAsync(ReturnRequest returnRequest, OrderItem orderItem, Order order)
        //{
        //    if (returnRequest == null)
        //        throw new ArgumentNullException(nameof(returnRequest));

        //    if (orderItem == null)
        //        throw new ArgumentNullException(nameof(orderItem));

        //    if (order == null)
        //        throw new ArgumentNullException(nameof(order));

        //    var node = await _clusteringService.GetNodeByIdAsync(order.NodeId) ?? await _nodeContext.GetCurrentNodeAsync();
        //    var languageId = await EnsureLanguageIsActiveAsync(order.UserLanguageId, node.Id);

        //    var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.ReturnRequestStatusChangedUserNotification, node.Id);
        //    if (!messageTemplates.Any())
        //        return new List<int>();

        //    var user = await _userService.GetUserByIdAsync(returnRequest.UserId);

        //    //tokens
        //    var commonTokens = new List<Token>();
        //    await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
        //    await _messageTokenProvider.AddUserTokensAsync(commonTokens, user);
        //    await _messageTokenProvider.AddReturnRequestTokensAsync(commonTokens, returnRequest, orderItem, languageId);

        //    return await messageTemplates.SelectAwait(async messageTemplate =>
        //    {
        //        //email account
        //        var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

        //        var tokens = new List<Token>(commonTokens);
        //        await _messageTokenProvider.AddNodeTokensAsync(tokens, node, emailAccount);

        //        //event notification
        //        await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

        //        var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

        //        var toEmail = await _userService.IsGuestAsync(user)
        //            ? billingAddress.Email
        //            : user.Email;
        //        var toName = await _userService.IsGuestAsync(user)
        //            ? billingAddress.FirstName
        //            : await _userService.GetUserFullNameAsync(user);

        //        return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        //    }).ToListAsync();
        //}

        #endregion

        #region Forum Notifications

        ///// <summary>
        ///// Sends a forum subscription message to a user
        ///// </summary>
        ///// <param name="user">User instance</param>
        ///// <param name="forumTopic">Forum Topic</param>
        ///// <param name="forum">Forum</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the queued email identifier
        ///// </returns>
        //public virtual async Task<IList<int>> SendNewForumTopicMessageAsync(User user, ForumTopic forumTopic, Forum forum, int languageId)
        //{
        //    if (user == null)
        //        throw new ArgumentNullException(nameof(user));

        //    var node = await _nodeContext.GetCurrentNodeAsync();

        //    var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.NewForumTopicMessage, node.Id);
        //    if (!messageTemplates.Any())
        //        return new List<int>();

        //    //tokens
        //    var commonTokens = new List<Token>();
        //    await _messageTokenProvider.AddForumTopicTokensAsync(commonTokens, forumTopic);
        //    await _messageTokenProvider.AddForumTokensAsync(commonTokens, forum);
        //    await _messageTokenProvider.AddUserTokensAsync(commonTokens, user);

        //    return await messageTemplates.SelectAwait(async messageTemplate =>
        //    {
        //        //email account
        //        var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

        //        var tokens = new List<Token>(commonTokens);
        //        await _messageTokenProvider.AddNodeTokensAsync(tokens, node, emailAccount);

        //        //event notification
        //        await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

        //        var toEmail = user.Email;
        //        var toName = await _userService.GetUserFullNameAsync(user);

        //        return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        //    }).ToListAsync();
        //}

        ///// <summary>
        ///// Sends a forum subscription message to a user
        ///// </summary>
        ///// <param name="user">User instance</param>
        ///// <param name="forumPost">Forum post</param>
        ///// <param name="forumTopic">Forum Topic</param>
        ///// <param name="forum">Forum</param>
        ///// <param name="friendlyForumTopicPageIndex">Friendly (starts with 1) forum topic page to use for URL generation</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the queued email identifier
        ///// </returns>
        //public virtual async Task<IList<int>> SendNewForumPostMessageAsync(User user, ForumPost forumPost, ForumTopic forumTopic,
        //    Forum forum, int friendlyForumTopicPageIndex, int languageId)
        //{
        //    if (user == null)
        //        throw new ArgumentNullException(nameof(user));

        //    var node = await _nodeContext.GetCurrentNodeAsync();

        //    var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.NewForumPostMessage, node.Id);
        //    if (!messageTemplates.Any())
        //        return new List<int>();

        //    //tokens
        //    var commonTokens = new List<Token>();
        //    await _messageTokenProvider.AddForumPostTokensAsync(commonTokens, forumPost);
        //    await _messageTokenProvider.AddForumTopicTokensAsync(commonTokens, forumTopic, friendlyForumTopicPageIndex, forumPost.Id);
        //    await _messageTokenProvider.AddForumTokensAsync(commonTokens, forum);
        //    await _messageTokenProvider.AddUserTokensAsync(commonTokens, user);

        //    return await messageTemplates.SelectAwait(async messageTemplate =>
        //    {
        //        //email account
        //        var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

        //        var tokens = new List<Token>(commonTokens);
        //        await _messageTokenProvider.AddNodeTokensAsync(tokens, node, emailAccount);

        //        //event notification
        //        await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

        //        var toEmail = user.Email;
        //        var toName = await _userService.GetUserFullNameAsync(user);

        //        return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        //    }).ToListAsync();
        //}

        ///// <summary>
        ///// Sends a system message notification
        ///// </summary>
        ///// <param name="systemMessage">System message</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the queued email identifier
        ///// </returns>
        //public virtual async Task<IList<int>> SendSystemMessageNotificationAsync(SystemMessage systemMessage, int languageId)
        //{
        //    ArgumentNullException.ThrowIfNull(systemMessage);

        //    var node = await _clusteringService.GetNodeByIdAsync(systemMessage.NodeId) ?? (NI2SNode)await _nodeContext.GetCurrentNodeAsync();

        //    var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.SystemMessageNotification, node.Id);
        //    if (!messageTemplates.Any())
        //        return [];

        //    //tokens
        //    var commonTokens = new List<Token>();
        //    await _messageTokenProvider.AddSystemMessageTokensAsync(commonTokens, systemMessage);
        //    await _messageTokenProvider.AddUserTokensAsync(commonTokens, systemMessage.ToUserId);

        //    return await messageTemplates.SelectAwait(async messageTemplate =>
        //    {
        //        //email account
        //        var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

        //        var tokens = new List<Token>(commonTokens);
        //        await _messageTokenProvider.AddNodeTokensAsync(tokens, node, emailAccount);

        //        //event notification
        //        await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

        //        var user = await _userService.GetUserByIdAsync(systemMessage.ToUserId);
        //        var toEmail = user.Email;
        //        var toName = await _userService.GetUserFullNameAsync(user);

        //        return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        //    }).ToListAsync();
        //}

        #endregion

        #region Misc

        ///// <summary>
        ///// Sends 'New partner account submitted' message to an administrator
        ///// </summary>
        ///// <param name="user">User</param>
        ///// <param name="partner">Partner</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the queued email identifier
        ///// </returns>
        //public virtual async Task<IList<int>> SendNewPartnerAccountApplyNotificationAsync(User user, Partner partner, int languageId)
        //{
        //    ArgumentNullException.ThrowIfNull(user);

        //    ArgumentNullException.ThrowIfNull(partner);

        //    var node = (NI2SNode)await _nodeContext.GetCurrentNodeAsync();
        //    languageId = await EnsureLanguageIsActiveAsync(languageId, node.Id);

        //    var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.NewPartnerAccountApplyNotification, node.Id);
        //    if (!messageTemplates.Any())
        //        return [];

        //    //tokens
        //    var commonTokens = new List<Token>();
        //    await _messageTokenProvider.AddUserTokensAsync(commonTokens, user);
        //    await _messageTokenProvider.AddPartnerTokensAsync(commonTokens, partner);

        //    return await messageTemplates.SelectAwait(async messageTemplate =>
        //    {
        //        //email account
        //        var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

        //        var tokens = new List<Token>(commonTokens);
        //        await _messageTokenProvider.AddNodeTokensAsync(tokens, node, emailAccount);

        //        //event notification
        //        await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

        //        var (toEmail, toName) = await GetAdminNameAndEmailAsync(emailAccount);

        //        return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        //    }).ToListAsync();
        //}

        ///// <summary>
        ///// Sends 'Partner information changed' message to an administrator
        ///// </summary>
        ///// <param name="partner">Partner</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the queued email identifier
        ///// </returns>
        //public virtual async Task<IList<int>> SendPartnerInformationChangeNotificationAsync(Partner partner, int languageId)
        //{
        //    ArgumentNullException.ThrowIfNull(partner);

        //    var node = await _nodeContext.GetCurrentNodeAsync();
        //    languageId = await EnsureLanguageIsActiveAsync(languageId, node.Id);

        //    var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.PartnerInformationChangeNotification, node.Id);
        //    if (!messageTemplates.Any())
        //        return [];

        //    //tokens
        //    var commonTokens = new List<Token>();
        //    await _messageTokenProvider.AddPartnerTokensAsync(commonTokens, partner);

        //    return await messageTemplates.SelectAwait(async messageTemplate =>
        //    {
        //        //email account
        //        var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

        //        var tokens = new List<Token>(commonTokens);
        //        await _messageTokenProvider.AddNodeTokensAsync(tokens, node, emailAccount);

        //        //event notification
        //        await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

        //        var (toEmail, toName) = await GetAdminNameAndEmailAsync(emailAccount);

        //        return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        //    }).ToListAsync();
        //}

        ///// <summary>
        ///// Sends a gift card notification
        ///// </summary>
        ///// <param name="giftCard">Gift card</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the queued email identifier
        ///// </returns>
        //public virtual async Task<IList<int>> SendGiftCardNotificationAsync(GiftCard giftCard, int languageId)
        //{
        //    if (giftCard == null)
        //        throw new ArgumentNullException(nameof(giftCard));

        //    var order = await _orderService.GetOrderByOrderItemAsync(giftCard.PurchasedWithOrderItemId ?? 0);
        //    var currentNode = await _nodeContext.GetCurrentNodeAsync();
        //    var node = order != null ? await _clusteringService.GetNodeByIdAsync(order.NodeId) ?? currentNode : currentNode;

        //    languageId = await EnsureLanguageIsActiveAsync(languageId, node.Id);

        //    var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.GiftCardNotification, node.Id);
        //    if (!messageTemplates.Any())
        //        return new List<int>();

        //    //tokens
        //    var commonTokens = new List<Token>();
        //    await _messageTokenProvider.AddGiftCardTokensAsync(commonTokens, giftCard, languageId);

        //    return await messageTemplates.SelectAwait(async messageTemplate =>
        //    {
        //        //email account
        //        var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

        //        var tokens = new List<Token>(commonTokens);
        //        await _messageTokenProvider.AddNodeTokensAsync(tokens, node, emailAccount);

        //        //event notification
        //        await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

        //        var toEmail = giftCard.RecipientEmail;
        //        var toName = giftCard.RecipientName;

        //        return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        //    }).ToListAsync();
        //}

        ///// <summary>
        ///// Sends a product review notification message to an administrator
        ///// </summary>
        ///// <param name="productReview">Product review</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the queued email identifier
        ///// </returns>
        //public virtual async Task<IList<int>> SendProductReviewNotificationMessageAsync(ProductReview productReview, int languageId)
        //{
        //    if (productReview == null)
        //        throw new ArgumentNullException(nameof(productReview));

        //    var node = await _nodeContext.GetCurrentNodeAsync();
        //    languageId = await EnsureLanguageIsActiveAsync(languageId, node.Id);

        //    var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.ProductReviewNotification, node.Id);
        //    if (!messageTemplates.Any())
        //        return new List<int>();

        //    //tokens
        //    var commonTokens = new List<Token>();
        //    await _messageTokenProvider.AddProductReviewTokensAsync(commonTokens, productReview);
        //    await _messageTokenProvider.AddUserTokensAsync(commonTokens, productReview.UserId);

        //    return await messageTemplates.SelectAwait(async messageTemplate =>
        //    {
        //        //email account
        //        var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

        //        var tokens = new List<Token>(commonTokens);
        //        await _messageTokenProvider.AddNodeTokensAsync(tokens, node, emailAccount);

        //        //event notification
        //        await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

        //        var (toEmail, toName) = await GetAdminNameAndEmailAsync(emailAccount);

        //        return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        //    }).ToListAsync();
        //}

        ///// <summary>
        ///// Sends a product review reply notification message to a user
        ///// </summary>
        ///// <param name="productReview">Product review</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the queued email identifier
        ///// </returns>
        //public virtual async Task<IList<int>> SendProductReviewReplyUserNotificationMessageAsync(ProductReview productReview, int languageId)
        //{
        //    if (productReview == null)
        //        throw new ArgumentNullException(nameof(productReview));

        //    var node = await _clusteringService.GetNodeByIdAsync(productReview.NodeId) ?? await _nodeContext.GetCurrentNodeAsync();
        //    languageId = await EnsureLanguageIsActiveAsync(languageId, node.Id);

        //    var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.ProductReviewReplyUserNotification, node.Id);
        //    if (!messageTemplates.Any())
        //        return new List<int>();

        //    var user = await _userService.GetUserByIdAsync(productReview.UserId);

        //    //We should not send notifications to guests
        //    if (await _userService.IsGuestAsync(user))
        //        return new List<int>();

        //    //We should not send notifications to guests
        //    if (await _userService.IsGuestAsync(user))
        //        return new List<int>();

        //    //tokens
        //    var commonTokens = new List<Token>();
        //    await _messageTokenProvider.AddProductReviewTokensAsync(commonTokens, productReview);
        //    await _messageTokenProvider.AddUserTokensAsync(commonTokens, user);

        //    return await messageTemplates.SelectAwait(async messageTemplate =>
        //    {
        //        //email account
        //        var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

        //        var tokens = new List<Token>(commonTokens);
        //        await _messageTokenProvider.AddNodeTokensAsync(tokens, node, emailAccount);

        //        //event notification
        //        await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

        //        var toEmail = user.Email;
        //        var toName = await _userService.GetUserFullNameAsync(user);

        //        return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        //    }).ToListAsync();
        //}

        ///// <summary>
        ///// Sends a "quantity below" notification to an administrator
        ///// </summary>
        ///// <param name="product">Product</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the queued email identifier
        ///// </returns>
        //public virtual async Task<IList<int>> SendQuantityBelowNotificationAsync(Product product, int languageId)
        //{
        //    if (product == null)
        //        throw new ArgumentNullException(nameof(product));

        //    var node = await _nodeContext.GetCurrentNodeAsync();
        //    languageId = await EnsureLanguageIsActiveAsync(languageId, node.Id);

        //    var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.QuantityBelowNotification, node.Id);
        //    if (!messageTemplates.Any())
        //        return new List<int>();

        //    var commonTokens = new List<Token>();
        //    await _messageTokenProvider.AddProductTokensAsync(commonTokens, product, languageId);

        //    return await messageTemplates.SelectAwait(async messageTemplate =>
        //    {
        //        //email account
        //        var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

        //        var tokens = new List<Token>(commonTokens);
        //        await _messageTokenProvider.AddNodeTokensAsync(tokens, node, emailAccount);

        //        //event notification
        //        await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

        //        var (toEmail, toName) = await GetAdminNameAndEmailAsync(emailAccount);

        //        return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        //    }).ToListAsync();
        //}

        ///// <summary>
        ///// Sends a "quantity below" notification to an administrator
        ///// </summary>
        ///// <param name="combination">Attribute combination</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the queued email identifier
        ///// </returns>
        //public virtual async Task<IList<int>> SendQuantityBelowNotificationAsync(ProductAttributeCombination combination, int languageId)
        //{
        //    if (combination == null)
        //        throw new ArgumentNullException(nameof(combination));

        //    var node = await _nodeContext.GetCurrentNodeAsync();
        //    languageId = await EnsureLanguageIsActiveAsync(languageId, node.Id);

        //    var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.QuantityBelowAttributeCombinationNotification, node.Id);
        //    if (!messageTemplates.Any())
        //        return new List<int>();

        //    var commonTokens = new List<Token>();
        //    var product = await _productService.GetProductByIdAsync(combination.ProductId);

        //    await _messageTokenProvider.AddProductTokensAsync(commonTokens, product, languageId);
        //    await _messageTokenProvider.AddAttributeCombinationTokensAsync(commonTokens, combination, languageId);

        //    return await messageTemplates.SelectAwait(async messageTemplate =>
        //    {
        //        //email account
        //        var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

        //        var tokens = new List<Token>(commonTokens);
        //        await _messageTokenProvider.AddNodeTokensAsync(tokens, node, emailAccount);

        //        //event notification
        //        await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

        //        var (toEmail, toName) = await GetAdminNameAndEmailAsync(emailAccount);

        //        return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        //    }).ToListAsync();
        //}

        /// <summary>
        /// Sends a "new VAT submitted" notification to an administrator
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="vatName">Received VAT name</param>
        /// <param name="vatAddress">Received VAT address</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the queued email identifier
        /// </returns>
        public virtual async Task<IList<int>> SendNewVatSubmittedNotificationAsync(User user,
            string vatName, string vatAddress, int languageId)
        {
            ArgumentNullException.ThrowIfNull(user);

            var node = (NI2SNode)await _nodeContext.GetCurrentNodeAsync();
            languageId = await EnsureLanguageIsActiveAsync(languageId, node.Id);

            var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.NewVatSubmittedNotification, node.Id);
            if (!messageTemplates.Any())
                return [];

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddUserTokensAsync(commonTokens, user);
            commonTokens.Add(new Token("VatValidatio.Name", vatName));
            commonTokens.Add(new Token("VatValidatio.Address", vatAddress));

            return await messageTemplates.SelectAwait(async messageTemplate =>
            {
                //email account
                var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                await _messageTokenProvider.AddNodeTokensAsync(tokens, node, emailAccount);

                await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

                var (toEmail, toName) = await GetAdminNameAndEmailAsync(emailAccount);

                return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToListAsync();
        }

        /// <summary>
        /// Sends a blog comment notification message to an administrator
        /// </summary>
        /// <param name="blogComment">Blog comment</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of queued email identifiers
        /// </returns>
        public virtual async Task<IList<int>> SendBlogCommentNotificationMessageAsync(BlogComment blogComment, int languageId)
        {
            ArgumentNullException.ThrowIfNull(blogComment);

            var node = (NI2SNode)await _nodeContext.GetCurrentNodeAsync();
            languageId = await EnsureLanguageIsActiveAsync(languageId, node.Id);

            var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.BlogCommentNotification, node.Id);
            if (!messageTemplates.Any())
                return [];

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddBlogCommentTokensAsync(commonTokens, blogComment);
            await _messageTokenProvider.AddUserTokensAsync(commonTokens, blogComment.UserId);

            return await messageTemplates.SelectAwait(async messageTemplate =>
            {
                //email account
                var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                await _messageTokenProvider.AddNodeTokensAsync(tokens, node, emailAccount);

                //event notification
                await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

                var (toEmail, toName) = await GetAdminNameAndEmailAsync(emailAccount);

                return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToListAsync();
        }

        /// <summary>
        /// Sends a news comment notification message to an administrator
        /// </summary>
        /// <param name="newsComment">News comment</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the queued email identifier
        /// </returns>
        public virtual async Task<IList<int>> SendNewsCommentNotificationMessageAsync(NewsComment newsComment, int languageId)
        {
            ArgumentNullException.ThrowIfNull(newsComment);

            var node = (NI2SNode)await _nodeContext.GetCurrentNodeAsync();
            languageId = await EnsureLanguageIsActiveAsync(languageId, node.Id);

            var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.NewsCommentNotification, node.Id);
            if (!messageTemplates.Any())
                return [];

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddNewsCommentTokensAsync(commonTokens, newsComment);
            await _messageTokenProvider.AddUserTokensAsync(commonTokens, newsComment.UserId);

            return await messageTemplates.SelectAwait(async messageTemplate =>
            {
                //email account
                var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                await _messageTokenProvider.AddNodeTokensAsync(tokens, node, emailAccount);

                //event notification
                await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

                var (toEmail, toName) = await GetAdminNameAndEmailAsync(emailAccount);

                return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToListAsync();
        }

        ///// <summary>
        ///// Sends a 'Back in stock' notification message to a user
        ///// </summary>
        ///// <param name="subscription">Subscription</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the queued email identifier
        ///// </returns>
        //public virtual async Task<IList<int>> SendBackInStockNotificationAsync(BackInStockSubscription subscription, int languageId)
        //{
        //    if (subscription == null)
        //        throw new ArgumentNullException(nameof(subscription));

        //    var user = await _userService.GetUserByIdAsync(subscription.UserId);

        //    if (user == null)
        //        throw new ArgumentNullException(nameof(user));

        //    //ensure that user is registered (simple and fast way)
        //    if (!CommonHelper.IsValidEmail(user.Email))
        //        return new List<int>();

        //    var node = await _clusteringService.GetNodeByIdAsync(subscription.NodeId) ?? await _nodeContext.GetCurrentNodeAsync();
        //    languageId = await EnsureLanguageIsActiveAsync(languageId, node.Id);

        //    var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.BackInStockNotification, node.Id);
        //    if (!messageTemplates.Any())
        //        return new List<int>();

        //    //tokens
        //    var commonTokens = new List<Token>();
        //    await _messageTokenProvider.AddUserTokensAsync(commonTokens, user);
        //    await _messageTokenProvider.AddBackInStockTokensAsync(commonTokens, subscription);

        //    return await messageTemplates.SelectAwait(async messageTemplate =>
        //    {
        //        //email account
        //        var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

        //        var tokens = new List<Token>(commonTokens);
        //        await _messageTokenProvider.AddNodeTokensAsync(tokens, node, emailAccount);

        //        //event notification
        //        await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

        //        var toEmail = user.Email;
        //        var toName = await _userService.GetUserFullNameAsync(user);

        //        return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        //    }).ToListAsync();
        //}

        /// <summary>
        /// Sends "contact us" message
        /// </summary>
        /// <param name="languageId">Message language identifier</param>
        /// <param name="senderEmail">Sender email</param>
        /// <param name="senderName">Sender name</param>
        /// <param name="subject">Email subject. Pass null if you want a message template subject to be used.</param>
        /// <param name="body">Email body</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the queued email identifier
        /// </returns>
        public virtual async Task<IList<int>> SendContactUsMessageAsync(int languageId, string senderEmail,
            string senderName, string subject, string body)
        {
            var node = (NI2SNode)await _nodeContext.GetCurrentNodeAsync();
            languageId = await EnsureLanguageIsActiveAsync(languageId, node.Id);

            var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.ContactUsMessage, node.Id);
            if (!messageTemplates.Any())
                return [];

            //tokens
            var commonTokens = new List<Token>
            {
                new("ContactUs.SenderEmail", senderEmail),
                new("ContactUs.SenderName", senderName)
            };

            return await messageTemplates.SelectAwait(async messageTemplate =>
            {
                //email account
                var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                await _messageTokenProvider.AddNodeTokensAsync(tokens, node, emailAccount);

                string fromEmail;
                string fromName;
                //required for some SMTP nodes
                if (_commonSettings.UseSystemEmailForContactUsForm)
                {
                    fromEmail = emailAccount.Email;
                    fromName = emailAccount.DisplayName;
                    body = $"<strong>From</strong>: {WebUtility.HtmlEncode(senderName)} - {WebUtility.HtmlEncode(senderEmail)}<br /><br />{body}";
                }
                else
                {
                    fromEmail = senderEmail;
                    fromName = senderName;
                }

                tokens.Add(new Token("ContactUs.Body", body, true));

                //event notification
                await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName,
                    fromEmail: fromEmail,
                    fromName: fromName,
                    subject: subject,
                    replyToEmailAddress: senderEmail,
                    replyToName: senderName);
            }).ToListAsync();
        }

        ///// <summary>
        ///// Sends "contact partner" message
        ///// </summary>
        ///// <param name="partner">Partner</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <param name="senderEmail">Sender email</param>
        ///// <param name="senderName">Sender name</param>
        ///// <param name="subject">Email subject. Pass null if you want a message template subject to be used.</param>
        ///// <param name="body">Email body</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the queued email identifier
        ///// </returns>
        //public virtual async Task<IList<int>> SendContactPartnerMessageAsync(Partner partner, int languageId, string senderEmail,
        //    string senderName, string subject, string body)
        //{
        //    ArgumentNullException.ThrowIfNull(partner);

        //    var node = await _nodeContext.GetCurrentNodeAsync();
        //    languageId = await EnsureLanguageIsActiveAsync(languageId, node.Id);

        //    var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.ContactPartnerMessage, node.Id);
        //    if (!messageTemplates.Any())
        //        return [];

        //    //tokens
        //    var commonTokens = new List<Token>
        //    {
        //        new("ContactUs.SenderEmail", senderEmail),
        //        new("ContactUs.SenderName", senderName),
        //        new("ContactUs.Body", body, true)
        //    };

        //    return await messageTemplates.SelectAwait(async messageTemplate =>
        //    {
        //        //email account
        //        var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

        //        string fromEmail;
        //        string fromName;
        //        //required for some SMTP nodes
        //        if (_commonSettings.UseSystemEmailForContactUsForm)
        //        {
        //            fromEmail = emailAccount.Email;
        //            fromName = emailAccount.DisplayName;
        //            body = $"<strong>From</strong>: {WebUtility.HtmlEncode(senderName)} - {WebUtility.HtmlEncode(senderEmail)}<br /><br />{body}";
        //        }
        //        else
        //        {
        //            fromEmail = senderEmail;
        //            fromName = senderName;
        //        }

        //        var tokens = new List<Token>(commonTokens);
        //        await _messageTokenProvider.AddNodeTokensAsync(tokens, node, emailAccount);

        //        //event notification
        //        await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

        //        var toEmail = partner.Email;
        //        var toName = partner.Name;

        //        return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName,
        //            fromEmail: fromEmail,
        //            fromName: fromName,
        //            subject: subject,
        //            replyToEmailAddress: senderEmail,
        //            replyToName: senderName);
        //    }).ToListAsync();
        //}

        /// <summary>
        /// Sends a test email
        /// </summary>
        /// <param name="messageTemplateId">Message template identifier</param>
        /// <param name="sendToEmail">Send to email</param>
        /// <param name="tokens">Tokens</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the queued email identifier
        /// </returns>
        public virtual async Task<int> SendTestEmailAsync(int messageTemplateId, string sendToEmail, List<Token> tokens, int languageId)
        {
            var messageTemplate = await _messageTemplateService.GetMessageTemplateByIdAsync(messageTemplateId) ?? throw new ArgumentException("Template cannot be loaded");

            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            //force sending
            messageTemplate.DelayBeforeSend = null;

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, sendToEmail, null);
        }

        #endregion

        #region Common

        /// <summary>
        /// Send notification
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="emailAccount">Email account</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="tokens">Tokens</param>
        /// <param name="toEmailAddress">Recipient email address</param>
        /// <param name="toName">Recipient name</param>
        /// <param name="attachmentFilePath">Attachment file path</param>
        /// <param name="attachmentFileName">Attachment file name</param>
        /// <param name="replyToEmailAddress">"Reply to" email</param>
        /// <param name="replyToName">"Reply to" name</param>
        /// <param name="fromEmail">Sender email. If specified, then it overrides passed "emailAccount" details</param>
        /// <param name="fromName">Sender name. If specified, then it overrides passed "emailAccount" details</param>
        /// <param name="subject">Subject. If specified, then it overrides subject of a message template</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the queued email identifier
        /// </returns>
        public virtual async Task<int> SendNotificationAsync(MessageTemplate messageTemplate,
            EmailAccount emailAccount, int languageId, IList<Token> tokens,
            string toEmailAddress, string toName,
            string attachmentFilePath = null, string attachmentFileName = null,
            string replyToEmailAddress = null, string replyToName = null,
            string fromEmail = null, string fromName = null, string subject = null)
        {
            ArgumentNullException.ThrowIfNull(messageTemplate);

            ArgumentNullException.ThrowIfNull(emailAccount);

            //retrieve localized message template data
            var bcc = await _localizationService.GetLocalizedAsync(messageTemplate, mt => mt.BccEmailAddresses, languageId);
            if (string.IsNullOrEmpty(subject))
                subject = await _localizationService.GetLocalizedAsync(messageTemplate, mt => mt.Subject, languageId);
            var body = await _localizationService.GetLocalizedAsync(messageTemplate, mt => mt.Body, languageId);

            //Replace subject and body tokens 
            var subjectReplaced = _tokenizer.Replace(subject, tokens, false);
            var bodyReplaced = _tokenizer.Replace(body, tokens, true);

            //limit name length
            toName = CommonHelper.EnsureMaximumLength(toName, 300);

            var email = new QueuedEmail
            {
                Priority = QueuedEmailPriority.High,
                From = !string.IsNullOrEmpty(fromEmail) ? fromEmail : emailAccount.Email,
                FromName = !string.IsNullOrEmpty(fromName) ? fromName : emailAccount.DisplayName,
                To = toEmailAddress,
                ToName = toName,
                ReplyTo = replyToEmailAddress,
                ReplyToName = replyToName,
                CC = string.Empty,
                Bcc = bcc,
                Subject = subjectReplaced,
                Body = bodyReplaced,
                AttachmentFilePath = attachmentFilePath,
                AttachmentFileName = attachmentFileName,
                AttachedDownloadId = messageTemplate.AttachedDownloadId,
                CreatedOnUtc = DateTime.UtcNow,
                EmailAccountId = emailAccount.Id,
                DontSendBeforeDateUtc = !messageTemplate.DelayBeforeSend.HasValue ? null
                    : (DateTime?)(DateTime.UtcNow + TimeSpan.FromHours(messageTemplate.DelayPeriod.ToHours(messageTemplate.DelayBeforeSend.Value)))
            };

            await _queuedEmailService.InsertQueuedEmailAsync(email);
            return email.Id;
        }

        #endregion

        #endregion
    }
}