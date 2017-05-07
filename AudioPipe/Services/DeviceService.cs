using CSCore.CoreAudioAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSCore.Win32;
using System.Diagnostics;

namespace AudioPipe.Services
{
    public sealed class DeviceService : IMMNotificationClient
    {
        public static event Action DefaultCaptureDeviceChanged;
        public static event Action OutputDevicesChanged;

        public static MMDevice DefaultCaptureDevice => Instance.GetDefaultCaptureDevice();

        public static MMDeviceCollection GetOutputDevices()
        {
            return Instance._deviceEnum.EnumAudioEndpoints(DataFlow.Render, DeviceState.Active);
        }

        public static bool Equals(MMDevice x, MMDevice y)
        {
            return MMDeviceEqualityComparer.Instance.Equals(x, y);
        }

        private static DeviceService Instance { get; } = new DeviceService();

        private const DataFlow CaptureDeviceDataFlow = DataFlow.Render;
        private const Role CaptureDeviceRole = Role.Console;

        private readonly MMDeviceEnumerator _deviceEnum = new MMDeviceEnumerator();
        private readonly object _lock = new Object();
        private MMDevice _defaultCaptureDevice;

        public void OnDeviceStateChanged(string deviceId, DeviceState deviceState)
        {
            // TODO: output devices changed?
        }

        public void OnDeviceAdded(string deviceId)
        {
            // TODO: output devices changed?
        }

        public void OnDeviceRemoved(string deviceId)
        {
            // TODO: output devices changed?
        }

        public void OnDefaultDeviceChanged(DataFlow dataFlow, Role role, string deviceId)
        {
            if (dataFlow == CaptureDeviceDataFlow && role == CaptureDeviceRole)
            {
                UpdateDefaultDevice();
                DefaultCaptureDeviceChanged?.Invoke();
            }
        }

        public void OnPropertyValueChanged(string deviceId, PropertyKey key)
        {
            // Nothing to do.
        }

        private DeviceService()
        {
            _deviceEnum.RegisterEndpointNotificationCallback(this);
            UpdateDefaultDevice();
        }

        private void UpdateDefaultDevice()
        {
            lock (_lock)
            {
                _defaultCaptureDevice = _deviceEnum.GetDefaultAudioEndpoint(CaptureDeviceDataFlow, CaptureDeviceRole);
                Debug.WriteLine("Default device changed to {0}", _defaultCaptureDevice.FriendlyName);
            }
        }

        private MMDevice GetDefaultCaptureDevice()
        {
            lock (_lock)
            {
                return _defaultCaptureDevice;
            }
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
