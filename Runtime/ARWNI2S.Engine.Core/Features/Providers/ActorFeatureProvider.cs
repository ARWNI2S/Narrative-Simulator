using ARWNI2S.Engine.Attributes;
using ARWNI2S.Infrastructure.Engine.Features;
using ARWNI2S.Infrastructure.EngineParts;
using System.Reflection;

namespace ARWNI2S.Engine.Features.Providers
{
    /// <summary>
    /// Discovers controllers from a list of <see cref="EnginePart"/> instances.
    /// </summary>
    public class ActorFeatureProvider : IEngineFeatureProvider<ActorFeature>
    {
        private const string ControllerTypeNameSuffix = "Controller";

        /// <inheritdoc />
        public void PopulateFeature(
            IEnumerable<EnginePart> parts,
            ActorFeature feature)
        {
            foreach (var part in parts.OfType<IEnginePartTypeProvider>())
            {
                foreach (var type in part.Types)
                {
                    if (IsController(type) && !feature.Controllers.Contains(type))
                    {
                        feature.Controllers.Add(type);
                    }
                }
            }
        }

        /// <summary>
        /// Determines if a given <paramref name="typeInfo"/> is a controller.
        /// </summary>
        /// <param name="typeInfo">The <see cref="TypeInfo"/> candidate.</param>
        /// <returns><see langword="true" /> if the type is a controller; otherwise <see langword="false" />.</returns>
        protected virtual bool IsController(TypeInfo typeInfo)
        {
            if (!typeInfo.IsClass)
            {
                return false;
            }

            if (typeInfo.IsAbstract)
            {
                return false;
            }

            // We only consider public top-level classes as controllers. IsPublic returns false for nested
            // classes, regardless of visibility modifiers
            if (!typeInfo.IsPublic)
            {
                return false;
            }

            if (typeInfo.ContainsGenericParameters)
            {
                return false;
            }

            if (typeInfo.IsDefined(typeof(NonActorAttribute)))
            {
                return false;
            }

            if (!typeInfo.Name.EndsWith(ControllerTypeNameSuffix, StringComparison.OrdinalIgnoreCase) &&
                !typeInfo.IsDefined(typeof(ActorAttribute)))
            {
                return false;
            }

            return true;
        }
    }
}
