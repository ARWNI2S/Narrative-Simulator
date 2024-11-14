using ARWNI2S.Engine.Hosting.Diagnostics;
using ARWNI2S.Engine.Hosting.Internal;
using ARWNI2S.Infrastructure.Engine;
using ARWNI2S.Infrastructure.Engine.Builder;
using ARWNI2S.Infrastructure.Extensions;
using ARWNI2S.Node.Builder;
using ARWNI2S.Node.Configuration.Options;
using ARWNI2S.Node.Hosting;
using ARWNI2S.Node.Hosting.Infrastructure;
using ARWNI2S.Node.Hosting.Startup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using IHostingEnvironment = Microsoft.Extensions.Hosting.IHostingEnvironment;

namespace ARWNI2S.Node.Hosting.Internal
{
    internal sealed class GenericNodeHostBuilder : NodeHostBuilderBase, ISupportsHostStartup
    {
        private object _startupObject;
        private readonly object _startupKey = new();

        private AggregateException _hostingStartupErrors;
        private HostingStartupNodeHostBuilder _hostingStartupNodeHostBuilder;

        public GenericNodeHostBuilder(IHostBuilder builder, NodeHostBuilderOptions options)
            : base(builder, options)
        {
            _builder.ConfigureHostConfiguration(config =>
            {
                config.AddConfiguration(_config);

                // We do this super early but still late enough that we can process the configuration
                // wired up by calls to UseSetting
                ExecuteHostingStartups();
            });

            // IHostingStartup needs to be executed before any direct methods on the builder
            // so register these callbacks first
            _builder.ConfigureAppConfiguration((context, configurationBuilder) =>
            {
                if (_hostingStartupNodeHostBuilder != null)
                {
                    var nodehostContext = GetNodeHostBuilderContext(context);
                    _hostingStartupNodeHostBuilder.ConfigureAppConfiguration(nodehostContext, configurationBuilder);
                }
            });

            _builder.ConfigureServices((context, services) =>
            {
                var nodehostContext = GetNodeHostBuilderContext(context);
                var nodeHostOptions = (NodeHostOptions)context.Properties[typeof(NodeHostOptions)];

                // Add the IHostingEnvironment and IEngineLifetime from ARWNI2S.Node.Hosting
                services.AddSingleton(nodehostContext.HostingEnvironment);
#pragma warning disable CS0618 // Type or member is obsolete
                services.AddSingleton((IHostingEnvironment)nodehostContext.HostingEnvironment);
                services.AddSingleton<IApplicationLifetime, GenericNodeHostApplicationLifetime>();
#pragma warning restore CS0618 // Type or member is obsolete

                services.Configure<GenericNodeHostServiceOptions>(options =>
                {
                    // Set the options
                    options.NodeHostOptions = nodeHostOptions;
                    // Store and forward any startup errors
                    options.HostingStartupExceptions = _hostingStartupErrors;
                });

                // REVIEW: This is bad since we don't own this type. Anybody could add one of these and it would mess things up
                // We need to flow this differently
                services.TryAddSingleton(sp => new DiagnosticListener("ARWNI2S.Node"));
                services.TryAddSingleton<DiagnosticSource>(sp => sp.GetRequiredService<DiagnosticListener>());
                services.TryAddSingleton(sp => new ActivitySource("ARWNI2S.Node"));
                services.TryAddSingleton(DistributedContextPropagator.Current);

                services.TryAddSingleton<IEngineContextFactory, DefaultEngineContextFactory>();
                //services.TryAddScoped<IMiddlewareFactory, MiddlewareFactory>();
                services.TryAddSingleton<IEngineBuilderFactory, EngineBuilderFactory>();

                services.AddMetrics();
                services.TryAddSingleton<HostingEngineMetrics>();

                // IMPORTANT: This needs to run *before* direct calls on the builder (like UseStartup)
                _hostingStartupNodeHostBuilder?.ConfigureServices(nodehostContext, services);

                // Support UseStartup(assemblyName)
                if (!string.IsNullOrEmpty(nodeHostOptions.StartupAssembly))
                {
                    ScanAssemblyAndRegisterStartup(context, services, nodehostContext, nodeHostOptions);
                }
            });
        }

