namespace ARWNI2S.Engine.Simulation
{
    /// <summary>
    /// Simulation LOD levels
    /// </summary>
    public enum LODLevel : int
    {
        /// <summary>
        /// Disabled level of detail
        /// </summary>
        /// <remarks>
        /// Ensures the usage of <see cref="Resolution.Frequency"/>.
        /// If <see cref="Resolution.Frequency"/> set to 0 or less, it effectively disables simulation updates.
        /// </remarks>
        DISABLED = int.MinValue,
        /// <summary>
        /// Minimum level of detail
        /// </summary>
        /// <remarks>Used for general inactive actor periodical updating. Ensures that every actor in the simulation updates once each frame.</remarks>
        LOD_0 = int.MinValue + 1,
        /// <summary>
        /// Minimal level of detail
        /// </summary>
        /// <remarks>Used for a more detailed out-of-scene AI dependant/required behaviour updates</remarks>
        MINIMAL = 1000,
        /// <summary>
        /// Background level of detail
        /// </summary>
        /// <remarks>Used widely for latent backstage behaviours and out-of-scene frequent updates</remarks>
        BACKGROUND = 2000,
        /// <summary>
        /// Simulable events level of detail.
        /// </summary>
        /// <remarks>Intermediate level of detail used to simulate any interactable out-of-scene behaviours.</remarks>
        EVENTS = 5000,
        /// <summary>
        /// On-stage scene, non-interactive level of detail.
        /// </summary>
        /// <remarks>Non interactable behaviour updates.</remarks>
        STAGED = 8000,
        /// <summary>
        /// Scene-interactive level of detail.
        /// </summary>
        /// <remarks>Interactable behaviour updates.</remarks>
        SCENE = 9000,
        /// <summary>
        /// Maximum available level of detail.
        /// </summary>
        /// <remarks>
        /// Ensures the usage of <see cref="Resolution.Frequency"/>.
        /// If <see cref="Resolution.Frequency"/> set to 0 or less, it effectively forces idle and per-cycle updates.
        /// </remarks>
        LOD_MAX = int.MaxValue
    }

    public struct Resolution
    {
        public static Resolution Inactive => new((int)LODLevel.DISABLED, -1);
        public static Resolution UpdateOnce => new((int)LODLevel.LOD_0, 0);
        public static Resolution EnsureBehaviour => new((int)LODLevel.MINIMAL, 0);
        public static Resolution LatentBehaviour => new((int)LODLevel.BACKGROUND, 0);
        public static Resolution InteractableBehaviour => new((int)LODLevel.EVENTS, 0);
        public static Resolution StagedActor => new((int)LODLevel.STAGED, 0);
        public static Resolution PreScene => new((int)LODLevel.SCENE - 1, 0);
        public static Resolution InScene => new((int)LODLevel.SCENE, 0);
        public static Resolution EnsureAlways => new((int)LODLevel.LOD_MAX, 0);
        public static Resolution OnePerSecond => new((int)LODLevel.LOD_MAX, 1000);

        internal int _level;
        internal double _resolutionMs;

        private Resolution(int level, double resolutionMs)
        {
            _level = level;
            _resolutionMs = resolutionMs;
        }

        public readonly LODLevel Level => (LODLevel)_level;

        public readonly int Frequency => (int)(_resolutionMs * 1000);

        #region Operators

        public static bool operator ==(Resolution lhs, Resolution rhs)
        {
            return lhs.Level == rhs.Level;
        }
        public static bool operator <(Resolution lhs, Resolution rhs)
        {
            return lhs.Level < rhs.Level;
        }
        public static bool operator >(Resolution lhs, Resolution rhs)
        {
            return lhs.Level > rhs.Level;
        }
        public static bool operator !=(Resolution lhs, Resolution rhs)
        {
            return lhs.Level != rhs.Level;
        }

        public override readonly bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is not Resolution lodObj)
                return false;

            return Level.Equals(lodObj.Level);
        }

        public override readonly int GetHashCode()
        {
            return Level.GetHashCode();
        }

        #endregion
    }

    public static class ResolutionExtensions
    {
        public static Resolution EnsurePerSecondUpdates(this Resolution resolution, int frequencyHz)
        {
            resolution._resolutionMs = 1000.0 / (double)frequencyHz;
            return resolution;
        }
    }

}
