using ARWNI2S.Engine.Hosting;

namespace ARWNI2S.Engine.Events
{
    internal delegate void SimulationEventDelegate(params object[] data);

    internal sealed class SimulationEvent : IDisposable, IEquatable<SimulationEvent>, IComparable<SimulationEvent>
    {
        /// <inheritdoc />
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public int CompareTo(SimulationEvent? other)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public bool Equals(SimulationEvent? other)
        {
            /* critical */

            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return GetHashCode().Equals(other.GetHashCode());

            /* critical */
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            /* critical */

            if (obj is null) return false;
            if (obj is not SimulationEvent) return false;

            if (ReferenceEquals(this, obj)) return true;

            return GetHashCode().Equals(obj.GetHashCode());

            /* critical */
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            // TODO : Implement equallity
            /* critical */

            return base.GetHashCode();

            /* critical */
        }

        /// <inheritdoc />
        public override string ToString()
        {
            // TODO : Implement quick stats string generation.
            /* critical */

            return nameof(SimulationHostedService);

            /* critical */
        }
    }
}
