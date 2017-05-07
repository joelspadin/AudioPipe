using CSCore.CoreAudioAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioPipe.Services
{
    public static class DeviceService
    {
        private static MMDeviceEnumerator _deviceEnum = new MMDeviceEnumerator();

        // TODO: refresh the cache when things change
        private static MMDevice _defaultCaptureDevice;

        public static MMDevice GetDefaultCaptureDevice()
        {
            if (_defaultCaptureDevice == null)
            {
                _defaultCaptureDevice = _deviceEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
            }
            return _defaultCaptureDevice;
        }

        public static MMDeviceCollection GetOutputDevices()
        {
            return _deviceEnum.EnumAudioEndpoints(DataFlow.Render, DeviceState.Active);
        }

        public static bool Equals(MMDevice x, MMDevice y)
        {
            return MMDeviceEqualityComparer.Instance.Equals(x, y);
        }
    }

    public class MMDeviceEqualityComparer : IEqualityComparer<MMDevice>
    {
        public static MMDeviceEqualityComparer Instance = new MMDeviceEqualityComparer();

        public bool Equals(MMDevice x, MMDevice y)
        {
            return x?.DeviceID == y?.DeviceID;
        }

        public int GetHashCode(MMDevice obj)
        {
            return obj.DeviceID.GetHashCode();
        }
    }
}
