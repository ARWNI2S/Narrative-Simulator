using ARWNI2S.Portal.Framework.Routing;
using ARWNI2S.Portal.Services.Installation;

namespace ARWNI2S.Portal.Infrastructure
{
    /// <summary>
    /// Represents provider that provides basic routes
    /// </summary>
    public partial class RouteProvider : BaseRouteProvider, IRouteProvider
    {
        #region Methods

        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            //get language pattern
            //it's not needed to use language pattern in AJAX requests and for actions returning the result directly (e.g. file to download),
            //use it only for URLs of pages that the user can go to
            var lang = GetLanguageRoutePattern();

            //areas
            endpointRouteBuilder.MapControllerRoute(name: "areaRoute",
                pattern: $"{{area:exists}}/{{controller=Home}}/{{action=Index}}/{{id?}}");

            // Página de inicio, redirigir a Blazor
            endpointRouteBuilder.MapFallbackToPage($"/_Host");

            //home page
            endpointRouteBuilder.MapControllerRoute(name: "Homepage",
                pattern: $"{lang}",
                defaults: new { controller = "Home", action = "Index" });

            //login
            endpointRouteBuilder.MapControllerRoute(name: "Login",
                pattern: $"{lang}/login/",
                defaults: new { controller = "User", action = "Login" });

            // multi-factor verification digit code page
            endpointRouteBuilder.MapControllerRoute(name: "MultiFactorVerification",
                pattern: $"{lang}/multi-factor-verification/",
                defaults: new { controller = "User", action = "MultiFactorVerification" });

            //register
            endpointRouteBuilder.MapControllerRoute(name: "Register",
                pattern: $"{lang}/register/",
                defaults: new { controller = "User", action = "Register" });

            //logout
            endpointRouteBuilder.MapControllerRoute(name: "Logout",
                pattern: $"{lang}/logout/",
                defaults: new { controller = "User", action = "Logout" });

            //user account links
            endpointRouteBuilder.MapControllerRoute(name: "UserInfo",
                pattern: $"{lang}/user/info",
                defaults: new { controller = "User", action = "Info" });

            endpointRouteBuilder.MapControllerRoute(name: "UserAddresses",
                pattern: $"{lang}/user/addresses",
                defaults: new { controller = "User", action = "Addresses" });

            endpointRouteBuilder.MapControllerRoute(name: "UserWallets",
                pattern: $"{lang}/user/wallets",
                defaults: new { controller = "User", action = "Wallets" });

            //contact us
            endpointRouteBuilder.MapControllerRoute(name: "ContactUs",
                pattern: $"{lang}/contactus",
                defaults: new { controller = "Common", action = "ContactUs" });

            //product search
            endpointRouteBuilder.MapControllerRoute(name: "EntitySearch",
                pattern: $"{lang}/search/",
                defaults: new { controller = "Search", action = "Search" });

            //autocomplete search term (AJAX)
            //endpointRouteBuilder.MapControllerRoute(name: "ThingSearchAutoComplete",
            //    pattern: $"todo/searchtermautocomplete",
            //    defaults: new { controller = "TODO", action = "SearchTermAutoComplete" });

            //change currency
            endpointRouteBuilder.MapControllerRoute(name: "ChangeCurrency",
                pattern: $"{lang}/changecurrency/{{usercurrency:min(0)}}",
                defaults: new { controller = "Common", action = "SetCurrency" });

            //change token
            endpointRouteBuilder.MapControllerRoute(name: "ChangeToken",
                pattern: $"{lang}/changetoken/{{usertoken:min(0)}}",
                defaults: new { controller = "Common", action = "SetToken" });

            //change language
            endpointRouteBuilder.MapControllerRoute(name: "ChangeLanguage",
                pattern: $"{lang}/changelanguage/{{langid:min(0)}}",
                defaults: new { controller = "Common", action = "SetLanguage" });

            //change tax
            endpointRouteBuilder.MapControllerRoute(name: "ChangeTaxType",
                pattern: $"{lang}/changetaxtype/{{usertaxtype:min(0)}}",
                defaults: new { controller = "Common", action = "SetTaxType" });

