using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioPipe.ViewModels
{
    public class DeviceViewModelComparer : IComparer<DeviceViewModel>
    {
        public static DeviceViewModelComparer Instance = new DeviceViewModelComparer();

        public int Compare(DeviceViewModel x, DeviceViewModel y)
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
