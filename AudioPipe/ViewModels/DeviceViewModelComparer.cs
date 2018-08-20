using System.Collections.Generic;

namespace AudioPipe.ViewModels
{
    /// <summary>
    /// Implements <see cref="IComparer{T}"/> for <see cref="IDeviceViewModel"/> objects.
    /// </summary>
    public class DeviceViewModelComparer : IComparer<IDeviceViewModel>
    {
        /// <summary>
        /// Gets a singleton instance of the <see cref="DeviceViewModelComparer"/> class.
        /// </summary>
        public static DeviceViewModelComparer Instance { get; } = new DeviceViewModelComparer();

        /// <inheritdoc/>
        public int Compare(IDeviceViewModel x, IDeviceViewModel y)
        {
            if (x.IsDefault)
            {
                return 1;
            }
            else if (y.IsDefault)
            {
                return -1;
            }
            else
            {
                return string.CompareOrdinal(x.DeviceName, y.DeviceName);
            }
        }
    }
}
