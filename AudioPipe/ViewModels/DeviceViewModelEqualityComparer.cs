using System.Collections.Generic;

namespace AudioPipe.ViewModels
{
    /// <summary>
    /// Implements <see cref="IEqualityComparer{T}"/> for <see cref="IDeviceViewModel"/> objects.
    /// </summary>
    public class DeviceViewModelEqualityComparer : IEqualityComparer<IDeviceViewModel>
    {
        /// <summary>
        /// Gets a singleton instance of the <see cref="DeviceViewModelEqualityComparer"/> class.
        /// </summary>
        public static DeviceViewModelEqualityComparer Instance { get; } = new DeviceViewModelEqualityComparer();

        /// <inheritdoc/>
        public bool Equals(IDeviceViewModel x, IDeviceViewModel y)
        {
            return x?.Device?.ID == y?.Device?.ID;
        }

        /// <inheritdoc/>
        public int GetHashCode(IDeviceViewModel obj)
        {
            return obj?.Device?.ID.GetHashCode() ?? 0;
        }
    }
}
