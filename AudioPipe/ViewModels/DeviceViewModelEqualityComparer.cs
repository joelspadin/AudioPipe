using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioPipe.ViewModels
{
    public class DeviceViewModelEqualityComparer : IEqualityComparer<DeviceViewModel>
    {
        public static DeviceViewModelEqualityComparer Instance = new DeviceViewModelEqualityComparer();

        public bool Equals(DeviceViewModel x, DeviceViewModel y)
        {
            return x?.Device?.DeviceID == y?.Device?.DeviceID;
        }

        public int GetHashCode(DeviceViewModel obj)
        {
            return obj?.Device?.DeviceID.GetHashCode() ?? 0;
        }
    }
}
