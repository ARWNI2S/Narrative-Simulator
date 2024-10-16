using ARWNI2S.Node.Data;
using ARWNI2S.Portal.Framework.Routing;

namespace ARWNI2S.Portal.Infrastructure
{
    /// <summary>
    /// Represents provider that provides generic routes
    /// </summary>
    public partial class GenericUrlRouteProvider : BaseRouteProvider, IRouteProvider
    {
        #region Methods

        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            var lang = GetLanguageRoutePattern();

            //default routes
            //these routes are not generic, they are just default to map requests that don't match other patterns, 
            //but we define them here since this route provider is with the lowest priority, to allow to add additional routes before them
            if (!string.IsNullOrEmpty(lang))
            {
                endpointRouteBuilder.MapControllerRoute(name: "DefaultWithLanguageCode",
                    pattern: $"{lang}/{{controller=Home}}/{{action=Index}}/{{id?}}");
            }

            endpointRouteBuilder.MapControllerRoute(name: "Default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            //generic routes (actually routing is processed later in SlugRouteTransformer)
            var genericContentPattern = $"{lang}/{{controller}}/{{{NodeRoutingDefaults.RouteValue.SeName}}}";
            endpointRouteBuilder.MapDynamicControllerRoute<SlugRouteTransformer>(genericContentPattern);

            var genericPattern = $"{lang}/{{{NodeRoutingDefaults.RouteValue.SeName}}}";
            endpointRouteBuilder.MapDynamicControllerRoute<SlugRouteTransformer>(genericPattern);

            //routes for not found slugs
            if (!string.IsNullOrEmpty(lang))
            {
                endpointRouteBuilder.MapControllerRoute(name: NodeRoutingDefaults.RouteName.Generic.GenericUrlWithLanguageCode,
                    pattern: genericPattern,
                    defaults: new { controller = "Common", action = "GenericUrl" });

                endpointRouteBuilder.MapControllerRoute(name: NodeRoutingDefaults.RouteName.Generic.GenericContentUrlWithLanguageCode,
                    pattern: genericContentPattern,
                    defaults: new { controller = "Common", action = "GenericUrl" });
            }

            endpointRouteBuilder.MapControllerRoute(name: NodeRoutingDefaults.RouteName.Generic.GenericUrl,
                pattern: $"{{{NodeRoutingDefaults.RouteValue.SeName}}}",
                defaults: new { controller = "Common", action = "GenericUrl" });

            endpointRouteBuilder.MapControllerRoute(name: NodeRoutingDefaults.RouteName.Generic.GenericContentUrl,
                pattern: $"{{controller}}/{{{NodeRoutingDefaults.RouteValue.SeName}}}",
                defaults: new { controller = "Common", action = "GenericUrl" });

            //routes for entities that support content path and slug (e.g. '/category-seo-name/thing-seo-name')

            //routes for entities that support single slug (e.g. '/thing-seo-name')
            endpointRouteBuilder.MapControllerRoute(name: NodeRoutingDefaults.RouteName.Generic.GameTitle,
                pattern: genericPattern,
                defaults: new { controller = "Games", action = "Title" });

            endpointRouteBuilder.MapControllerRoute(name: NodeRoutingDefaults.RouteName.Generic.Quest,
                pattern: genericPattern,
                defaults: new { controller = "Quest", action = "Quest" });

            endpointRouteBuilder.MapControllerRoute(name: NodeRoutingDefaults.RouteName.Generic.Tournament,
                pattern: genericPattern,
                defaults: new { controller = "Tournament", action = "Tournament" });

            endpointRouteBuilder.MapControllerRoute(name: NodeRoutingDefaults.RouteName.Generic.NewsItem,
                pattern: genericPattern,
                defaults: new { controller = "News", action = "NewsItem" });

            endpointRouteBuilder.MapControllerRoute(name: NodeRoutingDefaults.RouteName.Generic.BlogPost,
                pattern: genericPattern,
                defaults: new { controller = "Blog", action = "BlogPost" });

            endpointRouteBuilder.MapControllerRoute(name: NodeRoutingDefaults.RouteName.Generic.Topic,
                pattern: genericPattern,
                defaults: new { controller = "Topic", action = "TopicDetails" });

            //endpointRouteBuilder.MapControllerRoute(name: NodeRoutingDefaults.RouteName.Generic.ThingTag,
            //    pattern: genericPattern,
            //    defaults: new { controller = "TODO", action = "ThingsByTag" });
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        /// <remarks>
        /// it should be the last route. we do not set it to -int.MaxValue so it could be overridden (if required)
        /// </remarks>
        public int Priority => -1000000;

        #endregion
    }
}