            //blog
            endpointRouteBuilder.MapControllerRoute(name: "Blog",
                pattern: $"{lang}/blog",
                defaults: new { controller = "Blog", action = "List" });

            //news
            endpointRouteBuilder.MapControllerRoute(name: "NewsArchive",
                pattern: $"{lang}/news",
                defaults: new { controller = "News", action = "List" });

            //games
            endpointRouteBuilder.MapControllerRoute(name: "Games",
                pattern: $"{lang}/games",
                defaults: new { controller = "Games", action = "List" });

            //quests
            endpointRouteBuilder.MapControllerRoute(name: "Quests",
                pattern: $"{lang}/quests",
                defaults: new { controller = "Quest", action = "List" });

            //inventory
            endpointRouteBuilder.MapControllerRoute(name: "Inventory",
                pattern: $"{lang}/inventory",
                defaults: new { controller = "Governance", action = "Inventory" });

            //tournaments
            endpointRouteBuilder.MapControllerRoute(name: "Tournaments",
                pattern: $"{lang}/tournaments",
                defaults: new { controller = "Tournament", action = "List" });

            //URGENT: partners
            //partners
            //endpointRouteBuilder.MapControllerRoute(name: "PartnerList",
            //    pattern: $"{lang}/partner/all/",
            //    defaults: new { controller = "TODO", action = "PartnerAll" });

            //subscribe newsletters (AJAX)
            endpointRouteBuilder.MapControllerRoute(name: "SubscribeNewsletter",
                pattern: $"subscribenewsletter",
                defaults: new { controller = "Newsletter", action = "SubscribeNewsletter" });

            //register result page
            endpointRouteBuilder.MapControllerRoute(name: "RegisterResult",
                pattern: $"{lang}/registerresult/{{resultId:min(0)}}",
                defaults: new { controller = "User", action = "RegisterResult" });

            //check username availability (AJAX)
            endpointRouteBuilder.MapControllerRoute(name: "CheckUsernameAvailability",
                pattern: $"user/checkusernameavailability",
                defaults: new { controller = "User", action = "CheckUsernameAvailability" });

            //passwordrecovery
            endpointRouteBuilder.MapControllerRoute(name: "PasswordRecovery",
                pattern: $"{lang}/passwordrecovery",
                defaults: new { controller = "User", action = "PasswordRecovery" });

            //password recovery confirmation
            endpointRouteBuilder.MapControllerRoute(name: "PasswordRecoveryConfirm",
                pattern: $"{lang}/passwordrecovery/confirm",
                defaults: new { controller = "User", action = "PasswordRecoveryConfirm" });

            //topics (AJAX)
            endpointRouteBuilder.MapControllerRoute(name: "TopicPopup",
                pattern: $"t-popup/{{SystemName}}",
                defaults: new { controller = "Topic", action = "TopicDetailsPopup" });

            //topics (AJAX)
            endpointRouteBuilder.MapControllerRoute(name: "TopicPopup",
                pattern: $"t-popup/{{SystemName}}",
                defaults: new { controller = "Topic", action = "TopicDetailsPopup" });

            //topics (AJAX)
            endpointRouteBuilder.MapControllerRoute(name: "TopicPopup",
                pattern: $"t-popup/{{SystemName}}",
                defaults: new { controller = "Topic", action = "TopicDetailsPopup" });

            //tournaments (AJAX)
            endpointRouteBuilder.MapControllerRoute(name: "TournamentPopup",
                pattern: $"tour-popup/{{SystemName}}",
                defaults: new { controller = "Tournament", action = "TopicDetailsPopup" });



            //blog
            endpointRouteBuilder.MapControllerRoute(name: "BlogByTag",
                pattern: $"{lang}/blog/tag/{{tag}}",
                defaults: new { controller = "Blog", action = "BlogByTag" });

            endpointRouteBuilder.MapControllerRoute(name: "BlogByMonth",
                pattern: $"{lang}/blog/month/{{month}}",
                defaults: new { controller = "Blog", action = "BlogByMonth" });

