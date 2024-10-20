﻿using ARWNI2S.Engine.Features;
using System.Reflection;

namespace ARWNI2S.Engine.EngineParts
{
    /// <summary>
    /// Manages the parts and features of an NI2S application.
    /// </summary>
    public class EnginePartManager
    {
        /// <summary>
        /// Gets the list of <see cref="IEngineFeatureProvider"/>s.
        /// </summary>
        public IList<IEngineFeatureProvider> FeatureProviders { get; } =
            [];

        /// <summary>
        /// Gets the list of <see cref="EnginePart"/> instances.
        /// <para>
        /// Instances in this collection are stored in precedence order. An <see cref="EnginePart"/> that appears
        /// earlier in the list has a higher precedence.
        /// An <see cref="IEngineFeatureProvider"/> may choose to use this an interface as a way to resolve conflicts when
        /// multiple <see cref="EnginePart"/> instances resolve equivalent feature values.
        /// </para>
        /// </summary>
        public IList<EnginePart> ApplicationParts { get; } = [];

        /// <summary>
        /// Populates the given <paramref name="feature"/> using the list of
        /// <see cref="IApplicationFeatureProvider{TFeature}"/>s configured on the
        /// <see cref="EnginePartManager"/>.
        /// </summary>
        /// <typeparam name="TFeature">The type of the feature.</typeparam>
        /// <param name="feature">The feature instance to populate.</param>
        public void PopulateFeature<TFeature>(TFeature feature)
        {
            if (feature == null)
            {
                throw new ArgumentNullException(nameof(feature));
            }

            foreach (var provider in FeatureProviders.OfType<IApplicationFeatureProvider<TFeature>>())
            {
                provider.PopulateFeature(ApplicationParts, feature);
            }
        }

        internal void PopulateDefaultParts(string entryAssemblyName)
        {
            var assemblies = GetApplicationPartAssemblies(entryAssemblyName);

            var seenAssemblies = new HashSet<Assembly>();

            foreach (var assembly in assemblies)
            {
                if (!seenAssemblies.Add(assembly))
                {
                    // "assemblies" may contain duplicate values, but we want unique ApplicationPart instances.
                    // Note that we prefer using a HashSet over Distinct since the latter isn't
                    // guaranteed to preserve the original ordering.
                    continue;
                }

                var partFactory = EnginePartFactory.GetApplicationPartFactory(assembly);
                foreach (var applicationPart in partFactory.GetApplicationParts(assembly))
                {
                    ApplicationParts.Add(applicationPart);
                }
            }
        }

        private static IEnumerable<Assembly> GetApplicationPartAssemblies(string entryAssemblyName)
        {
            var entryAssembly = Assembly.Load(new AssemblyName(entryAssemblyName));

            // Use ApplicationPartAttribute to get the closure of direct or transitive dependencies
            // that reference NI2S.
            var assembliesFromAttributes = entryAssembly.GetCustomAttributes<EnginePartAttribute>()
                .Select(name => Assembly.Load(name.AssemblyName))
                .OrderBy(assembly => assembly.FullName, StringComparer.Ordinal)
                .SelectMany(GetAssemblyClosure);

            // The SDK will not include the entry assembly as an application part. We'll explicitly list it
            // and have it appear before all other assemblies \ ApplicationParts.
            return GetAssemblyClosure(entryAssembly)
                .Concat(assembliesFromAttributes);
        }

        private static IEnumerable<Assembly> GetAssemblyClosure(Assembly assembly)
        {
            yield return assembly;

            var relatedAssemblies = RelatedAssemblyAttribute.GetRelatedAssemblies(assembly, throwOnError: false)
                .OrderBy(assembly => assembly.FullName, StringComparer.Ordinal);

            foreach (var relatedAssembly in relatedAssemblies)
            {
                yield return relatedAssembly;
            }
        }
    }
}
