using NAudio.CoreAudioApi;
using System.Collections.Generic;

namespace AudioPipe.Services
{
    /// <summary>
    /// <see cref="IEqualityComparer{T}"/> for <see cref="MMDevice"/> objects.
    /// </summary>
    public class MMDeviceEqualityComparer : IEqualityComparer<MMDevice>
    {
        /// <summary>
        /// Gets a singleton instance of an <see cref="MMDeviceEqualityComparer"/>.
        /// </summary>
        public static MMDeviceEqualityComparer Instance { get; } = new MMDeviceEqualityComparer();

        /// <inheritdoc/>
        public bool Equals(MMDevice x, MMDevice y)
        {
            return x?.ID == y?.ID;
        }

        /// <inheritdoc/>
        public int GetHashCode(MMDevice obj)
        {
            return obj.ID.GetHashCode();
        }
    }
}