        [UnconditionalSuppressMessage("Trimmer", "IL2072", Justification = "Finding startup type in assembly requires unreferenced code. Surfaced to user in UseStartup(assemblyName).")]
        private void ScanAssemblyAndRegisterStartup(HostBuilderContext context, IServiceCollection services, NodeHostBuilderContext nodehostContext, NodeHostOptions nodeHostOptions)
        {
            try
            {
                //var startupType = StartupLoader.FindStartupType(nodeHostOptions.StartupAssembly!, nodehostContext.HostingEnvironment.EnvironmentName);
                //UseStartup(startupType, context, services);
            }
            catch (Exception ex) when (nodeHostOptions.CaptureStartupErrors)
            {
                var capture = ExceptionDispatchInfo.Capture(ex);

                services.Configure<GenericNodeHostServiceOptions>(options =>
                {
                    options.ConfigureEngine = engine =>
                    {
                        // Throw if there was any errors initializing startup
                        capture.Throw();
                    };
                });
            }
        }

        private void ExecuteHostingStartups()
        {
            var nodeHostOptions = new NodeHostOptions(_config);

            if (nodeHostOptions.PreventHostingStartup)
            {
                return;
            }

            var exceptions = new List<Exception>();
            var processed = new HashSet<Assembly>();

            _hostingStartupNodeHostBuilder = new HostingStartupNodeHostBuilder(this);

            // Execute the hosting startup assemblies
            foreach (var assemblyName in nodeHostOptions.GetFinalHostingStartupAssemblies())
            {
                try
                {
                    var assembly = Assembly.Load(new AssemblyName(assemblyName));

                    if (!processed.Add(assembly))
                    {
                        // Already processed, skip it
                        continue;
                    }

                    foreach (var attribute in assembly.GetCustomAttributes<HostingStartupAttribute>())
                    {
                        var hostingStartup = (IHostingStartup)Activator.CreateInstance(attribute.HostingStartupType)!;
                        hostingStartup.Configure(_hostingStartupNodeHostBuilder);
                    }
                }
                catch (Exception ex)
                {
                    // Capture any errors that hengineen during startup
                    exceptions.Add(new InvalidOperationException($"Startup assembly {assemblyName} failed to execute. See the inner exception for more details.", ex));
                }
            }

            if (exceptions.Count > 0)
            {
                _hostingStartupErrors = new AggregateException(exceptions);
            }
        }

        public INodeHostBuilder UseStartup([DynamicallyAccessedMembers(StartupLinkerOptions.Accessibility)] Type startupType)
        {
            var startupAssemblyName = startupType.Assembly.GetName().Name;

            UseSetting(NodeHostDefaults.EngineKey, startupAssemblyName);

            // UseStartup can be called multiple times. Only run the last one.
            _startupObject = startupType;

            _builder.ConfigureServices((context, services) =>
            {
                // Run this delegate if the startup type matches
                if (ReferenceEquals(_startupObject, startupType))
                {
                    UseStartup(startupType, context, services);
                }
            });

            return this;
        }

