using System.Reflection;

namespace ARWNI2S.Engine.EngineParts
{
    /// <summary>
    /// An <see cref="EnginePart"/> backed by an <see cref="System.Reflection.Assembly"/>.
    /// </summary>
    public class AssemblyPart : EnginePart, IEnginePartTypeProvider
    {
        /// <summary>
        /// Initializes a new <see cref="AssemblyPart"/> instance.
        /// </summary>
        /// <param name="assembly">The backing <see cref="System.Reflection.Assembly"/>.</param>
        public AssemblyPart(Assembly assembly)
        {
            Assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
        }

        /// <summary>
        /// Gets the <see cref="Assembly"/> of the <see cref="EnginePart"/>.
        /// </summary>
        public Assembly Assembly { get; }

        /// <summary>
        /// Gets the name of the <see cref="EnginePart"/>.
        /// </summary>
        public override string Name => Assembly.GetName().Name!;

        /// <inheritdoc />
        public IEnumerable<TypeInfo> Types => Assembly.DefinedTypes;
    }
}
