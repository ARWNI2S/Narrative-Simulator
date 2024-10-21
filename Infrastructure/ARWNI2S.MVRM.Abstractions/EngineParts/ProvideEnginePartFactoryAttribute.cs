using ARWNI2S.Engine.Properties;

namespace ARWNI2S.Engine.EngineParts
{
    /// <summary>
    /// Provides a <see cref="EnginePartFactory"/> type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public sealed class ProvideEnginePartFactoryAttribute : Attribute
    {
        private readonly Type _applicationPartFactoryType;
        private readonly string _applicationPartFactoryTypeName;

        /// <summary>
        /// Creates a new instance of <see cref="ProvideEnginePartFactoryAttribute"/> with the specified type.
        /// </summary>
        /// <param name="factoryType">The factory type.</param>
        public ProvideEnginePartFactoryAttribute(Type factoryType)
        {
            _applicationPartFactoryType = factoryType ?? throw new ArgumentNullException(nameof(factoryType));
        }

        /// <summary>
        /// Creates a new instance of <see cref="ProvideEnginePartFactoryAttribute"/> with the specified type name.
        /// </summary>
        /// <param name="factoryTypeName">The assembly qualified type name.</param>
        public ProvideEnginePartFactoryAttribute(string factoryTypeName)
        {
            if (string.IsNullOrEmpty(factoryTypeName))
            {
                throw new ArgumentException(Resources.ArgumentCannotBeNullOrEmpty, nameof(factoryTypeName));
            }

            _applicationPartFactoryTypeName = factoryTypeName;
        }

        /// <summary>
        /// Gets the factory type.
        /// </summary>
        /// <returns></returns>
        public Type GetFactoryType()
        {
            return _applicationPartFactoryType ??
                Type.GetType(_applicationPartFactoryTypeName!, throwOnError: true)!;
        }
    }
}
