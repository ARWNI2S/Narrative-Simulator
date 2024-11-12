﻿using System.Reflection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ARWNI2S.Infrastructure.Network.Protocol;
using ARWNI2S.Engine.Network.Middleware;
using ARWNI2S.Engine.Network.Session;

namespace ARWNI2S.Engine.Network.Command
{

    public class CommandMiddleware<TKey, TPackageInfo> : CommandMiddleware<TKey, TPackageInfo, TPackageInfo>
        where TPackageInfo : class, IKeyedPackageInfo<TKey>
    {

        class TransparentMapper : IPackageMapper<TPackageInfo, TPackageInfo>
        {
            public TPackageInfo Map(TPackageInfo package)
            {
                return package;
            }
        }

        public CommandMiddleware(IServiceProvider serviceProvider, IOptions<CommandOptions> commandOptions)
            : base(serviceProvider, commandOptions)
        {

        }

        protected override IPackageMapper<TPackageInfo, TPackageInfo> CreatePackageMapper(IServiceProvider serviceProvider)
        {
            return new TransparentMapper();
        }
    }

    public class CommandMiddleware<TKey, TNetPackageInfo, TPackageInfo> : MiddlewareBase, IPackageHandler<TNetPackageInfo>
        where TPackageInfo : class, IKeyedPackageInfo<TKey>
        where TNetPackageInfo : class
    {
        private Dictionary<TKey, ICommandSet> _commands;

        private ILogger _logger;

        protected IPackageMapper<TNetPackageInfo, TPackageInfo> PackageMapper { get; private set; }

        public CommandMiddleware(IServiceProvider serviceProvider, IOptions<CommandOptions> commandOptions)
            : this(serviceProvider, commandOptions, null)
        {

        }

        public CommandMiddleware(IServiceProvider serviceProvider, IOptions<CommandOptions> commandOptions, IPackageMapper<TNetPackageInfo, TPackageInfo> packageMapper)
        {
            _logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger("CommandMiddleware");

            var sessionFactory = serviceProvider.GetService<ISessionFactory>();
            var sessionType = sessionFactory.SessionType;

            var commandInterfaces = new List<CommandTypeInfo>();
            var commandSetFactories = new List<ICommandSetFactory>();

            var ignorePackageInterfaces = new Type[] { typeof(IKeyedPackageInfo<TKey>) };
            var availablePackageTypes = typeof(TPackageInfo).GetTypeInfo()
                .GetInterfaces()
                .Where(f => !ignorePackageInterfaces.Contains(f))
                .ToList();
            availablePackageTypes.Add(typeof(TPackageInfo));

            var availableSessionTypes = new List<Type> { typeof(INodeSession), sessionType };

            var currentSessionType = sessionType;

            while (true)
            {
                var baseSessionType = currentSessionType.BaseType;

                if (baseSessionType == null || baseSessionType == typeof(object))
                    break;

                availableSessionTypes.Add(baseSessionType);
                currentSessionType = baseSessionType;
            }

            var knownInterfaces = new Type[] { typeof(IKeyedPackageInfo<TKey>) };

            foreach (var pt in availablePackageTypes)
            {
                foreach (var st in availableSessionTypes)
                {
                    RegisterCommandInterfaces(commandInterfaces, commandSetFactories, serviceProvider, st, pt, true);
                }
            }

            commandSetFactories.AddRange(commandOptions.Value.GetCommandTypes(t => true).Select((t) =>
            {
                if (t.IsAbstract)
                    return null;

                for (var i = 0; i < commandInterfaces.Count; i++)
                {
                    var face = commandInterfaces[i];

                    if (face.CommandType.IsAssignableFrom(t))
                        return face.CreateCommandSetFactory(t);
                }

                return null;
            }).Where(t => t != null));


            var commands = commandSetFactories.Select(t => t.Create(serviceProvider, commandOptions.Value));
            var comparer = serviceProvider.GetService<IEqualityComparer<TKey>>();

            var commandDict = comparer == null ?
                [] : new Dictionary<TKey, ICommandSet>(comparer);

            foreach (var cmd in commands)
            {
                if (commandDict.ContainsKey(cmd.Key))
                {
                    var error = $"Duplicated command with Key {cmd.Key} is found: {cmd.ToString()}";
                    _logger.LogError(error);
                    throw new Exception(error);
                }

                commandDict.Add(cmd.Key, cmd);
                _logger.LogDebug($"The command with key {cmd.Key} is registered: {cmd.ToString()}");
            }

            _commands = commandDict;

            PackageMapper = packageMapper != null ? packageMapper : CreatePackageMapper(serviceProvider);
        }

        private void RegisterCommandInterfaces(List<CommandTypeInfo> commandInterfaces, List<ICommandSetFactory> commandSetFactories, IServiceProvider serviceProvider, Type sessionType, Type packageType, bool wrapRequired = false)
        {
            var genericTypes = new[] { sessionType, packageType };

            var commandInterface = typeof(ICommand<,>).GetTypeInfo().MakeGenericType(genericTypes);
            var asyncCommandInterface = typeof(IAsyncCommand<,>).GetTypeInfo().MakeGenericType(genericTypes);

            var commandSetFactoryType = typeof(CommandSetFactory<>).MakeGenericType(typeof(TKey), typeof(TNetPackageInfo), typeof(TPackageInfo), sessionType);

            var syncCommandType = new CommandTypeInfo(typeof(ICommand<,>).GetTypeInfo().MakeGenericType(genericTypes), commandSetFactoryType);
            var asyncCommandType = new CommandTypeInfo(typeof(IAsyncCommand<,>).GetTypeInfo().MakeGenericType(genericTypes), commandSetFactoryType);

            commandInterfaces.Add(syncCommandType);
            commandInterfaces.Add(asyncCommandType);

            if (wrapRequired)
            {
                syncCommandType.WrapRequired = true;
                syncCommandType.WrapFactory = (t) =>
                {
                    return typeof(CommandWrap<,,,>).GetTypeInfo().MakeGenericType(sessionType, typeof(TPackageInfo), packageType, t);
                };

                asyncCommandType.WrapRequired = true;
                asyncCommandType.WrapFactory = (t) =>
                {
                    return typeof(AsyncCommandWrap<,,,>).GetTypeInfo().MakeGenericType(sessionType, typeof(TPackageInfo), packageType, t);
                };
            }

            RegisterCommandSetFactoriesFromServices(commandSetFactories, serviceProvider, syncCommandType.CommandType, commandSetFactoryType, syncCommandType.WrapFactory);
            RegisterCommandSetFactoriesFromServices(commandSetFactories, serviceProvider, asyncCommandType.CommandType, commandSetFactoryType, asyncCommandType.WrapFactory);
        }

        private void RegisterCommandSetFactoriesFromServices(List<ICommandSetFactory> commandSetFactories, IServiceProvider serviceProvider, Type commandType, Type commandSetFactoryType, Func<Type, Type> commandWrapFactory)
        {
            foreach (var command in serviceProvider.GetServices(commandType).OfType<ICommand>())
            {
                var cmd = command;
                var actualCommandType = cmd.GetType();

                if (commandWrapFactory != null)
                {
                    var commandWrapType = commandWrapFactory(command.GetType());
                    cmd = Activator.CreateInstance(commandWrapType, command) as ICommand;
                }

                var commandTypeInfo = new CommandTypeInfo(cmd);
                commandTypeInfo.ActualCommandType = actualCommandType;
                commandSetFactories.Add(Activator.CreateInstance(commandSetFactoryType, commandTypeInfo) as ICommandSetFactory);
            }
        }

        protected virtual IPackageMapper<TNetPackageInfo, TPackageInfo> CreatePackageMapper(IServiceProvider serviceProvider)
        {
            return serviceProvider.GetService<IPackageMapper<TNetPackageInfo, TPackageInfo>>();
        }

        protected virtual async ValueTask HandlePackage(INodeSession session, TPackageInfo package, CancellationToken cancellationToken)
        {
            if (!_commands.TryGetValue(package.Key, out ICommandSet commandSet))
            {
                return;
            }

            await commandSet.ExecuteAsync(session, package, cancellationToken);
        }

        protected virtual async Task OnPackageReceived(INodeSession session, TPackageInfo package, CancellationToken cancellationToken)
        {
            await HandlePackage(session, package, cancellationToken);
        }

        ValueTask IPackageHandler<TNetPackageInfo>.Handle(INodeSession session, TNetPackageInfo package, CancellationToken cancellationToken)
        {
            return HandlePackage(session, PackageMapper.Map(package), cancellationToken);
        }

        interface ICommandSet
        {
            TKey Key { get; }

            ValueTask ExecuteAsync(INodeSession session, TPackageInfo package, CancellationToken cancellationToken);
        }

        class CommandTypeInfo
        {
            public Type CommandType { get; private set; }

            public Type ActualCommandType { get; set; }

            public ICommand Command { get; private set; }

            public Type CommandSetFactoryType { get; private set; }

            public bool WrapRequired { get; set; }

            public Func<Type, Type> WrapFactory { get; set; }

            public CommandTypeInfo(ICommand command)
            {
                Command = command;
                CommandType = command.GetType();
            }

            public CommandTypeInfo(Type commandType, Type commandSetFactoryType)
                : this(commandType, commandSetFactoryType, false)
            {

            }

            public CommandTypeInfo(Type commandType, Type commandSetFactoryType, bool wrapRequired)
            {
                CommandType = commandType;
                CommandSetFactoryType = commandSetFactoryType;
                WrapRequired = wrapRequired;
            }

            public ICommandSetFactory CreateCommandSetFactory(Type type)
            {
                var commandTyeInfo = new CommandTypeInfo(WrapRequired ? WrapFactory(type) : type, null);
                commandTyeInfo.ActualCommandType = type;
                return Activator.CreateInstance(CommandSetFactoryType, commandTyeInfo) as ICommandSetFactory;
            }
        }

        interface ICommandSetFactory
        {
            ICommandSet Create(IServiceProvider serviceProvider, CommandOptions commandOptions);
        }

        class CommandSetFactory<TAppSession> : ICommandSetFactory
            where TAppSession : INodeSession

        {
            public CommandTypeInfo CommandType { get; private set; }

            public CommandSetFactory(CommandTypeInfo commandType)
            {
                CommandType = commandType;
            }

            public ICommandSet Create(IServiceProvider serviceProvider, CommandOptions commandOptions)
            {
                var commandSet = new CommandSet<TAppSession>();
                commandSet.Initialize(serviceProvider, CommandType, commandOptions);
                return commandSet;
            }
        }

        class CommandSet<TAppSession> : ICommandSet
            where TAppSession : INodeSession
        {
            public IAsyncCommand<TAppSession, TPackageInfo> AsyncCommand { get; private set; }

            public ICommand<TAppSession, TPackageInfo> Command { get; private set; }

            public IReadOnlyList<ICommandFilter> Filters { get; private set; }

            public CommandMetadata Metadata { get; private set; }

            public TKey Key { get; private set; }

            private readonly bool _isKeyString = false;

            public CommandSet()
            {
                _isKeyString = typeof(TKey) == typeof(string);
            }

            private CommandMetadata GetCommandMetadata(Type commandType)
            {
                var cmdAtt = commandType.GetCustomAttribute(typeof(CommandAttribute)) as CommandAttribute;
                var cmdMeta = default(CommandMetadata);

                if (cmdAtt == null)
                {
                    if (!_isKeyString)
                    {
                        throw new Exception($"The command {commandType.FullName} needs a CommandAttribute defined.");
                    }

                    cmdMeta = new CommandMetadata(commandType.Name, commandType.Name);
                }
                else
                {
                    var cmdName = cmdAtt.Name;

                    if (string.IsNullOrEmpty(cmdName))
                        cmdName = commandType.Name;

                    if (cmdAtt.Key == null)
                    {
                        if (!_isKeyString)
                        {
                            throw new Exception($"The command {commandType.FullName} needs a Key in type '{typeof(TKey).Name}' defined in its CommandAttribute.");
                        }

                        cmdMeta = new CommandMetadata(cmdName, cmdName);
                    }
                    else
                    {
                        cmdMeta = new CommandMetadata(cmdName, cmdAtt.Key);
                    }
                }

                return cmdMeta;
            }

            protected void SetCommand(ICommand command)
            {
                Command = command as ICommand<TAppSession, TPackageInfo>;
                AsyncCommand = command as IAsyncCommand<TAppSession, TPackageInfo>;
            }

            public void Initialize(IServiceProvider serviceProvider, CommandTypeInfo commandTypeInfo, CommandOptions commandOptions)
            {
                var command = commandTypeInfo.Command;

                if (command == null)
                {
                    if (commandTypeInfo.CommandType != commandTypeInfo.ActualCommandType)
                    {
                        var commandFactory = ActivatorUtilities.CreateFactory(commandTypeInfo.CommandType, [typeof(IServiceProvider)]);
                        command = commandFactory.Invoke(serviceProvider, [serviceProvider]) as ICommand;
                    }
                    else
                    {
                        command = ActivatorUtilities.CreateInstance(serviceProvider, commandTypeInfo.CommandType) as ICommand;
                    }
                }

                SetCommand(command);

                var cmdMeta = GetCommandMetadata(commandTypeInfo.ActualCommandType);

                try
                {
                    Key = (TKey)cmdMeta.Key;
                    Metadata = cmdMeta;
                }
                catch (Exception e)
                {
                    throw new Exception($"The command {cmdMeta.Name}'s Key {cmdMeta.Key} cannot be converted to the desired type '{typeof(TKey).Name}'.", e);
                }

                var filters = new List<ICommandFilter>();

                if (commandOptions.GlobalCommandFilterTypes.Any())
                    filters.AddRange(commandOptions.GlobalCommandFilterTypes.Select(t => ActivatorUtilities.CreateInstance(serviceProvider, t) as CommandFilterBaseAttribute));

                filters.AddRange(commandTypeInfo.ActualCommandType.GetCustomAttributes(false).OfType<CommandFilterBaseAttribute>());
                Filters = filters;
            }

            public async ValueTask ExecuteAsync(INodeSession session, TPackageInfo package, CancellationToken cancellationToken)
            {
                if (Filters.Count > 0)
                {
                    await ExecuteAsyncWithFilter(session, package, cancellationToken);
                    return;
                }

                var appSession = (TAppSession)session;

                var asyncCommand = AsyncCommand;

                if (asyncCommand != null)
                {
                    await asyncCommand.ExecuteAsync(appSession, package, cancellationToken);
                    return;
                }

                Command.Execute(appSession, package);
            }

            private async ValueTask ExecuteAsyncWithFilter(INodeSession session, TPackageInfo package, CancellationToken cancellationToken)
            {
                var context = new CommandExecutingContext();
                context.Package = package;
                context.Session = session;
                context.CancellationToken = cancellationToken;

                var command = AsyncCommand != null ? AsyncCommand as ICommand : Command as ICommand;

                if (command is ICommandWrap commandWrap)
                    command = commandWrap.InnerCommand;

                context.CurrentCommand = command;

                var filters = Filters;

                var continued = true;

                for (var i = 0; i < filters.Count; i++)
                {
                    var f = filters[i];

                    if (f is AsyncCommandFilterAttribute asyncCommandFilter)
                    {
                        continued = await asyncCommandFilter.OnCommandExecutingAsync(context);
                    }
                    else if (f is CommandFilterAttribute commandFilter)
                    {
                        continued = commandFilter.OnCommandExecuting(context);
                    }

                    if (!continued)
                        break;
                }

                if (!continued)
                    return;

                try
                {
                    var appSession = (TAppSession)session;
                    var asyncCommand = AsyncCommand;

                    if (asyncCommand != null)
                    {
                        await asyncCommand.ExecuteAsync(appSession, package, cancellationToken);
                    }
                    else
                    {
                        Command.Execute(appSession, package);
                    }
                }
                catch (Exception e)
                {
                    context.Exception = e;
                }
                finally
                {
                    for (var i = 0; i < filters.Count; i++)
                    {
                        var f = filters[i];

                        if (f is AsyncCommandFilterAttribute asyncCommandFilter)
                        {
                            await asyncCommandFilter.OnCommandExecutedAsync(context);
                        }
                        else if (f is CommandFilterAttribute commandFilter)
                        {
                            commandFilter.OnCommandExecuted(context);
                        }
                    }
                }
            }

            public override string ToString()
            {
                ICommand command = Command;

                if (command == null)
                    command = AsyncCommand;

                return command?.GetType().ToString();
            }
        }
    }
}
