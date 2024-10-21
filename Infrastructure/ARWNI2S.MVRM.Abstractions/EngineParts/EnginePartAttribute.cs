namespace ARWNI2S.Engine.EngineParts
{
    /// <summary>
    /// Specifies an assembly to be added as an <see cref="EnginePart" />.
    /// <para>
    /// In the ordinary case, MVC will generate <see cref="EnginePartAttribute" />
    /// instances on the entry assembly for each dependency that references MVC.
    /// Each of these assemblies is treated as an <see cref="EnginePart" />.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class EnginePartAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="EnginePartAttribute" />.
        /// </summary>
        /// <param name="assemblyName">The assembly name.</param>
        public EnginePartAttribute(string assemblyName)
        {
            AssemblyName = assemblyName ?? throw new ArgumentNullException(nameof(assemblyName));
        }

        /// <summary>
        /// Gets the assembly name.
        /// </summary>
        public string AssemblyName { get; }
    }
}
