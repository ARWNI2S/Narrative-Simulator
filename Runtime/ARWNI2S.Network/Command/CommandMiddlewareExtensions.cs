using ARWNI2S.Engine.Network.Host;
using ARWNI2S.Infrastructure.Network.Protocol;
using Microsoft.Extensions.DependencyInjection;

namespace ARWNI2S.Engine.Network.Command
{
    public static class CommandMiddlewareExtensions
    {
        public static Type GetKeyType<TPackageInfo>()
        {
            var interfaces = typeof(TPackageInfo).GetInterfaces();
            var keyInterface = interfaces.FirstOrDefault(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IKeyedPackageInfo<>));

            if (keyInterface == null)
                throw new Exception($"The package type {nameof(TPackageInfo)} should implement the interface {typeof(IKeyedPackageInfo<>).Name}.");

            return keyInterface.GetGenericArguments().FirstOrDefault();
        }

        private static INodeServerHostBuilder ConfigureCommand(this INodeServerHostBuilder builder)
        {
            return builder.ConfigureServices((hostCxt, services) =>
                {
                    services.Configure<CommandOptions>(hostCxt.Configuration?.GetSection("serverOptions")?.GetSection("commands"));
                }) as INodeServerHostBuilder;
        }

        public static INodeServerHostBuilder<TPackageInfo> UseCommand<TPackageInfo>(this INodeServerHostBuilder<TPackageInfo> builder)
            where TPackageInfo : class
        {
            var keyType = GetKeyType<TPackageInfo>();

            var useCommandMethod = typeof(CommandMiddlewareExtensions).GetMethod("UseCommand", [typeof(INodeServerHostBuilder)]);
            useCommandMethod = useCommandMethod.MakeGenericMethod(keyType, typeof(TPackageInfo));

            var hostBuilder = useCommandMethod.Invoke(null, [builder]) as INodeServerHostBuilder;
            return hostBuilder.ConfigureCommand() as INodeServerHostBuilder<TPackageInfo>;
        }

        public static INodeServerHostBuilder<TPackageInfo> UseCommand<TPackageInfo>(this INodeServerHostBuilder<TPackageInfo> builder, Action<CommandOptions> configurator)
            where TPackageInfo : class
        {
            return builder.UseCommand()
               .ConfigureServices((hostCtx, services) =>
               {
                   services.Configure(configurator);
               }) as INodeServerHostBuilder<TPackageInfo>;
        }

        public static INodeServerHostBuilder<TPackageInfo> UseCommand<TKey, TPackageInfo>(this INodeServerHostBuilder<TPackageInfo> builder, Action<CommandOptions> configurator, IEqualityComparer<TKey> comparer)
            where TPackageInfo : class, IKeyedPackageInfo<TKey>
        {
            return builder.UseCommand(configurator)
                .ConfigureServices((hostCtx, services) =>
                {
                    services.AddSingleton<IEqualityComparer<TKey>>(comparer);
                }) as INodeServerHostBuilder<TPackageInfo>;
        }

        public static INodeServerHostBuilder<TPackageInfo> UseCommand<TKey, TPackageInfo>(this INodeServerHostBuilder builder)
            where TPackageInfo : class, IKeyedPackageInfo<TKey>
        {
            return builder.UseMiddleware<CommandMiddleware<TKey, TPackageInfo>>()
                .ConfigureCommand() as INodeServerHostBuilder<TPackageInfo>;
        }

        public static INodeServerHostBuilder<TPackageInfo> UseCommand<TKey, TPackageInfo>(this INodeServerHostBuilder builder, Action<CommandOptions> configurator)
            where TPackageInfo : class, IKeyedPackageInfo<TKey>
        {
            return builder.UseCommand<TKey, TPackageInfo>()
               .ConfigureServices((hostCtx, services) =>
               {
                   services.Configure(configurator);
               }) as INodeServerHostBuilder<TPackageInfo>;
        }

        public static INodeServerHostBuilder<TPackageInfo> UseCommand<TKey, TPackageInfo>(this INodeServerHostBuilder builder, Action<CommandOptions> configurator, IEqualityComparer<TKey> comparer)
            where TPackageInfo : class, IKeyedPackageInfo<TKey>
        {
            return builder.UseCommand<TKey, TPackageInfo>(configurator)
                .ConfigureServices((hostCtx, services) =>
                {
                    services.AddSingleton<IEqualityComparer<TKey>>(comparer);
                }) as INodeServerHostBuilder<TPackageInfo>;
        }
    }
}