            //blog RSS (file result)
            endpointRouteBuilder.MapControllerRoute(name: "BlogRSS",
                pattern: $"blog/rss/{{languageId:min(0)}}",
                defaults: new { controller = "Blog", action = "ListRss" });

            //news RSS (file result)
            endpointRouteBuilder.MapControllerRoute(name: "NewsRSS",
                pattern: $"news/rss/{{languageId:min(0)}}",
                defaults: new { controller = "News", action = "ListRss" });

            //quests RSS (file result)
            endpointRouteBuilder.MapControllerRoute(name: "QuestsRSS",
                pattern: $"quests/rss/{{languageId:min(0)}}",
                defaults: new { controller = "Quest", action = "ListRss" });

            //set review helpfulness (AJAX)
            endpointRouteBuilder.MapControllerRoute(name: "SetThingReviewHelpfulness",
                pattern: $"setthingreviewhelpfulness",
                defaults: new { controller = "Thing", action = "SetThingReviewHelpfulness" });

            endpointRouteBuilder.MapControllerRoute(name: "UserChangePassword",
                pattern: $"{lang}/user/changepassword",
                defaults: new { controller = "User", action = "ChangePassword" });

            endpointRouteBuilder.MapControllerRoute(name: "UserAvatar",
                pattern: $"{lang}/user/avatar",
                defaults: new { controller = "User", action = "Avatar" });

            endpointRouteBuilder.MapControllerRoute(name: "AccountActivation",
                pattern: $"{lang}/user/activation",
                defaults: new { controller = "User", action = "AccountActivation" });

            endpointRouteBuilder.MapControllerRoute(name: "EmailRevalidation",
                pattern: $"{lang}/user/revalidateemail",
                defaults: new { controller = "User", action = "EmailRevalidation" });

            endpointRouteBuilder.MapControllerRoute(name: "UserAddressEdit",
                pattern: $"{lang}/user/addressedit/{{addressId:min(0)}}",
                defaults: new { controller = "User", action = "AddressEdit" });

            endpointRouteBuilder.MapControllerRoute(name: "UserAddressAdd",
                pattern: $"{lang}/user/addressadd",
                defaults: new { controller = "User", action = "AddressAdd" });

            //endpointRouteBuilder.MapControllerRoute(name: "UserWalletEdit",
            //    pattern: $"{lang}/user/walletedit/{{walletId:min(0)}}",
            //    defaults: new { controller = "User", action = "WalletEdit" });

            //endpointRouteBuilder.MapControllerRoute(name: "UserWalletAdd",
            //    pattern: $"{lang}/user/walletadd",
            //    defaults: new { controller = "User", action = "WalletAdd" });

            endpointRouteBuilder.MapControllerRoute(name: "UserMultiFactorAuthenticationProviderConfig",
                pattern: $"{lang}/user/providerconfig",
                defaults: new { controller = "User", action = "ConfigureMultiFactorAuthenticationProvider" });

            //user profile page
            endpointRouteBuilder.MapControllerRoute(name: "UserProfile",
                pattern: $"{lang}/profile/{{id:min(0)}}",
                defaults: new { controller = "Profile", action = "Index" });

            endpointRouteBuilder.MapControllerRoute(name: "UserProfilePaged",
                pattern: $"{lang}/profile/{{id:min(0)}}/page/{{pageNumber:min(0)}}",
                defaults: new { controller = "Profile", action = "Index" });

            //contact partner
            endpointRouteBuilder.MapControllerRoute(name: "ContactPartner",
                pattern: $"{lang}/contactpartner/{{partnerId}}",
                defaults: new { controller = "Common", action = "ContactPartner" });

            //apply for partner account
            endpointRouteBuilder.MapControllerRoute(name: "ApplyPartnerAccount",
                pattern: $"{lang}/partner/apply",
                defaults: new { controller = "Partner", action = "ApplyPartner" });

            //partner info
            endpointRouteBuilder.MapControllerRoute(name: "UserPartnerInfo",
                pattern: $"{lang}/user/partnerinfo",
                defaults: new { controller = "Partner", action = "Info" });

