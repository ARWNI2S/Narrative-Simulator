using ARWNI2S.Engine.Builder;
using ARWNI2S.Engine.Features.Providers;
using ARWNI2S.Engine.Options;
using ARWNI2S.Node.Core.Engine;
using ARWNI2S.Node.Hosting.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace ARWNI2S.Engine.Hosting.Extensions
{
    internal static class EngineCoreServiceCollectionExtensions
    {
        /// <summary>
        /// Add services to the engine and configure service provider
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static INiisBuilder AddEngineCore2(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services);

            var environment = GetServiceFromCollection<IHostEnvironment>(services);
            var nodeModuleManager = GetEnginePartManager(services, environment);
            services.TryAddSingleton(nodeModuleManager);

            ConfigureDefaultFeatureProviders(nodeModuleManager);
            ConfigureDefaultServices(services);
            AddEngineCore2Services(services);

            var builder = new NiisBuilder(services, nodeModuleManager);

            return builder;

        }

        private static void ConfigureDefaultFeatureProviders(EnginePartManager manager)
        {
            if (!manager.FeatureProviders.OfType<ActorFeatureProvider>().Any())
            {
                manager.FeatureProviders.Add(new ActorFeatureProvider());
            }
        }

        private static EnginePartManager GetEnginePartManager(IServiceCollection services, IHostEnvironment environment)
        {
            var manager = GetServiceFromCollection<EnginePartManager>(services);
            if (manager == null)
            {
                manager = new EnginePartManager();

                var entryAssemblyName = environment?.ApplicationName;
                if (string.IsNullOrEmpty(entryAssemblyName))
                {
                    return manager;
                }

                manager.PopulateDefaultParts(entryAssemblyName);
            }

            return manager;
        }

        private static T GetServiceFromCollection<T>(IServiceCollection services)
        {
            return (T)services
                .LastOrDefault(d => d.ServiceType == typeof(T))
                ?.ImplementationInstance;
        }


        /// <summary>
        /// Adds the minimum essential NI2S services to the specified <see cref="IServiceCollection" />. Additional services
        /// including NI2S's support for authorization, formatters, and validation must be added separately using the
        /// <see cref="INiisBuilder"/> returned from this method.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="setupAction">An <see cref="Action{MVRMOptions}"/> to configure the provided <see cref="EngineOptions"/>.</param>
        /// <returns>An <see cref="INiisBuilder"/> that can be used to further configure the NI2S services.</returns>
        /// <remarks>
        /// The <see cref="AddEngineCore2(IServiceCollection)"/> approach for configuring
        /// NI2S is provided for experienced NI2S developers who wish to have full control over the set of default services
        /// registered. <see cref="AddEngineCore2(IServiceCollection)"/> will register
        /// the minimum set of services necessary to route requests and invoke controllers. It is not expected that any
        /// engine will satisfy its requirements with just a call to
        /// <see cref="AddEngineCore2(IServiceCollection)"/>. Additional configuration using the
        /// <see cref="INiisBuilder"/> will be required.
        /// </remarks>
        public static INiisBuilder AddEngineCore2(
            this IServiceCollection services,
            Action<EngineOptions> setupAction)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(setupAction);

            var builder = services.AddEngineCore2();
            services.Configure(setupAction);

            return builder;
        }



        // To enable unit testing
        internal static void AddEngineCore2Services(IServiceCollection _)
        {
            ////
            //// Options
            ////
            //services.TryAddEnumerable(
            //    ServiceDescriptor.Transient<IConfigureOptions<MVRMOptions>, MVRMCoreMVRMOptionsSetup>());
            //services.TryAddEnumerable(
            //    ServiceDescriptor.Transient<IPostConfigureOptions<MVRMOptions>, MVRMCoreMVRMOptionsSetup>());
            //services.TryAddEnumerable(
            //    ServiceDescriptor.Transient<IConfigureOptions<ApiBehaviorOptions>, ApiBehaviorOptionsSetup>());
            //services.TryAddEnumerable(
            //    ServiceDescriptor.Transient<IConfigureOptions<RouteOptions>, MVRMCoreRouteOptionsSetup>());

            ////
            //// Action Discovery
            ////
            //// These are consumed only when creating action descriptors, then they can be deallocated
            //services.TryAddSingleton<EngineModelFactory>();
            //services.TryAddEnumerable(
            //    ServiceDescriptor.Transient<IEngineModelProvider, DefaultEngineModelProvider>());
            //services.TryAddEnumerable(
            //    ServiceDescriptor.Transient<IEngineModelProvider, ApiBehaviorEngineModelProvider>());
            //services.TryAddEnumerable(
            //    ServiceDescriptor.Transient<IActionDescriptorProvider, ControllerActionDescriptorProvider>());

            //services.TryAddSingleton<IActionDescriptorCollectionProvider, DefaultActionDescriptorCollectionProvider>();

            ////
            //// Action Selection
            ////
            //services.TryAddSingleton<IActionSelector, ActionSelector>();
            //services.TryAddSingleton<ActionConstraintCache>();

            //// Will be cached by the DefaultActionSelector
            //services.TryAddEnumerable(ServiceDescriptor.Transient<IActionConstraintProvider, DefaultActionConstraintProvider>());

            //// Policies for Interactions
            //services.TryAddEnumerable(ServiceDescriptor.Singleton<MatcherPolicy, ActionConstraintMatcherPolicy>());

            ////
            //// Actor Factory
            ////
            //// This has a cache, so it needs to be a singleton
            //services.TryAddSingleton<IActorFactory, DefaultActorFactory>();

            //// Will be cached by the DefaultActorFactory
            //services.TryAddTransient<IActorActivator, DefaultActorActivator>();

            //services.TryAddSingleton<IActorFactoryProvider, ActorFactoryProvider>();
            //services.TryAddSingleton<IActorActivatorProvider, ActorActivatorProvider>();
            //services.TryAddEnumerable(
            //    ServiceDescriptor.Transient<IActorPropertyActivator, DefaultActorPropertyActivator>());

            ////
            //// Action Invoker
            ////
            //// The IActionInvokerFactory is cacheable
            //services.TryAddSingleton<IActionInvokerFactory, ActionInvokerFactory>();
            //services.TryAddEnumerable(
            //    ServiceDescriptor.Transient<IActionInvokerProvider, ActorActionInvokerProvider>());

            //// These are stateless
            //services.TryAddSingleton<ActorActionInvokerCache>();
            //services.TryAddEnumerable(
            //    ServiceDescriptor.Singleton<IFilterProvider, DefaultFilterProvider>());
            //services.TryAddSingleton<IActionResultTypeMapper, ActionResultTypeMapper>();

            ////
            //// Request body limit filters
            ////
            //services.TryAddTransient<RequestSizeLimitFilter>();
            //services.TryAddTransient<DisableRequestSizeLimitFilter>();
            //services.TryAddTransient<RequestFormLimitsFilter>();

            ////
            //// ModelBinding, Validation
            ////
            //// The DefaultModelMetadataProvider does significant caching and should be a singleton.
            //services.TryAddSingleton<IModelMetadataProvider, DefaultModelMetadataProvider>();
            //services.TryAdd(ServiceDescriptor.Transient<ICompositeMetadataDetailsProvider>(s =>
            //{
            //    var options = s.GetRequiredService<IOptions<MVRMOptions>>().Value;
            //    return new DefaultCompositeMetadataDetailsProvider(options.ModelMetadataDetailsProviders);
            //}));
            //services.TryAddSingleton<IModelBinderFactory, ModelBinderFactory>();
            //services.TryAddSingleton<IObjectModelValidator>(s =>
            //{
            //    var options = s.GetRequiredService<IOptions<MVRMOptions>>().Value;
            //    var metadataProvider = s.GetRequiredService<IModelMetadataProvider>();
            //    return new DefaultObjectValidator(metadataProvider, options.ModelValidatorProviders, options);
            //});
            //services.TryAddSingleton<ClientValidatorCache>();
            //services.TryAddSingleton<ParameterBinder>();

            ////
            //// Random Infrastructure
            ////
            //services.TryAddSingleton<NI2SMarkerService, NI2SMarkerService>();
            //services.TryAddSingleton<ITypeActivatorCache, TypeActivatorCache>();
            //services.TryAddSingleton<IUrlHelperFactory, UrlHelperFactory>();
            //services.TryAddSingleton<IHttpRequestStreamReaderFactory, MemoryPoolHttpRequestStreamReaderFactory>();
            //services.TryAddSingleton<IHttpResponseStreamWriterFactory, MemoryPoolHttpResponseStreamWriterFactory>();
            //services.TryAddSingleton(ArrayPool<byte>.Shared);
            //services.TryAddSingleton(ArrayPool<char>.Shared);
            //services.TryAddSingleton<OutputFormatterSelector, DefaultOutputFormatterSelector>();
            //services.TryAddSingleton<IActionResultExecutor<ObjectResult>, ObjectResultExecutor>();
            //services.TryAddSingleton<IActionResultExecutor<PhysicalFileResult>, PhysicalFileResultExecutor>();
            //services.TryAddSingleton<IActionResultExecutor<VirtualFileResult>, VirtualFileResultExecutor>();
            //services.TryAddSingleton<IActionResultExecutor<FileStreamResult>, FileStreamResultExecutor>();
            //services.TryAddSingleton<IActionResultExecutor<FileContentResult>, FileContentResultExecutor>();
            //services.TryAddSingleton<IActionResultExecutor<RedirectResult>, RedirectResultExecutor>();
            //services.TryAddSingleton<IActionResultExecutor<LocalRedirectResult>, LocalRedirectResultExecutor>();
            //services.TryAddSingleton<IActionResultExecutor<RedirectToActionResult>, RedirectToActionResultExecutor>();
            //services.TryAddSingleton<IActionResultExecutor<RedirectToRouteResult>, RedirectToRouteResultExecutor>();
            //services.TryAddSingleton<IActionResultExecutor<RedirectToPageResult>, RedirectToPageResultExecutor>();
            //services.TryAddSingleton<IActionResultExecutor<ContentResult>, ContentResultExecutor>();
            //services.TryAddSingleton<IActionResultExecutor<JsonResult>, SystemTextJsonResultExecutor>();
            //services.TryAddSingleton<IClientErrorFactory, ProblemDetailsClientErrorFactory>();

            ////
            //// Route Handlers
            ////
            //services.TryAddSingleton<NI2SRouteHandler>(); // Only one per app
            //services.TryAddTransient<NI2SAttributeRouteHandler>(); // Many per app

            ////
            //// Endpoint Routing / Endpoints
            ////
            //services.TryAddSingleton<ControllerActionEndpointDataSourceFactory>();
            //services.TryAddSingleton<OrderedEndpointsSequenceProviderCache>();
            //services.TryAddSingleton<ControllerActionEndpointDataSourceIdProvider>();
            //services.TryAddSingleton<ActionEndpointFactory>();
            //services.TryAddSingleton<DynamicControllerEndpointSelectorCache>();
            //services.TryAddEnumerable(ServiceDescriptor.Singleton<MatcherPolicy, DynamicControllerEndpointMatcherPolicy>());
            //services.TryAddEnumerable(ServiceDescriptor.Singleton<IRequestDelegateFactory, ControllerRequestDelegateFactory>());

            ////
            //// Middleware pipeline filter related
            ////
            //services.TryAddSingleton<MiddlewareFilterConfigurationProvider>();
            //// This maintains a cache of middleware pipelines, so it needs to be a singleton
            //services.TryAddSingleton<MiddlewareFilterBuilder>();
            //// Sets EngineBuilder on MiddlewareFilterBuilder
            //services.TryAddEnumerable(ServiceDescriptor.Singleton<IStartupFilter, MiddlewareFilterBuilderStartupFilter>());

            //// ProblemDetails
            //services.TryAddSingleton<ProblemDetailsFactory, DefaultProblemDetailsFactory>();
            //services.TryAddEnumerable(ServiceDescriptor.Singleton<IProblemDetailsWriter, DefaultApiProblemDetailsWriter>());
        }

        private static void ConfigureDefaultServices(IServiceCollection services)
        {
            //services.AddMemoryCache();
        }
    }
}
