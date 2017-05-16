using System.Collections.Generic;

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
