using NAudio.CoreAudioApi;

namespace AudioPipe.ViewModels
{
    /// <summary>
    /// <see cref="IDeviceViewModel"/> that shows a fake device.
    /// </summary>
    public class MockDeviceViewModel : BindableBase, IDeviceViewModel
    {
        private readonly string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="MockDeviceViewModel"/> class.
        /// </summary>
        /// <param name="name">The name of the device.</param>
        /// <param name="isDefault">Is this the default device?</param>
        public MockDeviceViewModel(string name, bool isDefault)
        {
            this.name = name;
            IsDefault = isDefault;
        }

        /// <inheritdoc/>
        public MMDevice Device
        {
            get => null;
            set => NotifyPropertyChanged();
        }

        /// <inheritdoc/>
        public string DeviceName => IsDefault ? Properties.Resources.DefaultDeviceText : name;

        /// <inheritdoc/>
        public bool IsDefault { get; }

        /// <inheritdoc/>
        public void RefreshName()
        {
            NotifyPropertyChanged(nameof(DeviceName));
        }
    }
}