        // Note: This method isn't 100% compatible with trimming. It is possible for the factory to return a derived type from TStartup.
        // RequiresUnreferencedCode isn't on this method because the majority of people won't do that.
        public INodeHostBuilder UseStartup<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] TStartup>(Func<NodeHostBuilderContext, TStartup> startupFactory)
        {
            var startupAssemblyName = startupFactory.GetMethodInfo().DeclaringType!.Assembly.GetName().Name;

            UseSetting(NodeHostDefaults.EngineKey, startupAssemblyName);

            // Clear the startup type
            _startupObject = startupFactory;

            _builder.ConfigureServices(ConfigureStartup);

            [UnconditionalSuppressMessage("Trimmer", "IL2072", Justification = "Startup type created by factory can't be determined statically.")]
            void ConfigureStartup(HostBuilderContext context, IServiceCollection services)
            {
                // UseStartup can be called multiple times. Only run the last one.
                if (ReferenceEquals(_startupObject, startupFactory))
                {
                    var nodeHostBuilderContext = GetNodeHostBuilderContext(context);
                    var instance = startupFactory(nodeHostBuilderContext) ?? throw new InvalidOperationException("The specified factory returned null startup instance.");
                    UseStartup(instance.GetType(), context, services, instance);
                }
            }

            return this;
        }

        private void UseStartup([DynamicallyAccessedMembers(StartupLinkerOptions.Accessibility)] Type startupType, HostBuilderContext context, IServiceCollection services, object instance = null)
        {
            var nodeHostBuilderContext = GetNodeHostBuilderContext(context);
            var nodeHostOptions = (NodeHostOptions)context.Properties[typeof(NodeHostOptions)];

            ExceptionDispatchInfo startupError = null;
            ConfigureBuilder configureBuilder = null;

            try
            {
                //We cannot support methods that return IServiceProvider as that is terminal and we need ConfigureServices to compose
                if (typeof(INodeStartup).IsAssignableFrom(startupType))
                {
                    throw new NotSupportedException($"{typeof(INodeStartup)} isn't supported");
                }
                if (StartupLoader.HasConfigureServicesIServiceProviderDelegate(startupType, context.HostingEnvironment.EnvironmentName))
                {
                    throw new NotSupportedException($"ConfigureServices returning an {typeof(IServiceProvider)} isn't supported.");
                }

                instance ??= ActivatorUtilities.CreateInstance(new HostServiceProvider(nodeHostBuilderContext), startupType);
                context.Properties[_startupKey] = instance;

                //Startup.ConfigureServices
                var configureServicesBuilder = StartupLoader.FindConfigureServicesDelegate(startupType, context.HostingEnvironment.EnvironmentName);
                var configureServices = configureServicesBuilder.Build(instance);

                configureServices(services);

                //REVIEW: We're doing this in the callback so that we have access to the hosting environment
                //Startup.ConfigureContainer
                var configureContainerBuilder = StartupLoader.FindConfigureContainerDelegate(startupType, context.HostingEnvironment.EnvironmentName);
                if (configureContainerBuilder.MethodInfo != null)
                {
                    // Store the builder in the property bag
                    _builder.Properties[typeof(ConfigureContainerBuilder)] = configureContainerBuilder;

                    InvokeContainer(this, configureContainerBuilder);
                }

                //Resolve Configure after calling ConfigureServices and ConfigureContainer
                configureBuilder = StartupLoader.FindConfigureDelegate(startupType, context.HostingEnvironment.EnvironmentName);
            }
            catch (Exception ex) when (nodeHostOptions.CaptureStartupErrors)
            {
                startupError = ExceptionDispatchInfo.Capture(ex);
            }

            // Startup.Configure
            services.Configure<GenericNodeHostServiceOptions>(options =>
            {
                options.ConfigureEngine = engine =>
                {
                    // Throw if there was any errors initializing startup
                    startupError?.Throw();

                    // Execute Startup.Configure
                    if (instance != null && configureBuilder != null)
                    {
                        configureBuilder.Build(instance)(engine);
                    }
                };
            });

            [UnconditionalSuppressMessage("AOT", "IL3050:RequiresDynamicCode",
                Justification = "There is a runtime check for ValueType startup container. It's unlikely anyone will use a ValueType here.")]
            static void InvokeContainer(GenericNodeHostBuilder genericNodeHostBuilder, ConfigureContainerBuilder configureContainerBuilder)
            {
                var containerType = configureContainerBuilder.GetContainerType();

                // Configure container uses MakeGenericType with the container type. MakeGenericType + struct container type requires IsDynamicCodeSupported.
                if (containerType.IsValueType && !RuntimeFeature.IsDynamicCodeSupported)
                {
                    throw new InvalidOperationException("A ValueType TContainerBuilder isn't supported with AOT.");
                }

                var actionType = typeof(Action<,>).MakeGenericType(typeof(HostBuilderContext), containerType);

                // Get the private ConfigureContainer method on this type then close over the container type
                var configureCallback = typeof(GenericNodeHostBuilder).GetMethod(nameof(ConfigureContainerImpl), BindingFlags.NonPublic | BindingFlags.Instance)!
                                                 .MakeGenericMethod(containerType)
                                                 .CreateDelegate(actionType, genericNodeHostBuilder);

                // _builder.ConfigureContainer<T>(ConfigureContainer);
                typeof(IHostBuilder).GetMethod(nameof(IHostBuilder.ConfigureContainer))!
                    .MakeGenericMethod(containerType)
                    .InvokeWithoutWrappingExceptions(genericNodeHostBuilder._builder, [configureCallback]);
            }
        }

