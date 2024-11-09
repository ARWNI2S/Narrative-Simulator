using System.Reflection;

namespace ARWNI2S.Engine.EngineParts
{
    /// <summary>
    /// Default <see cref="EnginePartFactory"/>.
    /// </summary>
    public class DefaultEnginePartFactory : EnginePartFactory
    {
        /// <summary>
        /// Gets an instance of <see cref="DefaultEnginePartFactory"/>.
        /// </summary>
        public static DefaultEnginePartFactory Instance { get; } = new DefaultEnginePartFactory();

        /// <summary>
        /// Gets the sequence of <see cref="EnginePart"/> instances that are created by this instance of <see cref="DefaultEnginePartFactory"/>.
        /// <para>
        /// Applications may use this method to get the same behavior as this factory produces during MVRM's default part discovery.
        /// </para>
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly"/>.</param>
        /// <returns>The sequence of <see cref="EnginePart"/> instances.</returns>
        public static IEnumerable<EnginePart> GetDefaultApplicationParts(Assembly assembly)
        {
            ArgumentNullException.ThrowIfNull(assembly);

            yield return new AssemblyPart(assembly);
        }

        /// <inheritdoc />
        public override IEnumerable<EnginePart> GetApplicationParts(Assembly assembly)
        {
            return GetDefaultApplicationParts(assembly);
        }
    }
}
