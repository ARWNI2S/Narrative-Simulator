using System.Reflection;

namespace ARWNI2S.Engine.EngineParts
{
    /// <summary>
    /// Exposes a set of types from an <see cref="EnginePart"/>.
    /// </summary>
    public interface IEnginePartTypeProvider
    {
        /// <summary>
        /// Gets the list of available types in the <see cref="EnginePart"/>.
        /// </summary>
        IEnumerable<TypeInfo> Types { get; }
    }
}