        private void ConfigureContainerImpl<TContainer>(HostBuilderContext context, TContainer container) where TContainer : notnull
        {
            var instance = context.Properties[_startupKey];
            var builder = (ConfigureContainerBuilder)context.Properties[typeof(ConfigureContainerBuilder)];
            //builder.Build(instance)(container);
        }

        public INodeHostBuilder Configure(Action<IEngineBuilder> configure)
        {
            var startupAssemblyName = configure.GetMethodInfo().DeclaringType!.Assembly.GetName().Name!;

            UseSetting(NodeHostDefaults.EngineKey, startupAssemblyName);

            // Clear the startup type
            _startupObject = configure;

            _builder.ConfigureServices((context, services) =>
            {
                if (ReferenceEquals(_startupObject, configure))
                {
                    services.Configure<GenericNodeHostServiceOptions>(options =>
                    {
                        options.ConfigureEngine = engine => configure(engine);
                    });
                }
            });

            return this;
        }

        public INodeHostBuilder Configure(Action<NodeHostBuilderContext, IEngineBuilder> configure)
        {
            var startupAssemblyName = configure.GetMethodInfo().DeclaringType!.Assembly.GetName().Name!;

            UseSetting(NodeHostDefaults.EngineKey, startupAssemblyName);

            // Clear the startup type
            _startupObject = configure;

            _builder.ConfigureServices((context, services) =>
            {
                if (ReferenceEquals(_startupObject, configure))
                {
                    services.Configure<GenericNodeHostServiceOptions>(options =>
                    {
                        var nodehostBuilderContext = GetNodeHostBuilderContext(context);
                        options.ConfigureEngine = engine => configure(nodehostBuilderContext, engine);
                    });
                }
            });

            return this;
        }

        // This exists just so that we can use ActivatorUtilities.CreateInstance on the Startup class
        private sealed class HostServiceProvider : IServiceProvider
        {
            private readonly NodeHostBuilderContext _context;

            public HostServiceProvider(NodeHostBuilderContext context)
            {
                _context = context;
            }

            public object GetService(Type serviceType)
            {
                // The implementation of the HostingEnvironment supports both interfaces
#pragma warning disable CS0618 // Type or member is obsolete
                if (serviceType == typeof(Microsoft.Extensions.Hosting.IHostingEnvironment)
                    || serviceType == typeof(IHostingEnvironment)
#pragma warning restore CS0618 // Type or member is obsolete
                        || serviceType == typeof(INodeHostEnvironment)
                    || serviceType == typeof(IHostEnvironment)
                    )
                {
                    return _context.HostingEnvironment;
                }

                if (serviceType == typeof(IConfiguration))
                {
                    return _context.Configuration;
                }

                return null;
            }
        }
    }
}