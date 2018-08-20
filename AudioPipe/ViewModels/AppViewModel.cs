using AudioPipe.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace AudioPipe.ViewModels
{
    /// <summary>
    /// View model for the main application window.
    /// </summary>
    public class AppViewModel : BindableBase, IAppViewModel
    {
        private IDeviceViewModel selectedDevice;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppViewModel"/> class.
        /// </summary>
        public AppViewModel()
        {
            Devices = new ObservableCollection<IDeviceViewModel>();
            DevicesView = CollectionViewSource.GetDefaultView(Devices);

            Sort();
        }

        /// <inheritdoc/>
        public ObservableCollection<IDeviceViewModel> Devices { get; }

        /// <inheritdoc/>
        public ICollectionView DevicesView { get; }

        /// <inheritdoc/>
        public IDeviceViewModel SelectedDevice
        {
            get => selectedDevice;
            set
            {
                selectedDevice = value;
                NotifyPropertyChanged();
            }
        }

        /// <inheritdoc/>
        public void Refresh()
        {
            var deviceModels = DeviceService.GetOutputDevices().Select(d => new DeviceViewModel(d));

            foreach (var removedItem in Devices.Except(deviceModels, DeviceViewModelEqualityComparer.Instance).ToList())
            {
                Devices.Remove(removedItem);
            }

            foreach (var addedItem in deviceModels.Except(Devices, DeviceViewModelEqualityComparer.Instance).ToList())
            {
                Devices.Add(addedItem);
            }

            foreach (var device in Devices)
            {
                device.RefreshName();
            }

            Sort();
        }

        private void Sort()
        {
            var deviceView = DevicesView as ListCollectionView;
            deviceView.CustomSort = new DeviceSorter();
        }

        private class DeviceSorter : System.Collections.IComparer
        {
            public int Compare(object x, object y)
            {
                var deviceX = x as IDeviceViewModel;
                var deviceY = y as IDeviceViewModel;

                return DeviceViewModelComparer.Instance.Compare(deviceX, deviceY);
            }
        }
    }
}
