using ARWNI2S.Node.Core;
using ARWNI2S.Node.Data;
using ARWNI2S.Node.Core.Entities.Clustering;
using ARWNI2S.Portal.Services.Entities.Mailing;
using ARWNI2S.Portal.Services.Users;

namespace ARWNI2S.Portal.Services.Mailing
{
    /// <summary>
    /// Campaign service
    /// </summary>
    public partial class CampaignService : ICampaignService
    {
        #region Fields

        private readonly PortalUserService _userService;
        private readonly IEmailSender _emailSender;
        private readonly IMessageTokenProvider _messageTokenProvider;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly IRepository<Campaign> _campaignRepository;
        private readonly INodeContext _nodeContext;
        private readonly ITokenizer _tokenizer;

        #endregion

        #region Ctor

        public CampaignService(PortalUserService userService,
            IEmailSender emailSender,
            IMessageTokenProvider messageTokenProvider,
            IQueuedEmailService queuedEmailService,
            IRepository<Campaign> campaignRepository,
            INodeContext nodeContext,
            ITokenizer tokenizer)
        {
            _userService = userService;
            _emailSender = emailSender;
            _messageTokenProvider = messageTokenProvider;
            _queuedEmailService = queuedEmailService;
            _campaignRepository = campaignRepository;
            _nodeContext = nodeContext;
            _tokenizer = tokenizer;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Inserts a campaign
        /// </summary>
        /// <param name="campaign">Campaign</param>        
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertCampaignAsync(Campaign campaign)
        {
            await _campaignRepository.InsertAsync(campaign);
        }

        /// <summary>
        /// Updates a campaign
        /// </summary>
        /// <param name="campaign">Campaign</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateCampaignAsync(Campaign campaign)
        {
            await _campaignRepository.UpdateAsync(campaign);
        }

        /// <summary>
        /// Deleted a queued email
        /// </summary>
        /// <param name="campaign">Campaign</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteCampaignAsync(Campaign campaign)
        {
            await _campaignRepository.DeleteAsync(campaign);
        }

        /// <summary>
        /// Gets a campaign by identifier
        /// </summary>
        /// <param name="campaignId">Campaign identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the campaign
        /// </returns>
        public virtual async Task<Campaign> GetCampaignByIdAsync(int campaignId)
        {
            return await _campaignRepository.GetByIdAsync(campaignId, cache => default);
        }

        /// <summary>
        /// Gets all campaigns
        /// </summary>
        /// <param name="nodeId">Node identifier; 0 to load all records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the campaigns
        /// </returns>
        public virtual async Task<IList<Campaign>> GetAllCampaignsAsync(int nodeId = 0)
        {
            var campaigns = await _campaignRepository.GetAllAsync(query =>
            {
                if (nodeId > 0)
                    query = query.Where(c => c.NodeId == nodeId);

                query = query.OrderBy(c => c.CreatedOnUtc);

                return query;
            });

            return campaigns;
        }

        /// <summary>
        /// Sends a campaign to specified emails
        /// </summary>
        /// <param name="campaign">Campaign</param>
        /// <param name="emailAccount">Email account</param>
        /// <param name="subscriptions">Subscriptions</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the otal emails sent
        /// </returns>
        public virtual async Task<int> SendCampaignAsync(Campaign campaign, EmailAccount emailAccount,
            IEnumerable<NewsLetterSubscription> subscriptions)
        {
            ArgumentNullException.ThrowIfNull(campaign);

            ArgumentNullException.ThrowIfNull(emailAccount);

            var totalEmailsSent = 0;

            foreach (var subscription in subscriptions)
            {
                var user = await _userService.GetUserByEmailAsync(subscription.Email);
                //ignore deleted or inactive users when sending newsletter campaigns
                if (user != null && (!user.Active || user.Deleted))
                    continue;

                var tokens = new List<Token>();
                await _messageTokenProvider.AddNodeTokensAsync(tokens, (NI2SNode)await _nodeContext.GetCurrentNodeAsync(), emailAccount);
                await _messageTokenProvider.AddNewsLetterSubscriptionTokensAsync(tokens, subscription);
                if (user != null)
                    await _messageTokenProvider.AddUserTokensAsync(tokens, user);

                var subject = _tokenizer.Replace(campaign.Subject, tokens, false);
                var body = _tokenizer.Replace(campaign.Body, tokens, true);

                var email = new QueuedEmail
                {
                    Priority = QueuedEmailPriority.Low,
                    From = emailAccount.Email,
                    FromName = emailAccount.DisplayName,
                    To = subscription.Email,
                    Subject = subject,
                    Body = body,
                    CreatedOnUtc = DateTime.UtcNow,
                    EmailAccountId = emailAccount.Id,
                    DontSendBeforeDateUtc = campaign.DontSendBeforeDateUtc
                };
                await _queuedEmailService.InsertQueuedEmailAsync(email);
                totalEmailsSent++;
            }

            return totalEmailsSent;
        }

        /// <summary>
        /// Sends a campaign to specified email
        /// </summary>
        /// <param name="campaign">Campaign</param>
        /// <param name="emailAccount">Email account</param>
        /// <param name="email">Email</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task SendCampaignAsync(Campaign campaign, EmailAccount emailAccount, string email)
        {
            ArgumentNullException.ThrowIfNull(campaign);

            ArgumentNullException.ThrowIfNull(emailAccount);

            var tokens = new List<Token>();
            await _messageTokenProvider.AddNodeTokensAsync(tokens, await _nodeContext.GetCurrentNodeAsync(), emailAccount);
            var user = await _userService.GetUserByEmailAsync(email);
            if (user != null)
                await _messageTokenProvider.AddUserTokensAsync(tokens, user);

            var subject = _tokenizer.Replace(campaign.Subject, tokens, false);
            var body = _tokenizer.Replace(campaign.Body, tokens, true);

            await _emailSender.SendEmailAsync(emailAccount, subject, body, emailAccount.Email, emailAccount.DisplayName, email, null);
        }

        #endregion
    }
}