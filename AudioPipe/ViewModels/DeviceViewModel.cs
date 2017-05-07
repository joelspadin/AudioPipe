using AudioPipe.Services;
using CSCore.CoreAudioAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public bool IsDefault => DeviceService.Equals(Device, DeviceService.GetDefaultCaptureDevice());

        public DeviceViewModel(MMDevice device)
        {
            Device = device;
        }
    }
}
