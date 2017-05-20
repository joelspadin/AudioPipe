using AudioPipe.Services;
using CSCore.CoreAudioAPI;

namespace AudioPipe.ViewModels
{
    public class MockDeviceViewModel : BindableBase, IDeviceViewModel
    {
        private string _name;

        public MMDevice Device
        {
            get => null;
            set => RaisePropertyChanged(nameof(Device));
        }

        public string DeviceName => IsDefault ? Properties.Resources.DefaultDeviceText : _name;
        public bool IsDefault { get; }

        public MockDeviceViewModel(string name, bool isDefault)
        {
            _name = name;
            IsDefault = isDefault;
        }

        public void RefreshName()
        {
            RaisePropertyChanged(nameof(DeviceName));
        }
    }
}
