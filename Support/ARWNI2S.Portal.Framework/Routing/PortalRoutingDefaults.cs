namespace ARWNI2S.Portal.Framework.Routing
{
    /// <summary>
    /// Represents default values related to routing
    /// </summary>
    public static partial class PortalRoutingDefaults
    {
        #region Route names

        public static partial class RouteName
        {
            public static partial class Generic
            {
                /// <summary>
                /// Gets the generic route name
                /// </summary>
                public static string GenericUrl => "GenericUrl";

                /// <summary>
                /// Gets the generic route name
                /// </summary>
                public static string GenericContentUrl => "GenericContentUrl";

                /// <summary>
                /// Gets the generic route (with language code e.g. en/) name
                /// </summary>
                public static string GenericUrlWithLanguageCode => "GenericUrlWithLanguageCode";

                /// <summary>
                /// Gets the generic route (with language code e.g. en/) name
                /// </summary>
                public static string GenericContentUrlWithLanguageCode => "GenericContentUrlWithLanguageCode";

                /// <summary>
                /// Gets the generic game title route name
                /// </summary>
                public static string GameTitle => "Title";

                /// <summary>
                /// Gets the generic quest route name
                /// </summary>
                public static string Quest => "Quest";

                /// <summary>
                /// Gets the generic tournament route name
                /// </summary>
                public static string Tournament => "Tournament";

                /// <summary>
                /// Gets the generic partner route name
                /// </summary>
                public static string Partner => "Partner";

                /// <summary>
                /// Gets the generic news item route name
                /// </summary>
                public static string NewsItem => "NewsItem";

                /// <summary>
                /// Gets the generic blog post route name
                /// </summary>
                public static string BlogPost => "BlogPost";

                /// <summary>
                /// Gets the generic topic route name
                /// </summary>
                public static string Topic => "TopicDetails";

                /// <summary>
                /// Gets default key for content route value
                /// </summary>
                public static string ContentSeName => "ContentSeName";

            }
        }

        #endregion

        #region Route values keys

        public static partial class RouteValue
        {
            /// <summary>
            /// Gets default key for action route value
            /// </summary>
            public static string Action => "action";

            /// <summary>
            /// Gets default key for controller route value
            /// </summary>
            public static string Controller => "controller";

            /// <summary>
            /// Gets default key for permanent redirect route value
            /// </summary>
            public static string PermanentRedirect => "permanentRedirect";

            /// <summary>
            /// Gets default key for url route value
            /// </summary>
            public static string Url => "url";

            /// <summary>
            /// Gets default key for blogpost id route value
            /// </summary>
            public static string BlogPostId => "blogpostId";

            /// <summary>
            /// Gets default key for newsitem id route value
            /// </summary>
            public static string NewsItemId => "newsitemId";

            /// <summary>
            /// Gets default key for topic id route value
            /// </summary>
            public static string TopicId => "topicid";

            /// <summary>
            /// Gets default key for title id route value
            /// </summary>
            public static string GameTitleId => "titleid";

            /// <summary>
            /// Gets default key for quest id route value
            /// </summary>
            public static string QuestId => "questid";

            /// <summary>
            /// Gets default key for tournament id route value
            /// </summary>
            public static string TournamentId => "tournamentid";

            /// <summary>
            /// Gets default key for partner id route value
            /// </summary>
            public static string PartnerId => "partnerid";

            /// <summary>
            /// Gets language route value
            /// </summary>
            public static string Language => "language";

            /// <summary>
            /// Gets default key for se name route value
            /// </summary>
            public static string SeName => "SeName";
        }

        #endregion

        /// <summary>
        /// Gets language parameter transformer
        /// </summary>
        public static string LanguageParameterTransformer => "lang";
    }
}
