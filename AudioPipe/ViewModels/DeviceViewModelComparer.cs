using System;
using System.Collections.Generic;

namespace AudioPipe.ViewModels
{
    public class DeviceViewModelComparer : IComparer<IDeviceViewModel>
    {
        public static DeviceViewModelComparer Instance = new DeviceViewModelComparer();

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
                return String.CompareOrdinal(x.DeviceName, y.DeviceName);
            }
        }
    }
}
