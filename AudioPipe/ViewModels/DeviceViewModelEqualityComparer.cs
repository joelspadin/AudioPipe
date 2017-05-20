using System.Collections.Generic;

namespace AudioPipe.ViewModels
{
    public class DeviceViewModelEqualityComparer : IEqualityComparer<IDeviceViewModel>
    {
        public static DeviceViewModelEqualityComparer Instance = new DeviceViewModelEqualityComparer();

        public bool Equals(IDeviceViewModel x, IDeviceViewModel y)
        {
            return x?.Device?.DeviceID == y?.Device?.DeviceID;
        }

        public int GetHashCode(IDeviceViewModel obj)
        {
            return obj?.Device?.DeviceID.GetHashCode() ?? 0;
        }
    }
}
