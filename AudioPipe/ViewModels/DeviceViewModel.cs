using AudioPipe.Services;
using CSCore.CoreAudioAPI;

namespace AudioPipe.ViewModels
{
    public class DeviceViewModel : BindableBase
    {
        private MMDevice _device;

        public MMDevice Device
        {
            get => _device;
            set
            {
                _device = value;
                RaisePropertyChanged(nameof(Device));
            }
        }

        public string DeviceName => IsDefault ? Properties.Resources.DefaultDeviceText : Device.FriendlyName;
        public bool IsDefault => DeviceService.Equals(Device, DeviceService.DefaultCaptureDevice);

        public DeviceViewModel(MMDevice device)
        {
            Device = device;
        }

        public void RefreshName()
        {
            RaisePropertyChanged(nameof(DeviceName));
        }
    }
}
