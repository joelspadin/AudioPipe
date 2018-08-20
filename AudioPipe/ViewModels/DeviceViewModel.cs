using AudioPipe.Services;
using CSCore.CoreAudioAPI;

namespace AudioPipe.ViewModels
{
    /// <summary>
    /// View model for an audio device.
    /// </summary>
    public class DeviceViewModel : BindableBase, IDeviceViewModel
    {
        private MMDevice device;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceViewModel"/> class.
        /// </summary>
        /// <param name="device">The device the <see cref="DeviceViewModel"/> represents.</param>
        public DeviceViewModel(MMDevice device)
        {
            Device = device;
        }

        /// <inheritdoc/>
        public MMDevice Device
        {
            get => device;
            set
            {
                device = value;
                NotifyPropertyChanged();
            }
        }

        /// <inheritdoc/>
        public string DeviceName => IsDefault ? Properties.Resources.DefaultDeviceText : Device.FriendlyName;

        /// <inheritdoc/>
        public bool IsDefault => DeviceService.Equals(Device, DeviceService.DefaultPlaybackDevice);

        /// <inheritdoc/>
        public void RefreshName()
        {
            NotifyPropertyChanged(nameof(DeviceName));
        }
    }
}
