using System.Reflection;

namespace ARWNI2S.Engine.EngineParts
{
    /// <summary>
    /// Specifies a contract for synthesizing one or more <see cref="EnginePart"/> instances
    /// from an <see cref="Assembly"/>.
    /// <para>
    /// By default, NI2S registers each application assembly that it discovers as an <see cref="AssemblyPart"/>.
    /// Assemblies can optionally specify an <see cref="EnginePartFactory"/> to configure parts for the assembly
    /// by using <see cref="ProvideEnginePartFactoryAttribute"/>.
    /// </para>
    /// </summary>
    public abstract class EnginePartFactory
    {
        /// <summary>
        /// Gets one or more <see cref="EnginePart"/> instances for the specified <paramref name="assembly"/>.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly"/>.</param>
        public abstract IEnumerable<EnginePart> GetApplicationParts(Assembly assembly);

        /// <summary>
        /// Gets the <see cref="EnginePartFactory"/> for the specified assembly.
        /// <para>
        /// An assembly may specify an <see cref="EnginePartFactory"/> using <see cref="ProvideEnginePartFactoryAttribute"/>.
        /// Otherwise, <see cref="DefaultEnginePartFactory"/> is used.
        /// </para>
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly"/>.</param>
        /// <returns>An instance of <see cref="EnginePartFactory"/>.</returns>
        public static EnginePartFactory GetApplicationPartFactory(Assembly assembly)
        {
            ArgumentNullException.ThrowIfNull(assembly);

            var provideAttribute = assembly.GetCustomAttribute<ProvideEnginePartFactoryAttribute>();
            if (provideAttribute == null)
            {
                return DefaultEnginePartFactory.Instance;
            }

            var type = provideAttribute.GetFactoryType();
            if (!typeof(EnginePartFactory).IsAssignableFrom(type))
            {
                //TODO : RECHOURZES
                throw new InvalidOperationException(/*Resources.FormatApplicationPartFactory_InvalidFactoryType(
                    type,
                    nameof(ProvideApplicationPartFactoryAttribute),
                    typeof(ApplicationPartFactory))*/);
            }

            return (EnginePartFactory)Activator.CreateInstance(type)!;
        }
    }
}
