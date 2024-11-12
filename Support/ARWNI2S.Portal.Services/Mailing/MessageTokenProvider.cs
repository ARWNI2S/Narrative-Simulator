using ARWNI2S.Node.Core;
using ARWNI2S.Node.Core.Entities.Clustering;
using ARWNI2S.Node.Core.Entities.Users;
using ARWNI2S.Node.Core.Events;
using ARWNI2S.Node.Services.Clustering;
using ARWNI2S.Node.Services.Common;
using ARWNI2S.Node.Services.Localization;
using ARWNI2S.Portal.Services.Blogs;
using ARWNI2S.Portal.Services.Clustering;
using ARWNI2S.Portal.Services.Entities;
using ARWNI2S.Portal.Services.Entities.Blogs;
using ARWNI2S.Portal.Services.Entities.Mailing;
using ARWNI2S.Portal.Services.Entities.News;
using ARWNI2S.Portal.Services.Entities.Tax;
using ARWNI2S.Portal.Services.News;
using ARWNI2S.Portal.Services.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace ARWNI2S.Portal.Services.Mailing
{
    /// <summary>
    /// Message token provider
    /// </summary>
    public partial class MessageTokenProvider : IMessageTokenProvider
    {
        #region Fields

        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IBlogService _blogService;
        private readonly IUserAttributeFormatter _userAttributeFormatter;
        private readonly PortalUserService _userService;
        private readonly INodeEventPublisher _eventPublisher;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly INewsService _newsService;
        private readonly IClusterContext _nodeContext;
        private readonly IClusteringService _clusteringService;
        private readonly IUrlHelperFactory _urlHelperFactory;
        //private readonly IPartnerAttributeFormatter _partnerAttributeFormatter;
        private readonly PortalInfoSettings _nodeInformationSettings;

        private Dictionary<string, IEnumerable<string>> _allowedTokens;

        #endregion

        #region Ctor

        public MessageTokenProvider(
            IActionContextAccessor actionContextAccessor,
            IBlogService blogService,
            IUserAttributeFormatter userAttributeFormatter,
            PortalUserService userService,
            INodeEventPublisher eventPublisher,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            INewsService newsService,
            IClusterContext nodeContext,
            IClusteringService clusteringService,
            IUrlHelperFactory urlHelperFactory,
            //IPartnerAttributeFormatter partnerAttributeFormatter,
            PortalInfoSettings nodeInformationSettings
            )
        {
            _actionContextAccessor = actionContextAccessor;
            _blogService = blogService;
            _userAttributeFormatter = userAttributeFormatter;
            _userService = userService;
            _eventPublisher = eventPublisher;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _newsService = newsService;
            _nodeContext = nodeContext;
            _clusteringService = clusteringService;
            _urlHelperFactory = urlHelperFactory;
            //_partnerAttributeFormatter = partnerAttributeFormatter;
            _nodeInformationSettings = nodeInformationSettings;
        }

        #endregion

        #region Allowed tokens

        /// <summary>
        /// Get all available tokens by token groups
        /// </summary>
        protected Dictionary<string, IEnumerable<string>> AllowedTokens
        {
            get
            {
                if (_allowedTokens != null)
                    return _allowedTokens;

                _allowedTokens = new Dictionary<string, IEnumerable<string>>
                {
                    //node tokens
                    {
                        TokenGroupNames.NodeTokens,
                        NodeTokens
                    },
                    //user tokens
                    {
                        TokenGroupNames.UserTokens,
                        UserTokens
                    },
                    //newsletter subscription tokens
                    {
                        TokenGroupNames.SubscriptionTokens,
                        SubscriptionTokens
                    },
                    //partner tokens
                    {
                        TokenGroupNames.PartnerTokens,
                        PartnerTokens
                    },
                    //blog comment tokens
                    {
                        TokenGroupNames.BlogCommentTokens,
                        BlogCommentTokens
                    },
                    //news comment tokens
                    {
                        TokenGroupNames.NewsCommentTokens,
                        NewsCommentTokens
                    },
                    //email a friend tokens
                    {
                        TokenGroupNames.EmailAFriendTokens,
                        EmailAFriendTokens
                    },
                    //VAT validation tokens
                    {
                        TokenGroupNames.VatValidation,
                        VatValidationTokens
                    },
                    //contact us tokens
                    {
                        TokenGroupNames.ContactUs,
                        ContactUsTokens
                    },
                    //contact partner tokens
                    {
                        TokenGroupNames.ContactPartner,
                        ContactPartnerTokens
                    }
                };

                return _allowedTokens;
            }
        }


        private static readonly string[] NodeTokens = [
            "%Node.Name%",
            "%Node.URL%",
            "%Node.Email%",
            "%Node.CompanyName%",
            "%Node.CompanyAddress%",
            "%Node.CompanyPhoneNumber%",
            "%Node.CompanyVat%",
            "%Facebook.URL%",
            "%DiscordInvite.URL%",
            "%Twitter.URL%",
            "%YouTube.URL%",
            "%Instagram.URL%"
        ];

        private static readonly string[] UserTokens = [
            "%User.Email%",
            "%User.Username%",
            "%User.FullName%",
            "%User.FirstName%",
            "%User.LastName%",
            "%User.VatNumber%",
            "%User.VatNumberStatus%",
            "%User.CustomAttributes%",
            "%User.PasswordRecoveryURL%",
            "%User.AccountActivationURL%",
            "%User.EmailRevalidationURL%",
            "%Wishlist.URLForUser%"
        ];

        private static readonly string[] SubscriptionTokens = [
            "%NewsLetterSubscription.Email%",
            "%NewsLetterSubscription.ActivationUrl%",
            "%NewsLetterSubscription.DeactivationUrl%"
        ];

        private static readonly string[] PartnerTokens = [
            "%Partner.Name%",
            "%Partner.Email%",
            "%Partner.PartnerAttributes%"
        ];

        private static readonly string[] BlogCommentTokens = [
            "%BlogComment.BlogPostTitle%"
        ];

        private static readonly string[] NewsCommentTokens = [
            "%NewsComment.NewsTitle%"
        ];

        private static readonly string[] EmailAFriendTokens = [
            "%EmailAFriend.PersonalMessage%",
            "%EmailAFriend.Email%"
        ];

        private static readonly string[] VatValidationTokens = [
            "%VatValidationResult.Name%",
            "%VatValidationResult.Address%"
        ];

        private static readonly string[] ContactUsTokens = [
            "%ContactUs.SenderEmail%",
            "%ContactUs.SenderName%",
            "%ContactUs.Body%"
        ];

        private static readonly string[] ContactPartnerTokens = [
            "%ContactUs.SenderEmail%",
            "%ContactUs.SenderName%",
            "%ContactUs.Body%"
        ];

        #endregion

        #region Utilities

        /// <summary>
        /// Generates an absolute URL for the specified node, routeName and route values
        /// </summary>
        /// <param name="nodeId">Node identifier; Pass 0 to load URL of the current node</param>
        /// <param name="routeName">The name of the route that is used to generate URL</param>
        /// <param name="routeValues">An object that contains route values</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the generated URL
        /// </returns>
        protected virtual async Task<string> RouteUrlAsync(int nodeId = 0, string routeName = null, object routeValues = null)
        {
            //try to get a node by the passed identifier
            var node = await _clusteringService.GetNodeByIdAsync(nodeId) ?? await _nodeContext.GetCurrentNodeAsync()
                ?? throw new NodeException("No node could be loaded");

            //ensure that the node URL is specified
            if (string.IsNullOrEmpty(node.GetUrl()))
                throw new NodeException("URL cannot be null");

            //generate the relative URL
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            var url = urlHelper.RouteUrl(routeName, routeValues);

            //compose the result
            return new Uri(new Uri(node.GetUrl()), url).AbsoluteUri;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add node tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="node">Node</param>
        /// <param name="emailAccount">Email account</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task AddNodeTokensAsync(IList<Token> tokens, ClusterNode node, EmailAccount emailAccount)
        {
            ArgumentNullException.ThrowIfNull(emailAccount);

            //tokens.Add(new Token("Node.Name", await _localizationService.GetLocalizedAsync(node, x => x.Name)));
            //tokens.Add(new Token("Node.URL", node.Url, true));
            //tokens.Add(new Token("Node.Email", emailAccount.Email));
            //tokens.Add(new Token("Node.CompanyName", node.CompanyName));
            //tokens.Add(new Token("Node.CompanyAddress", node.CompanyAddress));
            //tokens.Add(new Token("Node.CompanyPhoneNumber", node.CompanyPhoneNumber));
            //tokens.Add(new Token("Node.CompanyVat", node.CompanyVat));

            tokens.Add(new Token("Facebook.URL", _nodeInformationSettings.FacebookLink));
            tokens.Add(new Token("DiscordInvite.URL", _nodeInformationSettings.DiscordInviteLink));
            tokens.Add(new Token("Twitter.URL", _nodeInformationSettings.TwitterLink));
            tokens.Add(new Token("YouTube.URL", _nodeInformationSettings.YouTubeLink));
            tokens.Add(new Token("Instagram.URL", _nodeInformationSettings.InstagramLink));

            //event notification
            await _eventPublisher.EntityTokensAddedAsync(node, tokens);
        }

        /// <summary>
        /// Add user tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="userId">User identifier</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task AddUserTokensAsync(IList<Token> tokens, int userId)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(userId);

            var user = await _userService.GetUserByIdAsync(userId);

            await AddUserTokensAsync(tokens, user);
        }

        /// <summary>
        /// Add user tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="user">User</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task AddUserTokensAsync(IList<Token> tokens, User user)
        {
            tokens.Add(new Token("User.Email", user.Email));
            tokens.Add(new Token("User.Username", user.Username));
            tokens.Add(new Token("User.FullName", await _userService.GetUserFullNameAsync(user)));
            tokens.Add(new Token("User.FirstName", user.FirstName));
            tokens.Add(new Token("User.LastName", user.LastName));
            tokens.Add(new Token("User.VatNumber", user.VatNumber));
            tokens.Add(new Token("User.VatNumberStatus", ((VatNumberStatus)user.VatNumberStatusId).ToString()));

            var customAttributesXml = user.CustomUserAttributesXML;
            tokens.Add(new Token("User.CustomAttributes", await _userAttributeFormatter.FormatAttributesAsync(customAttributesXml), true));

            //note: we do not use SEO friendly URLS for these links because we can get errors caused by having .(dot) in the URL (from the email address)
            var passwordRecoveryUrl = await RouteUrlAsync(routeName: "PasswordRecoveryConfirm", routeValues: new { token = await _genericAttributeService.GetAttributeAsync<string>(user, UserDefaults.PasswordRecoveryTokenAttribute), guid = user.UserGuid });
            var accountActivationUrl = await RouteUrlAsync(routeName: "AccountActivation", routeValues: new { token = await _genericAttributeService.GetAttributeAsync<string>(user, UserDefaults.AccountActivationTokenAttribute), guid = user.UserGuid });
            var emailRevalidationUrl = await RouteUrlAsync(routeName: "EmailRevalidation", routeValues: new { token = await _genericAttributeService.GetAttributeAsync<string>(user, UserDefaults.EmailRevalidationTokenAttribute), guid = user.UserGuid });
            var wishlistUrl = await RouteUrlAsync(routeName: "Wishlist", routeValues: new { userGuid = user.UserGuid });
            tokens.Add(new Token("User.PasswordRecoveryURL", passwordRecoveryUrl, true));
            tokens.Add(new Token("User.AccountActivationURL", accountActivationUrl, true));
            tokens.Add(new Token("User.EmailRevalidationURL", emailRevalidationUrl, true));
            tokens.Add(new Token("Wishlist.URLForUser", wishlistUrl, true));

            //event notification
            await _eventPublisher.EntityTokensAddedAsync(user, tokens);
        }

        ///// <summary>
        ///// Add partner tokens
        ///// </summary>
        ///// <param name="tokens">List of already added tokens</param>
        ///// <param name="partner">Partner</param>
        ///// <returns>A task that represents the asynchronous operation</returns>
        //public virtual async Task AddPartnerTokensAsync(IList<Token> tokens, Partner partner)
        //{
        //    tokens.Add(new Token("Partner.Name", partner.Name));
        //    tokens.Add(new Token("Partner.Email", partner.Email));

        //    var partnerAttributesXml = await _genericAttributeService.GetAttributeAsync<string>(partner, PartnerServicesDefaults.PartnerAttributes);
        //    tokens.Add(new Token("Partner.PartnerAttributes", await _partnerAttributeFormatter.FormatAttributesAsync(partnerAttributesXml), true));

        //    //event notification
        //    await _eventPublisher.EntityTokensAddedAsync(partner, tokens);
        //}

        /// <summary>
        /// Add newsletter subscription tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="subscription">Newsletter subscription</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task AddNewsLetterSubscriptionTokensAsync(IList<Token> tokens, NewsLetterSubscription subscription)
        {
            tokens.Add(new Token("NewsLetterSubscription.Email", subscription.Email));

            var activationUrl = await RouteUrlAsync(routeName: "NewsletterActivation", routeValues: new { token = subscription.NewsLetterSubscriptionGuid, active = "true" });
            tokens.Add(new Token("NewsLetterSubscription.ActivationUrl", activationUrl, true));

            var deactivationUrl = await RouteUrlAsync(routeName: "NewsletterActivation", routeValues: new { token = subscription.NewsLetterSubscriptionGuid, active = "false" });
            tokens.Add(new Token("NewsLetterSubscription.DeactivationUrl", deactivationUrl, true));

            //event notification
            await _eventPublisher.EntityTokensAddedAsync(subscription, tokens);
        }

        /// <summary>
        /// Add blog comment tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="blogComment">Blog post comment</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task AddBlogCommentTokensAsync(IList<Token> tokens, BlogComment blogComment)
        {
            var blogPost = await _blogService.GetBlogPostByIdAsync(blogComment.BlogPostId);

            tokens.Add(new Token("BlogComment.BlogPostTitle", blogPost.Title));

            //event notification
            await _eventPublisher.EntityTokensAddedAsync(blogComment, tokens);
        }

        /// <summary>
        /// Add news comment tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="newsComment">News comment</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task AddNewsCommentTokensAsync(IList<Token> tokens, NewsComment newsComment)
        {
            var newsItem = await _newsService.GetNewsByIdAsync(newsComment.NewsItemId);

            tokens.Add(new Token("NewsComment.NewsTitle", newsItem.Title));

            //event notification
            await _eventPublisher.EntityTokensAddedAsync(newsComment, tokens);
        }

        ///// <summary>
        ///// Add system message tokens
        ///// </summary>
        ///// <param name="tokens">List of already added tokens</param>
        ///// <param name="systemMessage">Private message</param>
        ///// <returns>A task that represents the asynchronous operation</returns>
        //public virtual async Task AddSystemMessageTokensAsync(IList<Token> tokens, SystemMessage systemMessage)
        //{
        //    //attributes
        //    //we cannot inject ISystemMessageService into constructor because it'll cause circular references.
        //    //that's why we resolve it here this way
        //    var systemMessageService = EngineContext.Current.Resolve<ISystemMessageService>();

        //    tokens.Add(new Token("SystemMessage.Subject", systemMessage.Subject));
        //    tokens.Add(new Token("SystemMessage.Text", systemMessageService.FormatSystemMessageText(systemMessage), true));

        //    //event notification
        //    await _eventPublisher.EntityTokensAddedAsync(systemMessage, tokens);
        //}

        /// <summary>
        /// Get collection of allowed (supported) message tokens for campaigns
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the collection of allowed (supported) message tokens for campaigns
        /// </returns>
        public virtual async Task<IEnumerable<string>> GetListOfCampaignAllowedTokensAsync()
        {
            var additionalTokens = new CampaignAdditionalTokensAddedEvent();
            await _eventPublisher.PublishAsync(additionalTokens);

            var allowedTokens = (await GetListOfAllowedTokensAsync([TokenGroupNames.NodeTokens, TokenGroupNames.SubscriptionTokens])).ToList();
            allowedTokens.AddRange(additionalTokens.AdditionalTokens);

            return allowedTokens.Distinct();
        }

        /// <summary>
        /// Get collection of allowed (supported) message tokens
        /// </summary>
        /// <param name="tokenGroups">Collection of token groups; pass null to get all available tokens</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the collection of allowed message tokens
        /// </returns>
        public virtual async Task<IEnumerable<string>> GetListOfAllowedTokensAsync(IEnumerable<string> tokenGroups = null)
        {
            var additionalTokens = new AdditionalTokensAddedEvent
            {
                TokenGroups = tokenGroups
            };
            await _eventPublisher.PublishAsync(additionalTokens);

            var allowedTokens = AllowedTokens.Where(x => tokenGroups == null || tokenGroups.Contains(x.Key))
                .SelectMany(x => x.Value).ToList();

            allowedTokens.AddRange(additionalTokens.AdditionalTokens);

            return allowedTokens.Distinct();
        }

        /// <summary>
        /// Get token groups of message template
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <returns>Collection of token group names</returns>
        public virtual IEnumerable<string> GetTokenGroups(MessageTemplate messageTemplate)
        {
            //groups depend on which tokens are added at the appropriate methods in IWorkflowMessageService
            return messageTemplate.Name switch
            {
                MessageTemplateSystemNames.UserRegisteredNotification or
                MessageTemplateSystemNames.UserWelcomeMessage or
                MessageTemplateSystemNames.UserEmailValidationMessage or
                MessageTemplateSystemNames.UserEmailRevalidationMessage or
                MessageTemplateSystemNames.UserPasswordRecoveryMessage => [TokenGroupNames.NodeTokens, TokenGroupNames.UserTokens],

                MessageTemplateSystemNames.NewsletterSubscriptionActivationMessage or
                MessageTemplateSystemNames.NewsletterSubscriptionDeactivationMessage => [TokenGroupNames.NodeTokens, TokenGroupNames.SubscriptionTokens],

                MessageTemplateSystemNames.EmailAFriendMessage => [TokenGroupNames.NodeTokens, TokenGroupNames.UserTokens, TokenGroupNames.ProductTokens, TokenGroupNames.EmailAFriendTokens],

                MessageTemplateSystemNames.NewPartnerAccountApplyNotification => [TokenGroupNames.NodeTokens, TokenGroupNames.UserTokens, TokenGroupNames.PartnerTokens],
                MessageTemplateSystemNames.PartnerInformationChangeNotification => [TokenGroupNames.NodeTokens, TokenGroupNames.PartnerTokens],

                MessageTemplateSystemNames.NewVatSubmittedNotification => [TokenGroupNames.NodeTokens, TokenGroupNames.UserTokens, TokenGroupNames.VatValidation],
                MessageTemplateSystemNames.BlogCommentNotification => [TokenGroupNames.NodeTokens, TokenGroupNames.BlogCommentTokens, TokenGroupNames.UserTokens],
                MessageTemplateSystemNames.NewsCommentNotification => [TokenGroupNames.NodeTokens, TokenGroupNames.NewsCommentTokens, TokenGroupNames.UserTokens],
                MessageTemplateSystemNames.ContactUsMessage => [TokenGroupNames.NodeTokens, TokenGroupNames.ContactUs],
                MessageTemplateSystemNames.ContactPartnerMessage => [TokenGroupNames.NodeTokens, TokenGroupNames.ContactPartner],
                _ => Array.Empty<string>(),
            };
        }

        #endregion
    }
}