            //user GDPR
            endpointRouteBuilder.MapControllerRoute(name: "GdprTools",
                pattern: $"{lang}/user/gdpr",
                defaults: new { controller = "User", action = "GdprTools" });

            //player check experience balance 
            //endpointRouteBuilder.MapControllerRoute(name: "CheckExperience",
            //    pattern: $"{lang}/player/checkexperience",
            //    defaults: new { controller = "Player", action = "CheckExperience" });

            //user multi-factor authentication settings 
            endpointRouteBuilder.MapControllerRoute(name: "MultiFactorAuthenticationSettings",
                pattern: $"{lang}/user/multifactorauthentication",
                defaults: new { controller = "User", action = "MultiFactorAuthentication" });

            //poll vote (AJAX)
            endpointRouteBuilder.MapControllerRoute(name: "PollVote",
                pattern: $"poll/vote",
                defaults: new { controller = "Poll", action = "Vote" });

            //comparing things
            //endpointRouteBuilder.MapControllerRoute(name: "RemoveThingFromCompareList",
            //    pattern: $"{lang}/comparethings/remove/{{thingId}}",
            //    defaults: new { controller = "Thing", action = "RemoveThingFromCompareList" });

            //endpointRouteBuilder.MapControllerRoute(name: "ClearCompareList",
            //    pattern: $"{lang}/clearcomparelist/",
            //    defaults: new { controller = "Thing", action = "ClearCompareList" });

            //new RSS (file result)
            //endpointRouteBuilder.MapControllerRoute(name: "NewThingsRSS",
            //    pattern: $"newthings/rss",
            //    defaults: new { controller = "TODO", action = "NewThingsRss" });

            //get state list by country ID (AJAX)
            endpointRouteBuilder.MapControllerRoute(name: "GetStatesByCountryId",
                pattern: $"country/getstatesbycountryid/",
                defaults: new { controller = "Country", action = "GetStatesByCountryId" });

            //EU Cookie law accept button handler (AJAX)
            endpointRouteBuilder.MapControllerRoute(name: "EuCookieLawAccept",
                pattern: $"eucookielawaccept",
                defaults: new { controller = "Common", action = "EuCookieLawAccept" });

            //authenticate topic (AJAX)
            endpointRouteBuilder.MapControllerRoute(name: "TopicAuthenticate",
                pattern: $"topic/authenticate",
                defaults: new { controller = "Topic", action = "Authenticate" });

            //prepare top menu (AJAX)
            //endpointRouteBuilder.MapControllerRoute(name: "GetTODORoot",
            //    pattern: $"todo/gettodoroot",
            //    defaults: new { controller = "TODO", action = "GetTODORoot" });

            //endpointRouteBuilder.MapControllerRoute(name: "GetTODOSubCategories",
            //    pattern: $"todo/gettodosubcategories",
            //    defaults: new { controller = "TODO", action = "GetTODOSubCategories" });

            //UNDONE things (AJAX)
            //endpointRouteBuilder.MapControllerRoute(name: "GetCategoryThings",
            //    pattern: $"category/things/",
            //    defaults: new { controller = "TODO", action = "GetCategoryThings" });

            //endpointRouteBuilder.MapControllerRoute(name: "GetManufacturerThings",
            //    pattern: $"manufacturer/things/",
            //    defaults: new { controller = "TODO", action = "GetManufacturerThings" });

            //endpointRouteBuilder.MapControllerRoute(name: "GetTagThings",
            //    pattern: $"tag/things",
            //    defaults: new { controller = "TODO", action = "GetTagThings" });

            //endpointRouteBuilder.MapControllerRoute(name: "SearchThings",
            //    pattern: $"thing/search",
            //    defaults: new { controller = "TODO", action = "SearchThings" });

            //endpointRouteBuilder.MapControllerRoute(name: "GetPartnerThings",
            //    pattern: $"partner/things",
            //    defaults: new { controller = "TODO", action = "GetPartnerThings" });

            //endpointRouteBuilder.MapControllerRoute(name: "GetNewThings",
            //    pattern: $"newthings/things/",
            //    defaults: new { controller = "TODO", action = "GetNewThings" });

            ////thing attributes with "upload file" type (AJAX)
            //endpointRouteBuilder.MapControllerRoute(name: "UploadFileThingAttribute",
            //    pattern: $"uploadfilethingattribute/{{attributeId:min(0)}}",
            //    defaults: new { controller = "ShoppingCart", action = "UploadFileThingAttribute" });

            ////checkout attributes with "upload file" type (AJAX)
            //endpointRouteBuilder.MapControllerRoute(name: "UploadFileCheckoutAttribute",
            //    pattern: $"uploadfilecheckoutattribute/{{attributeId:min(0)}}",
            //    defaults: new { controller = "ShoppingCart", action = "UploadFileCheckoutAttribute" });

            ////return request with "upload file" support (AJAX)
            //endpointRouteBuilder.MapControllerRoute(name: "UploadFileReturnRequest",
            //    pattern: $"uploadfilereturnrequest",
            //    defaults: new { controller = "ReturnRequest", action = "UploadFileReturnRequest" });

            //system messages
            endpointRouteBuilder.MapControllerRoute(name: "SystemMessages",
                pattern: $"{lang}/systemmessages/{{tab?}}",
                defaults: new { controller = "SystemMessages", action = "Index" });

            endpointRouteBuilder.MapControllerRoute(name: "SystemMessagesPaged",
                pattern: $"{lang}/systemmessages/{{tab?}}/page/{{pageNumber:min(0)}}",
                defaults: new { controller = "SystemMessages", action = "Index" });

            endpointRouteBuilder.MapControllerRoute(name: "SystemMessagesInbox",
                pattern: $"{lang}/inboxupdate",
                defaults: new { controller = "SystemMessages", action = "InboxUpdate" });

            endpointRouteBuilder.MapControllerRoute(name: "ViewMessage",
                pattern: $"{lang}/viewpm/{{privateMessageId:min(0)}}",
                defaults: new { controller = "SystemMessages", action = "ViewMessage" });

            endpointRouteBuilder.MapControllerRoute(name: "DeleteMessage",
                pattern: $"{lang}/deletepm/{{privateMessageId:min(0)}}",
                defaults: new { controller = "SystemMessages", action = "DeleteMessage" });

            //activate newsletters
            endpointRouteBuilder.MapControllerRoute(name: "NewsletterActivation",
                pattern: $"{lang}/newsletter/subscriptionactivation/{{token:guid}}/{{active}}",
                defaults: new { controller = "Newsletter", action = "SubscriptionActivation" });

            //robots.txt (file result)
            endpointRouteBuilder.MapControllerRoute(name: "robots.txt",
                pattern: $"robots.txt",
                defaults: new { controller = "Common", action = "RobotsTextFile" });

            //sitemap
            endpointRouteBuilder.MapControllerRoute(name: "Sitemap",
                pattern: $"{lang}/sitemap",
                defaults: new { controller = "Common", action = "Sitemap" });

            //sitemap.xml (file result)
            endpointRouteBuilder.MapControllerRoute(name: "sitemap.xml",
                pattern: $"sitemap.xml",
                defaults: new { controller = "Common", action = "SitemapXml" });

            endpointRouteBuilder.MapControllerRoute(name: "sitemap-indexed.xml",
                pattern: $"sitemap-{{Id:min(0)}}.xml",
                defaults: new { controller = "Common", action = "SitemapXml" });

            //server offline
            endpointRouteBuilder.MapControllerRoute(name: "NodeOffline",
                pattern: $"{lang}/nodeoffline",
                defaults: new { controller = "Common", action = "NodeOffline" });

            //install
            endpointRouteBuilder.MapControllerRoute(name: "Installation",
                pattern: $"{PortalInstallationDefaults.InstallPath}",
                defaults: new { controller = "Install", action = "Index" });

            //page not found
            endpointRouteBuilder.MapControllerRoute(name: "PageNotFound",
                pattern: $"{lang}/page-not-found",
                defaults: new { controller = "Common", action = "PageNotFound" });

        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => 0;

        #endregion
    }
}
